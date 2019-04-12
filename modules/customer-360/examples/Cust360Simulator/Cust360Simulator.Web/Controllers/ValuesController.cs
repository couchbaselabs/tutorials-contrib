using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cust360Simulator.Core;
using Microsoft.AspNetCore.Mvc;

namespace Cust360Simulator.Web.Controllers
{
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly HomeDeliveryRepository _homeDeliveryRepository;

        public ValuesController(HomeDeliveryRepository homeDeliveryRepository)
        {
            _homeDeliveryRepository = homeDeliveryRepository;
        }

        // GET api/values
        [Route("api/homedelivery/create")]
        public IActionResult CreateHomeDeliveryUser()
        {
            _homeDeliveryRepository.CreateNewCustomer();
            return Ok("Success");
        }
    }
}
