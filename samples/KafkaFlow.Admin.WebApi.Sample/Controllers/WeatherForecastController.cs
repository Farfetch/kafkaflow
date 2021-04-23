namespace KafkaFlow.Admin.WebApi.Sample.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Microsoft.AspNetCore.Mvc;
    using KafkaFlow.Admin.WebApi.Sample.Models;

    [ApiController]
    [Route("v1/WeatherForecast")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5)
                .Select(
                    index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    })
                .ToArray();
        }
    }
}
