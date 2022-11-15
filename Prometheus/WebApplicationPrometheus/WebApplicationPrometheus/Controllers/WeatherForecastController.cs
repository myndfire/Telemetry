using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace WebApplicationPrometheus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static string _applicationId = "WeatherForecastController:localhost";
        private static readonly string[] Summaries = new[]
        {
            "Cold",  "Pleasant", "Hot"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherForecastMetricsCollector _weatherForecastMetricsCollector;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecastMetricsCollector weatherForecastMetricsCollector)
        {
            _logger = logger;
            _weatherForecastMetricsCollector = weatherForecastMetricsCollector;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            var summarytype = Summaries[rng.Next(Summaries.Length)];
            int temp=0;
            switch (summarytype)
            {
                case "Cold":
                    temp = rng.Next(-15, 20);
                    break;
                case "Pleasant":
                    temp = rng.Next(21, 40);
                    break;
                case "Hot":
                    temp = rng.Next(41, 60);
                    break;
            }

            _weatherForecastMetricsCollector.TemperatureSummaryTypeCounterMetricIncrement(_applicationId,summarytype);
            _weatherForecastMetricsCollector.TemperatureGaugeMetricSetvalue(_applicationId, summarytype, temp);
            _weatherForecastMetricsCollector.TemperatureHistogramMetricSetSummaryTypeValue(_applicationId,summarytype, temp);
            
            return new List<WeatherForecast>
            {
                new WeatherForecast
                {
                    Date = DateTime.Now,
                    Summary = summarytype,
                    TemperatureC = temp
                }
            };
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();
        }
    }
}
