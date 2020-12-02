﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coffee_App.IRepositories;
using Coffee_App.Models;
using Coffee_App.RequestModels;
using Coffee_App.Token;
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
                catch (DbUpdateException)
                {
                    var response = new ResponseRegisterModel() { Token = null, Status = 1, Error = "This user ID is exsisted." }; // tk da dc tạo 
                    return BadRequest(JsonConvert.SerializeObject(response));

                }
                catch (Exception)
                {
                    var response = new ResponseRegisterModel() { Token = null, Status = 2, Error = "Error Server." }; //
                    return BadRequest(JsonConvert.SerializeObject(response));
                }
            }
            return BadRequest();
        }
    }
}