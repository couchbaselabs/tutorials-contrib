using Cust360Simulator.Core;
using Microsoft.AspNetCore.Mvc;

namespace Cust360Simulator.Web.Controllers
{
    [ApiController]
    public class Customer360Controller : ControllerBase
    {
        private readonly HomeDeliveryRepository _homeDeliveryRepository;

        public Customer360Controller(HomeDeliveryRepository homeDeliveryRepository)
        {
            _homeDeliveryRepository = homeDeliveryRepository;
        }

        [HttpGet]
        [Route("api/homedelivery/create")]
        public IActionResult CreateHomeDeliveryUser()
        {
            _homeDeliveryRepository.CreateNewCustomer();
            return Ok("Success");
        }

        [HttpGet]
        [Route("api/homedelivery/update")]
        public IActionResult UpdateHomeDeliveryUser()
        {
            _homeDeliveryRepository.UpdateExistingCustomer();
            return Ok("Success");
        }
    }
}
