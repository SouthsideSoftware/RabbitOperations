using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitOperations.Domain.Configuration
{
    public class ConfigurationDocument
    {
        public ConfigurationDocument()
        {
            Id = 1;
            Applications = new List<ApplicationConfiguration>();
            AutoStartQueuePolling = false;
        }
        public int Id { get; set; }

        public IList<ApplicationConfiguration> Applications { get; set; }

        public bool AutoStartQueuePolling { get; set; }

        public int DatabaseSchemaVersion { get; set; }
    }
}
