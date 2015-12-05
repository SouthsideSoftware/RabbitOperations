using Autofac;
using Microsoft.Extensions.OptionsModel;
using SouthsideUtility.Core.DependencyInjection;

namespace RabbitOperations.Collector.Configuration
{
    public class RavenDbSettingsHelper
    {
        public static RavenDbSettings Instance => ServiceLocator.Container.Resolve<IOptions<RavenDbSettings>>().Value;
    }
}