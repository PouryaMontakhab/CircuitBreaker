using CircuitBreaker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [HttpGet]
        public IActionResult Ten()
        {
            counter += 1;
            if (counter % 10 == 0)
            {
                return Ok(new Message
                {
                    Id = counter,
                    Text = "Request was returned ok when the remainder of 10 is equal to 0"
                });
            }
            return BadRequest(new Message
            {
                Id = counter,
                Text = "Request was returned not ok when the remainder of 10 is not equal to 0"
            });
        }


        [HttpGet]
        public IActionResult Long()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            System.Threading.Thread.Sleep(20000);
            stopwatch.Stop();

            return Ok(new Message
            {
                Id = counter,
                Text = $"Your request was responsed in {stopwatch.ElapsedMilliseconds / 1000} seconds."
            });
        }

        [HttpGet]
        public IActionResult Ex()
        {
            throw new Exception("The Exeption has occured...");
            return Ok(new Message
            {
                Id=counter,
                Text = "Request was Responsed OK"
            });
        }
    }
}
