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
        ICouponRepository _couponRepository;
        IOrderRepository _orderRepository;

        public CouponController(ICouponRepository couponRepository, IOrderRepository orderRepository)
        {
            _couponRepository = couponRepository;
            _orderRepository = orderRepository;
        }

        [HttpGet("userid/{id}")]
        public async Task<ActionResult> getAllCoupons(string id)
        {
            try
            {
                List<Coupon> coupons =await  _couponRepository.GetAvailableCoupon();
                List<string> listCouponIdsAvailable = new List<string>();
                for (int i = 0; i < coupons.Count; i++)
                {
                    listCouponIdsAvailable.Add(coupons[i].CouponId);
                }
                List<string> couponIdsUsed = await _orderRepository.GetListCouponIdInOrderOfUser(id);
                List<Coupon> returnCoupons = new List<Coupon>();
                for (int i = 0; i < couponIdsUsed.Count; i++)
                {
                    if (listCouponIdsAvailable.Contains(couponIdsUsed[i]))
                    {
                        listCouponIdsAvailable.Remove(couponIdsUsed[i]);
                    }
                }

                for (int i = 0; i < coupons.Count; i++)
                {
                    if (listCouponIdsAvailable.Contains(coupons[i].CouponId))
                    {
                        returnCoupons.Add(coupons[i]);
                    }
                }
                string json = JsonConvert.SerializeObject(returnCoupons);
                return Ok(json);
            }
            catch (Exception e)
            {
                return BadRequest(JsonConvert.SerializeObject(new { Error = e.Message }));
            }
        }
    }
}