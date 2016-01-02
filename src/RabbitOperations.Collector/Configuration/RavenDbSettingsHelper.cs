using Autofac;
using Microsoft.Extensions.OptionsModel;
using SouthsideUtility.Core.DependencyInjection;

namespace RabbitOperations.Collector.Configuration
{
    public class RavenDbSettingsHelper
    {
        public static RavenDbSettings TestInstance;

        public static RavenDbSettings Instance
        {
            get
            {
                if (TestInstance != null)
                {
                    return TestInstance;
                }

                return ServiceLocator.Container.Resolve<IOptions<RavenDbSettings>>().Value;
            }
        } 
    }
}
