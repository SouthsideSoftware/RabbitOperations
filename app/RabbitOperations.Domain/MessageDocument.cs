using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitOperations.Domain
{
    public class MessageDocument
    {
        public MessageDocument()
        {
            Headers = new Dictionary<string, string>();
            MessageTypes = new List<string>();
            BusTechnology = "NServiceBus";
        }
        public int Id { get; set; }
        public IList<string> BusinessKeys { get; set; }
        public string ContentType { get; set; }
        public string BusTechnology { get; set; }

        public DateTime TimeSent { get; set; }

        public TimeSpan EnquedTime { get; set; }

        public TimeSpan ProcessingTime { get; set; }

        public IList<string> MessageTypes { get; set; } 

        public IList<string> Keywords { get; set; }

        public SagaInfo SagaInfo { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public string Body { get; set; }
    }
}
