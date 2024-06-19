using ArchPractice.IService;
using ArchPractice.Repository.Base;
using ArchPractice.Service;
using Autofac;
using Autofac.Extras.DynamicProxy;
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

            var aopTypes = new List<Type>() { typeof(ServiceAOP) };
            builder.RegisterType<ServiceAOP>();

            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(BaseService<,>)).As(typeof(IBaseService<,>))
                .EnableInterfaceInterceptors()
                .InterceptedBy(aopTypes.ToArray())
                .InstancePerDependency();

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
                .PropertiesAutowired()
                .EnableInterfaceInterceptors()
                .InterceptedBy(aopTypes.ToArray());
        }
    }
}
