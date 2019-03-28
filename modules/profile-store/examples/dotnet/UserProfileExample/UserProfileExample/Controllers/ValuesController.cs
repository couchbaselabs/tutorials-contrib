using System;
using System.Collections.Generic;
using System.Linq;
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
            user.Id = Guid.NewGuid().ToString();
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
        public IActionResult Populate()
        {
            // this method will populate the Couchbase bucket with a bunch of randomish user profiles
            for(var i=0;i<100;i++)
            {
                _userRepository.Save(FakeUser.Create());
            }

            // also add some Alex, Allex, and Alec users
            _userRepository.Save(FakeUser.Create(firstName: "Alex", enabled: true, countryCode: "US"));
            _userRepository.Save(FakeUser.Create(firstName: "Allex", enabled: true, countryCode: "US"));
            _userRepository.Save(FakeUser.Create(firstName: "Alec", enabled: true, countryCode: "US"));

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
    }
}
