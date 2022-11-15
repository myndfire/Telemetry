using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DemoOpentelemetry.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

		private readonly Serilog.ILogger _logger;

		public WeatherForecastController(Serilog.ILogger logger)
		{
			_logger = logger;
		}

		[HttpGet(Name = "GetWeatherForecast")]
		public IEnumerable<WeatherForecast> Get()
		{
			DateTime startDate = new(2022,1, 1, 0, 0, 0, DateTimeKind.Utc);
			Activity.Current?.AddEvent(new ActivityEvent("Generating random weather forecasts"));
			_logger.Information("Generating random weather forecasts for {forecastdate}", startDate.Date);
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = startDate.AddDays(index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();
		}
	}
}