using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        IConfiguration _config;
        CoffeeToken token;

        public UserController(IUserRepository userRepository, IConfiguration config)
        {
            _config = config;
            _userRepository = userRepository;
            token = new CoffeeToken(config);

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
                        string coffeeToken = token.CreateToken();
                        var response = new ResponseRegisterModel() { Token = coffeeToken, Status = 0, Error = null }; //
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
                        string coffeeToken = token.CreateToken();
                        var response = new ResponseRegisterModel() { Token = coffeeToken, Status = 0, Error = null };
                        return Ok(JsonConvert.SerializeObject(response));
                    }
                }

                catch (Exception ex)
                {
                    var response = new ResponseRegisterModel() { Token = null, Status = 2, Error = "Error Server. " + ex.InnerException.Message }; //
                    return BadRequest(JsonConvert.SerializeObject(response));
                }
            }
            return BadRequest();
        }

        [Authorize]
        [HttpGet("update/id/{id}")]
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
            return BadRequest();
        }

        [Authorize]
        [HttpGet("update/address/id/{id}")]
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
            return BadRequest();
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
    }
}