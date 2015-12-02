using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using SouthsideUtility.Core.DependencyInjection;

namespace RabbitOperations.Collector.Configuration
{
    public class RavenDbSettings
    {
        public static RavenDbSettings Instance { get { return ServiceLocator.ServiceProvider.Resolve<IOptions<RavenDbSettings>>().Value} }
        public bool UseEmbedded { get; set; }
        public string DefaultTenant { get; set; }
        public ExternalRavenDbSettings ExternalRavenDb { get; set; }
        public EmbeddedRavenDbSettings EmbeddedRavenDb { get; set; }
    }
}
