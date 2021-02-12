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
                if(_orderRepository.CheckStatusOrder(req.OrderId) == 0)
                {
                    if(_orderDetailRepository.CheckOrderDetailBelongToOrder(req.OrderDetailId,req.OrderId)==1)
                    {
                        OrderDetail orderDetail = _orderDetailRepository.Get(req.OrderDetailId);
                        if (orderDetail != null)
                        {
                            orderDetail.Quantity = req.Quantity;
                            if (_orderDetailRepository.SaveChanges() == 1)
                            {
                                int totalPrice = req.Quantity * req.UnitPrice;
                                if (UpdateOrderAfterBuy(req.OrderId, totalPrice) == 1)
                                {
                                    return Ok();
                                }
                            }
                            return BadRequest(JsonConvert.SerializeObject(new { message = "Server Error." }));
                        }
                    }
                    return BadRequest(JsonConvert.SerializeObject(new { message = "Order detail is not match to order." }));
                }
                return BadRequest(JsonConvert.SerializeObject(new { message = "The cart is done." }));
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Add(OrderDetailRequest req)
        {
            if (ModelState.IsValid)
            {
                if (_orderRepository.CheckStatusOrder(req.OrderId) == 0)
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
                    if (_orderDetailRepository.SaveChanges() > 0)
                    {
                        int totalPrice = req.Quantity * req.UnitPrice;
                        if (UpdateOrderAfterBuy(req.OrderId, totalPrice) == 1)
                        {
                            return Ok();
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
                order.TotalPrice = order.TotalPrice + price;
                if (_orderRepository.SaveChanges() == 1)
                {
                    return 1;
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
                IQueryable<OrderDetail> orderDetails = _orderDetailRepository.GetOrderDetailsByOrderId(id);
                if (orderDetails != null)
                {
                    return Ok(JsonConvert.SerializeObject(new { orderdetails = orderDetails, status = 1 }));
                }
                return Ok(JsonConvert.SerializeObject(new { status = 0 }));
            }
            return BadRequest(ModelState);
        }

    }
}