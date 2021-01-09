﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coffee_App.IRepositories;
using Coffee_App.Models;
using Coffee_App.RequestModels;
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

        public OrderController( IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
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