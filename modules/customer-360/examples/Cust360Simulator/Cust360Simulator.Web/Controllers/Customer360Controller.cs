using Cust360Simulator.Core;
using Microsoft.AspNetCore.Mvc;

namespace Cust360Simulator.Web.Controllers
{
    [ApiController]
    public class Customer360Controller : ControllerBase
    {
        private readonly HomeDeliveryRepository _homeDeliveryRepository;
        private readonly LoyaltyRepository _loyaltyRepository;

        public Customer360Controller(HomeDeliveryRepository homeDeliveryRepository, LoyaltyRepository loyaltyRepository)
        {
            _homeDeliveryRepository = homeDeliveryRepository;
            _loyaltyRepository = loyaltyRepository;
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

        [HttpGet]
        [Route("api/loyalty/create")]
        public IActionResult CreateLoyaltyMember()
        {
            _loyaltyRepository.CreateNewMember();
            return Ok("Success");
        }

        [HttpGet]
        [Route("api/loyalty/update")]
        public IActionResult UpdateLoyaltyMember()
        {
            _loyaltyRepository.UpdateLoyaltyMember();
            return Ok("Success");
        }
    }
}
