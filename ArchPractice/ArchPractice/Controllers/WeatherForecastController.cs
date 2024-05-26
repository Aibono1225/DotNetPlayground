using ArchPractice.Model;
using ArchPractice.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArchPractice.Controllers
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

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<List<UserVo>> Get()
        {
            var userService = new UserService();
            var userList = await userService.Query();
            return userList;
        }
    }
}
