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
            GlobalMessageHandlingInstructions = new List<MessageTypeHandling>();
            Id = 1;
            Environments = new List<EnvironmentConfiguration>();
            AutoStartQueuePolling = false;
            Environments = new List<EnvironmentConfiguration> { new EnvironmentConfiguration{
                MessageHandlingInstructions = new List<MessageTypeHandling>(),
                AuditQueue = "audit",
                ErrorQueue = "error",
                RabbitConnectionString = "amqp://localhost",
                PollingTimeoutMilliseconds = 5000,
                MaxMessagesPerRun = 0,
                EnvironmentName = "Default",
                EnvironmentId = "Default",
                AutoStartQueuePolling = false
              }};

        }
        public int Id { get; set; }

        public IList<MessageTypeHandling> GlobalMessageHandlingInstructions { get; set; } 

        public IList<EnvironmentConfiguration> Environments { get; set; }

        public bool AutoStartQueuePolling { get; set; }
    }
}
