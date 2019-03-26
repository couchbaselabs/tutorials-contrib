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
        // GET api/doSomething
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
    }
}
