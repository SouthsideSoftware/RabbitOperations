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
using RabbitOperations.Collector.RavenDb.Interfaces;
using Raven.Client;
using React.AspNet;
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
            services.AddReact();
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
            IOptions<AppSettings> appSettingsConfig, IOptions<RavenDbSettings> ravenDbSettingsConfig, ISchemaUpdater schemaUpdater)
        {
            var ravenSettings = ravenDbSettingsConfig.Value;
            var appSettings = appSettingsConfig.Value;
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Trace()
                .WriteTo.RavenDB(docStore, defaultDatabase:ravenSettings.DefaultTenant, expiration:appSettings.LogInRavenDbExpirationTimeSpan, errorExpiration:appSettings.LogErrorInRavenDbExpirationTimeSpan)
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

            // Initialise ReactJS.NET. Must be before static files.
            app.UseReact(config =>
            {
                // If you want to use server-side rendering of React components,
                // add all the necessary JavaScript files here. This includes
                // your components as well as all of their dependencies.
                // See http://reactjs.net/ for more information. Example:
                //config
                //    .AddScript("~/Scripts/First.jsx")
                //    .AddScript("~/Scripts/Second.jsx");

                // If you use an external build too (for example, Babel, Webpack,
                // Browserify or Gulp), you can improve performance by disabling
                // ReactJS.NET's version of Babel and loading the pre-transpiled
                // scripts. Example:
                //config
                //    .SetLoadBabel(false)
                //    .AddScriptWithoutTransform("~/Scripts/bundle.server.js");
            });

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