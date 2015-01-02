using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client.Events;
using RabbitOperations.Collector.MessageParser.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.MessageParser
{
    public class RawMessage : IRawMessage
    {
        public RawMessage()
        {
            Headers = new Dictionary<string, string>();
        }

        public RawMessage(BasicDeliverEventArgs deliveryEventArgs) : this()
        {
            Verify.RequireNotNull(deliveryEventArgs, "deliveryEventArgs");

            var rawBody = deliveryEventArgs.Body;
            Body = Encoding.UTF8.GetString(rawBody);
            foreach (var header in deliveryEventArgs.BasicProperties.Headers)
            {
                if (header.Value != null)
                {
                    Headers.Add(header.Key, Encoding.UTF8.GetString((byte[]) header.Value));
                }
            }
        }

        public RawMessage(Dictionary<string, string> headers, string body)
        {
            Verify.RequireStringNotNullOrWhitespace(body, "body");
            Verify.RequireNotNull(headers, "headers");

            Headers = headers;
            Body = body;
        }
        public IDictionary<string, string> Headers { get; protected set; }

        public string Body { get; protected set; }
    }
}