using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpWebAPI.Controllers
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
    public IEnumerable<WeatherForecast> Get()
    {
      var rng = new Random();
      return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      {
        Date = DateTime.Now.AddDays(index),
        TemperatureC = rng.Next(-20, 55),
        Summary = Summaries[rng.Next(Summaries.Length)]
      })
           .ToArray();
    }

    #region snippet_GetByCity
    [HttpGet("{city}")]
    public WeatherForecast Get(string city)
    {
      if (!string.Equals(city?.TrimEnd(), "Redmond", StringComparison.OrdinalIgnoreCase))
      {
        throw new ArgumentException(
            $"We don't offer a weather forecast for {city}.", nameof(city));
      }

      //return GetWeather().First();
      return Get().First();
    }
    #endregion


  }
}
