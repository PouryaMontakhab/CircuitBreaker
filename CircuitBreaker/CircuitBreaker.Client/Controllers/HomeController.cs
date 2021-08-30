using CircuitBreaker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CircuitBreaker.Client.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        private string _baseAddress = "https://localhost:44314/Home/Odd";

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(_baseAddress);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ClientMessage>(stringResponse);
            return Ok(deserializedResponse);
        }
    }
}
