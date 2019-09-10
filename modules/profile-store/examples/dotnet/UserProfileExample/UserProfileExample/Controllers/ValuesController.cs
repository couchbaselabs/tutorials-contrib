using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserProfileExample.Models;

namespace UserProfileExample.Controllers
{
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // tag::ctor[]
        private UserRepository _userRepository;

        public ValuesController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        // end::ctor[]

        // tag::doSomething[]
        [HttpGet]
        [Route("api/doSomething")]
        public IActionResult DoSomething()
        {
            var user = new User();
            user.Id = "user::" + Guid.NewGuid().ToString();
            user.CountryCode = "DE";
            user.Password = "letmein";
            user.UserName = "Bilbo";

            _userRepository.Save(user);

            // read after write
            var savedUser = _userRepository.FindById(user.Id);
            return Ok(savedUser);
        }
        // end::doSomething[]

        [HttpGet]
        [Route("api/populate")]
        public async Task<IActionResult> Populate()
        {
            // this method will populate the Couchbase bucket with a bunch of randomish user profiles
            var users = new List<User>();
            var numUsers = Faker.RandomNumber.Next(80, 120);
            for(var i=0;i< numUsers; i++)
            {
                var user = FakeUser.Create();
                _userRepository.Save(user);
                users.Add(user);
            }

            // also add some Alex, Allex, and Alec users
            _userRepository.Save(FakeUser.Create(firstName: "Alex", enabled: true, countryCode: "US"));
            _userRepository.Save(FakeUser.Create(firstName: "Allex", enabled: true, countryCode: "US"));
            _userRepository.Save(FakeUser.Create(firstName: "Alec", enabled: true, countryCode: "US"));

            // also add some randomish User Events
            var numEvents = Faker.RandomNumber.Next(80, 120);
            for (var i = 0; i < numEvents; i++)
            {
                // I'm only pulling from the first 10 users just to increase event density
                var randomUser = users[Faker.RandomNumber.Next(0, 10)];
                var fakeUserActivity = UserEventGenerator.Create(randomUser.Id);
                await _userRepository.AddEventAsync(fakeUserActivity);
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/listTenantUsers")]
        public IActionResult ListTenantUsers(int? tenantId, int offset = 0, int limit = 10)
        {
            if(!tenantId.HasValue)
                return BadRequest("tenantId is required");
            
            var result = _userRepository.ListTenantUsers(tenantId.Value, offset, limit);

            return Ok(result);
        }

        [HttpGet]
        [Route("api/findActiveUsersByFirstName")]
        public IActionResult FindActiveUsersByFirstName(string firstName, bool? enabled, string countryCode,
            int limit = 10, int offset = 0)
        {
            if (string.IsNullOrEmpty(firstName))
                return BadRequest("firstName is required");
            if (!enabled.HasValue)
                return BadRequest("enabled is required");
            if (string.IsNullOrEmpty(countryCode))
                return BadRequest("countryCode is required");

            var result = 
                _userRepository.FindActiveUsersByFirstName(firstName, enabled.Value, countryCode, limit, offset);

            return Ok(result);
        }

        [HttpGet]
        [Route("api/ftsListActiveUsers")]
        public IActionResult FtsListActiveUsers(string firstName, bool? enabled, string countryCode, int limit = 10,
            int skip = 0)
        {
            if (string.IsNullOrEmpty(firstName))
                return BadRequest("firstName is required");
            if (!enabled.HasValue)
                return BadRequest("enabled is required");
            if (string.IsNullOrEmpty(countryCode))
                return BadRequest("countryCode is required");

            var result = _userRepository.FtsListActiveUsers(firstName, enabled.Value, countryCode, limit, skip);

            return Ok(result);
        }

        // tag::FindLatestUserEvents[]
        [HttpGet]
        [Route("api/findLatestUserEvents")]
        public async Task<IActionResult> FindLatestUserEvents(string userId, EventType? eventType, int limit = 50, int offset = 0)
        {
            if(string.IsNullOrEmpty(userId))
                return BadRequest("userId is required");
            if(!eventType.HasValue)
                return BadRequest("eventType is required");

            var result = await _userRepository.FindLatestUserEvents(userId, eventType.Value, limit, offset);

            return Ok(result);
        }
        // end::FindLatestUserEvents[]

        [HttpGet]
        [Route("api/eventTimeSeries")]
        public async Task<IActionResult> GetTimeSeriesDataForEventType(EventType? eventType, DateTime? startDate, DateTime? endDate)
        {
            if (!eventType.HasValue)
                return BadRequest("You must specify an event type.");
            if(!startDate.HasValue)
                return BadRequest("You must specify a start date.");
            if(!endDate.HasValue)
                return BadRequest("You must specify an end date.");

            var result = await _userRepository.GetTimeSeriesDataForEventType(eventType.Value, startDate.Value, endDate.Value);

            return Ok(result);
        }
    }
}
