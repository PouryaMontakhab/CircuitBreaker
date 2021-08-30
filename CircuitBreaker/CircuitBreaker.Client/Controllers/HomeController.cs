using CircuitBreaker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace CircuitBreaker.Client.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        private string _baseAddress = "https://localhost:44314/Home/Ten";
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly AsyncFallbackPolicy<HttpResponseMessage> _fallbackPolicy;
        private static AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy =
            Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode).Or<HttpRequestException>()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(10),
                   (a, b) =>
                   {
                       string circuitLogMessage = "Break";
                   },
                    () =>
                    {
                        string circuitLogMessage = "Reset";
                    }, () =>
                    {
                        string circuitLogMessage = "Half";
                    }
                );

        public HomeController()
        {
            _retryPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .RetryAsync(5, (a, b) => { string s = "retry"; });

            _fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode).Or<BrokenCircuitException>()
                .FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                      Content = new ObjectContent(typeof(Message), new Message { 
                        Id=100,
                        Text="Try again a few second later"
                      }, new JsonMediaTypeFormatter())
                });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var httpClient = new HttpClient();
            var response = await _fallbackPolicy.ExecuteAsync(
                () => _retryPolicy.ExecuteAsync(
                    () => _circuitBreakerPolicy.ExecuteAsync(
                        () => httpClient.GetAsync(_baseAddress))));
            var stringResponse = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ClientMessage>(stringResponse);
            return Ok(deserializedResponse);
        }
    }
}
