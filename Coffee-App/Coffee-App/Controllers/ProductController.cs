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
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IConfiguration _config;
        IProductRepository _productRepository;
        ICouponRepository _couponRepository;
        IProductSizeRepository _productSizeRepository;

        public ProductController(IConfiguration config, IProductRepository productRepository, ICouponRepository couponRepository, IProductSizeRepository productSizeRepository)
        {
            _config = config;
            _productRepository = productRepository;
            _couponRepository = couponRepository;
            _productSizeRepository = productSizeRepository;
        }

        [HttpGet]
        public ActionResult FirstLoad()
        {
            try
            {
                IEnumerable<Product> products = _productRepository.GetAll();
                IEnumerable<ProductSize> productSizes = _productSizeRepository.GetAll();
                string json = JsonConvert.SerializeObject(new { Products = products, Sizes = productSizes });
                return Ok(json);
            }
            catch (Exception e)
            {
                return BadRequest(JsonConvert.SerializeObject(new { Error = e.Message}));
            }
        }


    }
}