using Autofac;
using Microsoft.Extensions.OptionsModel;
using SouthsideUtility.Core.DependencyInjection;

namespace RabbitOperations.Collector.Configuration
{
    public class AppSettingsHelper
    {
        public static AppSettings Instance => ServiceLocator.Container.Resolve<IOptions<AppSettings>>().Value;
    }
}