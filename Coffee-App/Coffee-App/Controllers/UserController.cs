using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coffee_App.Cache;
using Coffee_App.IRepositories;
using Coffee_App.Models;
using Coffee_App.RequestModels;
using Coffee_App.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Coffee_App.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserRepository _userRepository;
        ICoffeeToken _coffeeToken;

        public UserController(IUserRepository userRepository, ICoffeeToken coffeeToken)
        {
            _userRepository = userRepository;
            _coffeeToken = coffeeToken;
        }

        [HttpPost]
        public ActionResult Register(RequestBinder req)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_userRepository.Get(req.UserId) != null)
                    {
                        var response = _coffeeToken.CreateToken(req.UserId); // status =0 is no error
                        return Ok(JsonConvert.SerializeObject(response));
                    }
                    User user = new User();
                    user.UserId = req.UserId;
                    user.Fullname = req.Fullname;
                    user.ProviderId = req.ProviderId;
                    user.Image = req.Image;
                    user.Status = 0;
                    user.Role = 0;
                    user.CreateDate = DateTime.Now;
                    if (req.Identifier.Contains("@"))
                    {
                        user.Email = req.Identifier;
                    }
                    else
                    {
                        user.Phone = req.Identifier;
                    }
                    _userRepository.Add(user);
                    if (_userRepository.SaveChanges() == 1) // register success
                    {
                        var response = _coffeeToken.CreateToken(req.UserId);
                        return Ok(JsonConvert.SerializeObject(response));
                    }
                }
                catch (Exception ex)
                {
                    var response = new ResponseRegisterModel() { Token = null, Status = 1, RefreshToken = "", Error = "Error Server. " + ex.InnerException.Message }; //
                    return BadRequest(JsonConvert.SerializeObject(response));
                }
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpGet("logout/{id}")]
        public ActionResult Logout(string id)
        {
            if (ModelState.IsValid)
            {
                Dictionary<string, DateTime> jwtCache = JWTTokenCache.AppCache;
                string token = Request.Headers["Authorization"];
                token = token.Substring(7);
                jwtCache.Remove(token);
                return Ok();

            }
            return BadRequest(ModelState);
        }

        [HttpPost("refresh-token")]
        public ActionResult RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            if (ModelState.IsValid)
            {
                string resultCheck = _coffeeToken.CheckRefreshToken(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken);
                if (resultCheck == "OK")
                {
                    string userId = _coffeeToken.GetUserIdFromJWT(refreshTokenRequest.Token);
                    var rep = _coffeeToken.CreateToken(userId);
                    return Ok(JsonConvert.SerializeObject(rep));
                }
                else
                {
                    return BadRequest(JsonConvert.SerializeObject(new { Error = resultCheck }));
                }
            }
            return BadRequest(ModelState);
        }


        [Authorize]
        [HttpPut("update/id/{id}")]
        public ActionResult UpdateUser(string id, RequestUpdateUser userUpdate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User u = _userRepository.Get(id);
                    if (u != null)
                    {
                        u.Fullname = userUpdate.Fullname;
                        u.Phone = userUpdate.Phone;
                        u.Image = userUpdate.Image;
                        _userRepository.Update(u);
                        if (_userRepository.SaveChanges() == 1)
                        {
                            return Ok();
                        }
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(JsonConvert.SerializeObject(new { error = e.InnerException.Message }));
                }
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPut("update/address/id/{id}")]
        public ActionResult UpdateUserAddress(string id, RequestUpdateUser userUpdate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User u = _userRepository.Get(id);
                    if (u != null)
                    {
                        u.Address = userUpdate.address;
                        _userRepository.Update(u);
                        if (_userRepository.SaveChanges() == 1)
                        {
                            return Ok();
                        }
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(JsonConvert.SerializeObject(new { error = e.InnerException.Message }));
                }
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpGet("id/{id}")]
        public ActionResult GetInfo(string id)
        {
            try
            {
                User u = _userRepository.Get(id);
                if (u != null)
                {
                    var responseUser = new
                    {
                        UserId = u.UserId,
                        Fullname = u.Fullname,
                        Email = u.Email,
                        Phone = u.Phone,
                        Address = u.Address,
                        ProviderId = u.ProviderId,
                        Image = u.Image
                    };
                    string json = JsonConvert.SerializeObject(responseUser);
                    return Ok(json);
                }

            }
            catch (Exception e)
            {
                return BadRequest(JsonConvert.SerializeObject(new { error = e.InnerException.Message }));
            }
            return BadRequest(JsonConvert.SerializeObject(new { message = "This userId is not existed." }));
        }

        // set token into cookie
        /*   [HttpPost("authenticate")]
           public IActionResult Authenticate([FromBody]LoginRequest loginRequest)
           {

               //validate user credentials and if they validation failed return a similar response to below
               //return NotFound();

               var tokenHandler = new JwtSecurityTokenHandler();
               var key = Encoding.ASCII.GetBytes("MySecurelyInjectedAsymKey");
               var tokenDescriptor = new SecurityTokenDescriptor
               {
                   Subject = new ClaimsIdentity(new Claim[]
                   {
                       //add my users claims etc
                   }),
                   Expires = DateTime.UtcNow.AddDays(1),//configure your token lifespan and needed
                   SigningCredentials = new SigningCredentials(new SymmetricSecurityKey("MyVerySecureSecreteKey"), SecurityAlgorithms.HmacSha256Signature),
                   Issuer = "YourOrganizationOrUniqueKey",
                   IssuedAt = DateTime.UtcNow
               };

               var token = tokenHandler.CreateToken(tokenDescriptor);
               var tokenString = tokenHandler.WriteToken(token);
               var cookieOptions = new CookieOptions();
               cookieOptions.Expires = DateTimeOffset.UtcNow.AddHours(4);//you can set this to a suitable timeframe for your situation 
               cookieOptions.Domain = Request.Host.Value;
               cookieOptions.Path = "/";
               Response.Cookies.Append("jwt", tokenString, cookieOptions);
               return Ok();

           }*/
    }
}