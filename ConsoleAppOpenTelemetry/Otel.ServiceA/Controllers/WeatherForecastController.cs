using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Otel.ServiceA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {

            Startup.requestCounter.Add(1);
            using (var activity = Startup.activitySource.StartActivity("Get WeatherForecast data"))
            {
                // Add data the the activity
                // You can see these data in Zipkin
                activity?.AddTag("sample", "value");

                // Http calls are tracked by AddHttpClientInstrumentation
                var httpClient = new HttpClient();

                var str1 = await httpClient.GetStringAsync("https://example.com");
                var str2 = await httpClient.GetStringAsync("https://www.aol.com/");

                _logger.LogInformation("Response1 length: {Length}", str1.Length);
                _logger.LogInformation("Response2 length: {Length}", str2.Length);

                var rng = new Random();
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    })
                    .ToArray();
            }
            
        }
    }
}
