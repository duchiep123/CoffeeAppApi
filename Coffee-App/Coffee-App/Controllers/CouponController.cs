using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coffee_App.IRepositories;
using Coffee_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Coffee_App.Controllers
{
    [Route("api/coupons")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        IConfiguration _config;
        ICouponRepository _couponRepository;

        public CouponController(IConfiguration config, ICouponRepository couponRepository)
        {
            _config = config;
            _couponRepository = couponRepository;
        }

        [HttpGet]
        public ActionResult getAllCoupons()
        {
            try
            {
                IEnumerable<Coupon> coupons = _couponRepository.GetAll();
                string json = JsonConvert.SerializeObject(coupons);
                return Ok(json);
            }
            catch (Exception e)
            {
                return BadRequest(JsonConvert.SerializeObject(new { Error = e.Message }));
            }
        }
    }
}