using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore.Web.Commons.Configs;
using Autofac.Configuration;
using NetCore.Web.Filter;
using log4net.Repository;
using log4net;
using log4net.Config;
using System.IO;
using NetCore.Web.Commons;

namespace NetCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //log4net
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(Directory.GetCurrentDirectory()+@"/Configs/log4net.config"));
            LogRepository.Repository = logRepository;
        }

        public IConfiguration Configuration { get; }
        public IContainer Container { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            /* 配置文件注入方式 */
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddOptions();

            //Area
            var baseController = typeof(NetCore.Web.Controllers.BaseController);
            var controllerAssembly = baseController.GetTypeInfo().Assembly;
            services.AddMvc(options =>
            {
                options.Filters.Add<AppGlobalExceptionFilter>();
                options.Filters.Add<AppActionFilter>();
            }).ConfigureApplicationPartManager(m =>
            {
                var feature = new ControllerFeature();
                m.ApplicationParts.Add(new AssemblyPart(controllerAssembly));
                m.PopulateFeature(feature);
                services.AddSingleton(feature.Controllers.Select(t => t.AsType()).ToArray());
            });

            //autofac
            var builder = new ContainerBuilder();
            builder.Populate(services);
            var module = new ConfigurationModule(Configuration);
            builder.RegisterModule(module);
            this.Container = builder.Build();

            return new AutofacServiceProvider(this.Container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "{area:exists}/",
                   template: "{area:exists}/{controller=Default}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Default}/{action=Index}/{id?}");
            });

            //autofac
            appLifetime.ApplicationStopped.Register(() => this.Container.Dispose());
        }
    }
}
