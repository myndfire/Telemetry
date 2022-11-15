using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.ServiceA.Controllers
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
        public async  Task<IEnumerable<WeatherForecast>> Get()
        {
            IEnumerable<WeatherForecast> response;
            HttpContext.Response.Headers.Add("Request-Id", Activity.Current?.TraceId.ToString()??string.Empty);
           
            using (Activity activity = Startup._ActivitySource.StartActivity("Generating Random WeatherForecast", ActivityKind.Server))
            {
                _logger.LogInformation("Hello {Name}! It is {Time}", "Joe", DateTime.UtcNow);

                activity?.AddTag("WebApiOperation","WeatherForecast Get");
                activity?.AddBaggage("WebApiRequestId", new Guid().ToString());

                await LongRunningOperationAsync().ConfigureAwait(false);


                //using var httpClient = HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("WebApi.ServiceB");
                //var content = httpClient.GetStringAsync("/weatherforecast").Result;

                var rng = new Random();
                response= Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    })
                    .ToArray();
            }

            return response;
        }

        public async Task LongRunningOperationAsync()
        {
            await Task.Delay(1000).ConfigureAwait(false);

            // Log timestamped events that can take place during an activity.
            Activity.Current?.AddEvent(new ActivityEvent("LongRunningOperationAsync"));
        }
    }
}
