using CircuitBreaker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircuitBreaker.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        static int counter = 0;

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new Message { Id = 1, Text = "Request was returned ok" });
        }

        [HttpGet]
        public IActionResult Odd()
        {
            counter += 1;
            if (counter % 2 != 0)
            {
                return Ok(new Message
                {
                    Id = counter,
                    Text = "Request was returned ok in odd mode"
                });
            }
            return BadRequest(new Message
            {
                Id = counter,
                Text = "Request was returned bad request in even mode"
            });
        }
    }
}
