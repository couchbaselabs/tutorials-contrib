using System.Threading.Tasks;
using Cust360Simulator.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cust360Simulator.Web.Controllers
{
    [ApiController]
    public class Customer360Controller : ControllerBase
    {
        private readonly HomeDeliveryRepository _homeDeliveryRepository;
        private readonly LoyaltyRepository _loyaltyRepository;
        private readonly LoyaltyCsvExportService _loyaltyCsvExportService;
        private readonly LoyaltyCsvImportService _loyaltyCsvImportService;
        private readonly ILogger _logger;

        public Customer360Controller(
            HomeDeliveryRepository homeDeliveryRepository,
            LoyaltyRepository loyaltyRepository,
            LoyaltyCsvExportService loyaltyCsvExportService,
            LoyaltyCsvImportService loyaltyCsvImportService,
            ILogger<Customer360Controller> logger)
        {
            _homeDeliveryRepository = homeDeliveryRepository;
            _loyaltyRepository = loyaltyRepository;
            _loyaltyCsvExportService = loyaltyCsvExportService;
            _loyaltyCsvImportService = loyaltyCsvImportService;
            _logger = logger;
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

        [HttpGet]
        [Route("api/loyalty/produceCsv")]
        public IActionResult ProduceLoyaltyCsv()
        {
            _loyaltyCsvExportService.ExportCsv();

            return Ok("Success");
        }

        [HttpGet]
        [Route("api/loyalty/consumeCsv")]
        public async Task<IActionResult> ConsumeLoyaltyCsv()
        {
            await _loyaltyCsvImportService.ImportCsv();

            return Ok("Success");
        }

        [HttpGet]
        [Route("api/testlogging")]
        public IActionResult TestLogging()
        {
            _logger.Log(LogLevel.Information, "Hello, world!");
            _logger.Log(LogLevel.Error, "This is an error!");
            return Ok();
        }
    }
}
