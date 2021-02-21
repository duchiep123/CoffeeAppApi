using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coffee_App.IRepositories;
using Coffee_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Coffee_App.Controllers
{
    [Route("api/stores")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        IStoreRepository _storeRepository;

        public StoreController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        [HttpGet]
        public ActionResult GetAllStores()
        {
            try
            {
                IEnumerable<Store> stores = _storeRepository.GetAll();
                return Ok(JsonConvert.SerializeObject(stores));
            }
            catch (Exception)
            {

                return BadRequest(JsonConvert.SerializeObject(new { message = "Server error." }));
            }

        }

    }
}