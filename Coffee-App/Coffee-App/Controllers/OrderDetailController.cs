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
        public OrderDetailController(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        [Authorize]
        [HttpPost]
        public ActionResult Add(OrderDetailRequest req)
        {
            if (ModelState.IsValid)
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

                    return Ok();
                }
                return BadRequest(JsonConvert.SerializeObject(new { Error = "Add fail." }));
            }
            return BadRequest();

        }
    }
}