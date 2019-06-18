using System;
using Cust360Simulator.Core;
using Cust360Simulator.Core.OnlineStore;
using Microsoft.AspNetCore.Mvc;

namespace Cust360Simulator.Web.Controllers
{
    [ApiController]
    public class OnlineStoreController : ControllerBase
    {
        private readonly OnlineStoreRepository _onlineStoreRepository;

        public OnlineStoreController(OnlineStoreRepository onlineStoreRepository)
        {
            _onlineStoreRepository = onlineStoreRepository;
        }

        [HttpGet]
        [Route("api/onlineStore/getCustomerDetailsByEmail")]
        public IActionResult GetCustomerDetailsByEmail(string customerEmail)
        {
            if (string.IsNullOrEmpty(customerEmail))
                return BadRequest("You must specify customerEmail");

            if (!_onlineStoreRepository.DoesCustomerExistsByEmail(customerEmail))
                return NotFound($"No customer with customerEmail '{customerEmail}' was found.");

            return Ok(_onlineStoreRepository.GetCustomerDetailsByEmail(customerEmail));
        }

        [HttpGet]
        [Route("api/onlineStore/getOrdersByCustomerId")]
        public IActionResult GetOrdersByCustomerId(int customerId)
        {
            return Ok(_onlineStoreRepository.GetOrdersByCustomerId(customerId));
        }

        [HttpGet]
        [Route("api/onlineStore/getProductDetailsByProductId")]
        public IActionResult GetProductDetailsByProductId(int productId)
        {
            return Ok(_onlineStoreRepository.GetProductDetailsByProductId(productId));
        }
    }
}