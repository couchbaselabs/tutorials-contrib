using System.Threading.Tasks;
using Cust360Simulator.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cust360Simulator.Web.Controllers
{
    /// <summary>
    /// This controller class serves as a kind of 'test harness'
    /// For controlling all the pieces manually and individually
    /// Automated simulation should be done by automated/scheduled jobs
    /// </summary>
    [ApiController]
    public class Customer360Controller : ControllerBase
    {
        private readonly HomeDeliveryRepository _homeDeliveryRepository;
        private readonly LoyaltyRepository _loyaltyRepository;
        private readonly LoyaltyCsvExportService _loyaltyCsvExportService;
        private readonly LoyaltyCsvImportService _loyaltyCsvImportService;
        private readonly ILogger _logger;
        private readonly EnterpriseData _enterpriseData;
        private readonly OnlineStoreRepository _onlineStoreRepository;

        public Customer360Controller(
            HomeDeliveryRepository homeDeliveryRepository,
            LoyaltyRepository loyaltyRepository,
            LoyaltyCsvExportService loyaltyCsvExportService,
            LoyaltyCsvImportService loyaltyCsvImportService,
            ILogger<Customer360Controller> logger,
            EnterpriseData enterpriseData,
            OnlineStoreRepository onlineStoreRepository)
        {
            _homeDeliveryRepository = homeDeliveryRepository;
            _loyaltyRepository = loyaltyRepository;
            _loyaltyCsvExportService = loyaltyCsvExportService;
            _loyaltyCsvImportService = loyaltyCsvImportService;
            _logger = logger;
            _enterpriseData = enterpriseData;
            _onlineStoreRepository = onlineStoreRepository;

            // make sure all the appropriate tables/databases exist
            _enterpriseData.EnsureSchemas();
        }

        [HttpGet]
        [Route("api/populate")]
        public IActionResult Populate()
        {
            _enterpriseData.PopulateAPersonAcrossEnterprise();
            return Ok("Done");
        }

        [HttpGet]
        [Route("api/homedelivery/update")]
        public IActionResult UpdateHomeDeliveryUser()
        {
            _homeDeliveryRepository.UpdateExistingCustomer();
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

        [HttpGet]
        [Route("api/onlineStore/update")]
        public IActionResult UpdateOnlineStoreCustomer()
        {
            _onlineStoreRepository.UpdateExisting();
            return Ok("Success");
        }
    }
}
