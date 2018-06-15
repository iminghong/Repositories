using Autofac;
using NetCore.Web.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Web.Service
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(t => t.IsAssignableTo<IBaseService>())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
