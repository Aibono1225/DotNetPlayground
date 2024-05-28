using ArchPractice.IService;
using ArchPractice.Model;
using ArchPractice.Service;
using AutoMapper;
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
        private readonly IBaseService<Role, RoleVo> _baseService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, 
            IBaseService<Role, RoleVo> baseService)
        {
            _logger = logger;
            _baseService = baseService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<object> Get()
        {
            //var userService = new UserService();
            //var userList = await userService.Query();
            //return userList;

            //var roleService = new BaseService<Role, RoleVo>(_mapper);
            //var roleList = await roleService.Query();

            var roleList = await _baseService.Query();
            return roleList;
        }
    }
}
