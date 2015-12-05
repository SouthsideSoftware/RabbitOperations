using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.RavenDb;
using RabbitOperations.Collector.RavenDB.Interfaces;
using Raven.Client;
using Serilog;
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
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddMvc();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<RavenDbModule>();
            containerBuilder.RegisterModule<ConfigurationModule>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            ServiceLocator.Container = container;
            return container.Resolve<IServiceProvider>();
        }

        // Setup the HTTP Pipeline and startup background services
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IDocumentStore docStore, IApplicationEnvironment applicationEnvironment,
            IOptions<AppSettings> appSettingsConfig, ISchemaUpdater schemaUpdater)
        {
            var appSettings = appSettingsConfig.Value;
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Trace()
                .WriteTo.RavenDB(docStore)
                .WriteTo.ColoredConsole()
                .MinimumLevel.Is(appSettings.LogLevel)
                .CreateLogger();

            loggerFactory.AddSerilog();
            loggerFactory.MinimumLevel = appSettings.MicrosoftLogLevel;

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            StartupBackgroundServices(applicationEnvironment, appSettings, schemaUpdater);

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"); });
        }

        private void StartupBackgroundServices(IApplicationEnvironment applicationEnvironment, AppSettings appSettings, ISchemaUpdater schemaUpdater)
        {
            Log.Logger.Information("Application Startup Begins...");

            Log.Logger.Information("Minimum log levels -- Serilog:{SerilogLevel} Microsoft:{MicrosoftLogLevel}",
                appSettings.LogLevel, appSettings.MicrosoftLogLevel);
            Log.Logger.Information(
                "Application Base Path:{ApplicationBasePath} AppDomain Base Directory:{AppDomainBaseDirectory} Working Directory:{WorkingDirectory} Framework:{Framework}",
                applicationEnvironment.ApplicationBasePath, AppDomain.CurrentDomain.BaseDirectory,
                Directory.GetCurrentDirectory(), applicationEnvironment.RuntimeFramework);

            UpdateSchemaIfNeeded(schemaUpdater);
        }

        private void UpdateSchemaIfNeeded(ISchemaUpdater schemaUpdater)
        {
            schemaUpdater.UpdateSchema();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}