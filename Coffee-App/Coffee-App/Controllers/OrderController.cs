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
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Coffee_App.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IOrderRepository _orderRepository;
        IOrderDetailRepository _orderDetailRepository;
        ICouponRepository _couponRepository;

        public OrderController(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, ICouponRepository couponRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _couponRepository = couponRepository;
        }

        [Authorize]
        [HttpPut("confirm")]
        public ActionResult Confirm(ConfirmOrder req)
        {
            if (ModelState.IsValid)
            {
                if (_orderRepository.CheckStatusOrder(req.OrderId) == 0)
                {
                    Order order = _orderRepository.ConfirmOrder(req.OrderId, req.UserId);
                    if (order != null)
                    {
                        order.OrderTime = DateTime.Now;
                        order.Status = 1;
                        if (_orderRepository.SaveChanges() == 1)
                        {
                            return Ok();
                        }
                        else
                        {
                            return BadRequest(JsonConvert.SerializeObject(new { message = "Server Error" }));
                        }
                    }
                    return BadRequest(JsonConvert.SerializeObject(new { message = "Order and user not match." }));
                }
                return BadRequest(JsonConvert.SerializeObject(new { message = "The order has done." }));

            }
            return BadRequest(ModelState);
        }


        [Authorize]
        [HttpPut("coupon")]
        public ActionResult AddCouponToOrder(AddCouponModel req)
        {
            if (ModelState.IsValid)
            {
                if (_orderRepository.CheckStatusOrder(req.OrderId) == 0)
                {
                    if (CheckCouponOfUser(req.UserId, req.CouponId))
                    {
                        Order order = _orderRepository.Get(req.OrderId);
                        if (order.UserId.Equals(req.UserId))
                        {
                            Coupon coupon = _couponRepository.Get(req.CouponId);
                            if (coupon != null)
                            {
                                if (order.CouponId == null)
                                {
                                    if (coupon.Status == 1)
                                    {
                                        if (coupon.ExpiryDate >= DateTime.Now)
                                        {
                                            if (order.TotalPrice >= coupon.Condition)
                                            {
                                                if (coupon.Sale.Contains("%"))
                                                {
                                                    float discount = float.Parse(coupon.Sale.Split('%')[0]) / 100;
                                                    order.TotalPrice = (int)(order.TotalPrice - (order.TotalPrice * discount));
                                                }
                                                else
                                                {
                                                    int discount = int.Parse(coupon.Sale);
                                                    order.TotalPrice = order.TotalPrice - discount;
                                                }
                                                order.CouponId = req.CouponId;
                                                if (_orderRepository.SaveChanges() == 1)
                                                {
                                                    return Ok(JsonConvert.SerializeObject(new { totalPrice = order.TotalPrice }));
                                                }
                                                return BadRequest(JsonConvert.SerializeObject(new { message = "Server Error." }));
                                            }
                                            return BadRequest(JsonConvert.SerializeObject(new { message = "The user is not eligible to receive the coupon." }));
                                        }
                                        return BadRequest(JsonConvert.SerializeObject(new { message = "The coupon has expired." }));
                                    }
                                    return BadRequest(JsonConvert.SerializeObject(new { message = "The coupon is unavailable." }));
                                }
                                return BadRequest(JsonConvert.SerializeObject(new { message = "The order can not take 2 coupons. " }));
                            }
                            return BadRequest(JsonConvert.SerializeObject(new { message = "The coupon not found." }));
                        }
                        return BadRequest(JsonConvert.SerializeObject(new { message = "The order is not macth to user." }));
                    }
                    return BadRequest(JsonConvert.SerializeObject(new { message = "The coupon has been used." }));

                }
                return BadRequest(JsonConvert.SerializeObject(new { message = "The cart is done." }));
            }
            return BadRequest(ModelState);

        }

        private bool CheckCouponOfUser(string userId, string couponId)
        {
            bool result = _orderRepository.CheckCounpoInOrder(userId, couponId);
            return result;
        }


        [Authorize]
        [HttpGet("userid/{id}")]
        public ActionResult GetOrderId(string id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int orderId = _orderRepository.GetOrderIdByUserId(id);
                    if (orderId != -1)
                    {
                        int amountOrderDetail = _orderDetailRepository.GetAmountByOrderId(orderId);
                        string json = JsonConvert.SerializeObject(new { OrderId = orderId, AmountDetail = amountOrderDetail, Message = "Your cart is ready." });
                        return Ok(json);
                    }
                    else
                    {
                        string json = JsonConvert.SerializeObject(new { OrderId = -1, AmountDetail = -1, Message = "No cart right now." });
                        return BadRequest(json);
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(JsonConvert.SerializeObject(new { OrderId = -1, AmountDetail = -1, Message = e.InnerException.Message }));
                }
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Add(OrderRequest req)
        {
            if (ModelState.IsValid)
            {
                Order order = new Order()
                {
                    UserId = req.UserId,
                    OrderTime = DateTime.Now,
                    Address = req.Address,
                    ReceiverName = req.ReceiverName,
                    Phone = req.Phone,
                    TotalPrice = req.TotalPrice,
                    Status = 0
                };
                _orderRepository.Add(order);
                if (_orderRepository.SaveChanges() > 0)
                {
                    int orderId = _orderRepository.GetOrderIdByUserId(req.UserId);
                    string json = JsonConvert.SerializeObject(new { OrderId = orderId, Message = "Create order successed." });
                    return Ok(json);
                }
            }
            return BadRequest(ModelState);
        }
    }
}