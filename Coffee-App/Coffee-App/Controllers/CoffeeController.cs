using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Coffee_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeController : ControllerBase
    {
        IConfiguration _config;

        public CoffeeController(IConfiguration config)
        {
            _config = config;
        }


    }
}