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
            BusTechnology = "NServiceBus";
        }
        public int Id { get; set; }
        public IList<string> BusinessKeys { get; set; }

        public string BusTechnology { get; set; }

        public DateTime TimeSent { get; set; }

        public DateTime ProcessingStarted { get; set; }

        public DateTime ProcessingEnded { get; set; }

        public string ErrorInfo { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public string Body { get; set; }
    }
}
