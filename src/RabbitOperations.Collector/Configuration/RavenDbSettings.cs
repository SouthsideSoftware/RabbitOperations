using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

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
