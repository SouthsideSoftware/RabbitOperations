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
        }
        public int Id { get; set; }
        public IList<MessageTypeHandling> MessageHandlingInstructions { get; set; }
        public string AuditQueue { get; set; }
        public string ErrorQueue { get; set; }
    }
}
