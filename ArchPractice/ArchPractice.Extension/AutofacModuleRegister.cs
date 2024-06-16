using ArchPractice.IService;
using ArchPractice.Repository.Base;
using ArchPractice.Service;
using Autofac;
using System.Reflection;

namespace ArchPractice.Extension
{
    public class AutofacModuleRegister: Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var basePath = AppContext.BaseDirectory;

            var serviceDllFile = Path.Combine(basePath, "ArchPractice.Service.dll");
            var repositoryDllFile = Path.Combine(basePath, "ArchPractice.Repository.dll");

            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(BaseService<,>)).As(typeof(IBaseService<,>)).InstancePerDependency();

            // 获取Service.dll程序集服务，并注册
            var assemblysServices = Assembly.LoadFrom(serviceDllFile);
            builder.RegisterAssemblyTypes(assemblysServices)
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

            // 获取Registory.dll程序集服务，并注册
            var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository)
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();
        }
    }
}
