using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coffee_App.IRepositories;
using Coffee_App.Models;
using Coffee_App.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Coffee_App.Controllers
{
    [Route("api/orderdetails")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        IOrderDetailRepository _orderDetailRepository;
        IOrderRepository _orderRepository;
        public OrderDetailController(IOrderDetailRepository orderDetailRepository, IOrderRepository orderRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
        }

        [Authorize]
        [HttpPut]
        public ActionResult UpdateOrderDetail(UpdateOrderDertail req)
        {
            if (ModelState.IsValid)
            {
                if (_orderRepository.CheckStatusOrder(req.OrderId) == 0)
                {
                    if (_orderDetailRepository.CheckOrderDetailBelongToOrder(req.OrderDetailId, req.OrderId) == 1)
                    {
                        Order order = _orderRepository.Get(req.OrderId);
                        if (order.UserId.Equals(req.UserId))
                        {
                            OrderDetail orderDetail = _orderDetailRepository.Get(req.OrderDetailId);
                            if (orderDetail != null)
                            {
                                int price = req.UnitPrice;
                                if (req.Mode == 1)
                                {
                                    orderDetail.Quantity += 1; // nếu user giảm quantity thì quantity âm
                                }
                                else if (req.Mode == 0)
                                {
                                    orderDetail.Quantity -= 1;
                                    price = -price;
                                }
                                else
                                {
                                    return BadRequest(JsonConvert.SerializeObject(new { message = "Mode must between 1 or 0" }));
                                }
                                if (_orderDetailRepository.SaveChanges() == 1)
                                {

                                    int result = UpdateOrderAfterBuy(req.OrderId, price);
                                    if (result != -1)
                                    {
                                        return Ok(JsonConvert.SerializeObject(new { totalPrice = result }));
                                    }
                                }
                                return BadRequest(JsonConvert.SerializeObject(new { message = "Server Error." }));
                            }
                        }
                        return BadRequest(JsonConvert.SerializeObject(new { message = "The order is not macth to user." }));
                    }
                    return BadRequest(JsonConvert.SerializeObject(new { message = "Order detail is not match to order." }));
                }
                return BadRequest(JsonConvert.SerializeObject(new { message = "The cart is done." }));
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteOrderDetail(OrderDetailDelModel req)
        {
            if (ModelState.IsValid)
            {
                if (_orderRepository.CheckStatusOrder(req.OrderId) == 0)
                {
                    if (_orderDetailRepository.CheckOrderDetailBelongToOrder(req.OrderDetailId, req.OrderId) == 1)
                    {
                        Order order = _orderRepository.Get(req.OrderId);
                        if (order.UserId.Equals(req.UserId))
                        {
                            OrderDetail orderDetail = _orderDetailRepository.Get(req.OrderDetailId);
                            if (orderDetail != null)
                            {
                                _orderDetailRepository.Delete(req.OrderDetailId);
                                if (_orderDetailRepository.SaveChanges() == 1)
                                {
                                    int price = -(req.Quantity * req.UnitPrice);
                                    int result = UpdateOrderAfterBuy(req.OrderId, price);
                                    if (result != -1)
                                    {
                                        return Ok(JsonConvert.SerializeObject(new { totalPrice = result }));
                                    }
                                }
                                return BadRequest(JsonConvert.SerializeObject(new { message = "Server Error." }));
                            }
                            return BadRequest(JsonConvert.SerializeObject(new { message = "The order detail is not found." }));
                        }
                        return BadRequest(JsonConvert.SerializeObject(new { message = "The order is not macth to user." }));
                    }
                    return BadRequest(JsonConvert.SerializeObject(new { message = "Order detail is not match to order." }));
                }
                return BadRequest(JsonConvert.SerializeObject(new { message = "The cart is done." }));
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Add(OrderDetailRequest req) // ok
        {
            if (ModelState.IsValid)
            {
                if (_orderRepository.CheckStatusOrder(req.OrderId) == 0)
                {
                    OrderDetail o = _orderDetailRepository.GetOrderDetailFollowOption(req.OrderId, req.ProductId, req.Size);
                    if (o != null)
                    {
                        o.Quantity += req.Quantity;

                    }
                    else
                    {
                        OrderDetail orderDetail = new OrderDetail()
                        {
                            OrderId = req.OrderId,
                            ProductId = req.ProductId,
                            ProductName = req.ProductName,
                            Quantity = req.Quantity,
                            UnitPrice = req.UnitPrice,
                            Size = req.Size
                        };
                        _orderDetailRepository.Add(orderDetail);
                    }
                    if (_orderDetailRepository.SaveChanges() > 0)
                    {
                        int totalPrice = req.Quantity * req.UnitPrice;
                        int result = UpdateOrderAfterBuy(req.OrderId, totalPrice);
                        if (result != -1)
                        {
                            return Ok(JsonConvert.SerializeObject(new { totalPrice }));
                        }
                        return BadRequest(JsonConvert.SerializeObject(new { Error = "Update total price in order fail." }));
                    }
                }

                return BadRequest(JsonConvert.SerializeObject(new { Error = "Add fail." }));
            }
            return BadRequest(ModelState);

        }
        private int UpdateOrderAfterBuy(int orderId, int price)
        {
            Order order = _orderRepository.Get(orderId);
            if (order != null)
            {
                order.TotalPrice += price;
                if (_orderRepository.SaveChanges() == 1)
                {
                    return order.TotalPrice;
                }
            }
            return -1;
        }
        [Authorize]
        [HttpGet("orderid/{id}")]
        public ActionResult GetListDetail(int id)
        {
            if (ModelState.IsValid)
            {
                List<OrderDetail> orderDetails = _orderDetailRepository.GetOrderDetailsByOrderId(id);
                if (orderDetails != null)
                {
                    int totalPrice = 0;
                    for (int i = 0; i < orderDetails.Count(); i++)
                    {
                        totalPrice += orderDetails[i].UnitPrice;
                    }
                    return Ok(JsonConvert.SerializeObject(new { orderdetails = orderDetails, totalPrice, status = 1 }));
                }
                return Ok(JsonConvert.SerializeObject(new { status = 0 }));
            }
            return BadRequest(ModelState);
        }

    }
}