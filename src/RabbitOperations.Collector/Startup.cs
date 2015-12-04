using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNet.Mvc.Internal;
using Microsoft.Extensions.PlatformAbstractions;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.RavenDb;
using Raven.Client;
using Serilog;
using Serilog.Events;
using SouthsideUtility.Core.DependencyInjection;

namespace RabbitOperations.Collector
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<RavenDbSettings>(Configuration.GetSection("RavenDbSettings"));
            services.AddMvc();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<RavenDbModule>();
            containerBuilder.RegisterModule<ConfigurationModule>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            ServiceLocator.ServiceProvider = container.Resolve<IServiceProvider>();
            return ServiceLocator.ServiceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDocumentStore docStore, IApplicationEnvironment applicationEnvironment)
        {
            var logLevel = Configuration["Logging:LogLevel:Default"];
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Trace()
                .WriteTo.RavenDB(docStore)
                .WriteTo.ColoredConsole()
                .MinimumLevel.Is((LogEventLevel)Enum.Parse(typeof(LogEventLevel), logLevel))
                .CreateLogger();

            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var logger = loggerFactory.CreateLogger("Application");
            logger.LogInformation($"Minimum log level: {logLevel}");
            logger.LogVerbose($"AppBase {applicationEnvironment.ApplicationBasePath}");
            logger.LogVerbose($"Framework {applicationEnvironment.RuntimeFramework}");
            logger.LogVerbose($"AppDomain {AppDomain.CurrentDomain.BaseDirectory}");
            logger.LogVerbose($"Current Directory {Directory.GetCurrentDirectory()}");

        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
