﻿using Autofac;
using Microsoft.AspNetCore.Mvc;

namespace ArchPractice.Extensions
{
    public class AutofacPropertyModuleReg: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var controllerBaseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired();
        }
    }
}
