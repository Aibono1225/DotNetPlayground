using ArchPractice.Common;
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

        // 属性注册
        public IBaseService<Role, RoleVo> _roleServiceObj { get; set; }

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

            //var roleList = await _baseService.Query();

            var redisEnable = AppSettings.app(new string[] { "Redis", "Enable" });
            var redisConnectionString = AppSettings.GetValue("Redis:ConnectionString");
            Console.WriteLine($"Enable: {redisEnable}, ConnectionString: {redisConnectionString}");
            
            var roleList = await _roleServiceObj.Query();
            return roleList;
        }
    }
}
