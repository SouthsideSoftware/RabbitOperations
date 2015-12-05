using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Serilog.Events;
using SouthsideUtility.Core.DependencyInjection;

namespace RabbitOperations.Collector.Configuration
{
    public class AppSettings
    {
        public static AppSettings Instance
        {
            get { return ServiceLocator.ServiceProvider.Resolve<IOptions<AppSettings>>().Value; }
        }

        public LogEventLevel LogLevel { get; set; }

        public bool AllowDevelopmntMode { get; set; }

        public LogLevel MicrosoftLogLevel
        {
            get
            {
                switch (LogLevel)
                {
                    case LogEventLevel.Verbose:
                        return Microsoft.Extensions.Logging.LogLevel.Debug;
                    case LogEventLevel.Debug:
                        return Microsoft.Extensions.Logging.LogLevel.Verbose;
                    case LogEventLevel.Information:
                        return Microsoft.Extensions.Logging.LogLevel.Information;
                    case LogEventLevel.Warning:
                        return Microsoft.Extensions.Logging.LogLevel.Warning;
                    case LogEventLevel.Error:
                        return Microsoft.Extensions.Logging.LogLevel.Error;
                    case LogEventLevel.Fatal:
                        return Microsoft.Extensions.Logging.LogLevel.Critical;
                }

                return Microsoft.Extensions.Logging.LogLevel.Information;
            }
        }
    }
}