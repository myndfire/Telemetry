using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibraryA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Platform.Logging.Api.Attributes;
using Platform.Logging.Api.Filters;

namespace WebApiSerilogElasticsearch.Controllers
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
        private RequestManager _requestManager;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, RequestManager requestManager)
        {
            _logger = logger;
            _requestManager = requestManager;
        }

        [HttpGet]
        //[ServiceFilter(typeof(TrackActionPerformanceFilter))]
        [TypeFilter(typeof(TrackPerformance))]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogTrace("IEnumerable<WeatherForecast> Get() (Trace)");
            _logger.LogDebug("IEnumerable<WeatherForecast> Get() (Debug)");
            _logger.LogInformation("IEnumerable<WeatherForecast> Get() (Information)");
            _logger.LogWarning("IEnumerable<WeatherForecast> Get() (Warning)");
            _logger.LogError("IEnumerable<WeatherForecast> Get() (Error)");
            _logger.LogCritical("IEnumerable<WeatherForecast> Get() (Fatal)");

            //Use Correlation Id of "WeatherForecast:Get() Request" to allow filtering call tree logs starting from  _requestManager.StartProcessing();
            using (_logger.BeginScope("WeatherForecast:Get() Request"))
            {
                _requestManager.StartProcessing();
            }

            throw  new Exception("Testing Exception Handler");
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
