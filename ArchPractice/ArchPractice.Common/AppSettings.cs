using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchPractice.Common
{
    /// <summary>
    /// appsettings.json操作类
    /// </summary>
    public class AppSettings
    {
        public static IConfiguration Configuration { get; set; }
        static string contentPath {  get; set; }

        public AppSettings(string contentPath)
        {
            //string path = "appsettings.json";

            //根据环境变量，获取对应的配置文件
            string path = $"appsettings" +
                $".{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

            Configuration = new ConfigurationBuilder()
                .SetBasePath(contentPath)
                .Add(new JsonConfigurationSource
                {
                    Path = path,
                    Optional = false,
                    ReloadOnChange = true
                }) //直接读取目录里的json文件，而不是bin文件夹下的，所以不用修改复制属性
                .Build();
        }

        public AppSettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static string app(params string[] sections)
        {
            try
            {
                if (sections.Any())
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception ex)
            {

            }

            return "";
        }

        /// <summary>
        /// 递归获取配置信息数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<T> app<T>(params string[] sections)
        {
            List<T> list = new List<T>();
            // 引用Microsoft.Extensions.Configuration.Binder包
            Configuration.Bind(string.Join(":", sections), list);
            return list;
        }

        /// <summary>
        /// 根据路径获取配置信息 configuration["App:Name"];
        /// </summary>
        /// <param name="sectionsPath"></param>
        /// <returns></returns>
        public static string GetValue(string sectionsPath)
        {
            try
            {
                return Configuration[sectionsPath];
            }
            catch (Exception ex)
            {

            }

            return "";
        }
    }
}
