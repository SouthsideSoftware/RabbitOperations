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
            MessageHandlingInstructions = new List<MessageTypeHandling>();
            Id = 1;
            AuditQueue = "audit";
            ErrorQueue = "error";
            RabbitConnectionString = "amqp://localhost";
            PollingTimeout = 5000;
        }
        public int Id { get; set; }
        public IList<MessageTypeHandling> MessageHandlingInstructions { get; set; }
        public string AuditQueue { get; set; }
        public string ErrorQueue { get; set; }
        public int PollingTimeout { get; set; }

        public string RabbitConnectionString { get; set; }
    }
}
