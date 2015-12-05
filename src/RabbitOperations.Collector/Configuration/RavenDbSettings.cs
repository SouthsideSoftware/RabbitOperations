using Microsoft.Extensions.OptionsModel;
using SouthsideUtility.Core.DependencyInjection;
using Autofac;

namespace RabbitOperations.Collector.Configuration
{
    public class RavenDbSettings
    {
        public bool UseEmbedded { get; set; }
        public string DefaultTenant { get; set; }
        public ExternalRavenDbSettings ExternalRavenDb { get; set; }
        public EmbeddedRavenDbSettings EmbeddedRavenDb { get; set; }
    }
}