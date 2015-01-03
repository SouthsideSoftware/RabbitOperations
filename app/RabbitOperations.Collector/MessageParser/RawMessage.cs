using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitOperations.Collector.MessageParser.Interfaces;
using SouthsideUtility.Core.DesignByContract;
using Newtonsoft.Json.Linq;

namespace RabbitOperations.Collector.MessageParser
{
    public class RawMessage : IRawMessage
    {
        private static readonly UTF8Encoding utf8EncodingNoByteOrderMark = new UTF8Encoding(true);
        public RawMessage()
        {
            Headers = new Dictionary<string, string>();
        }

        public RawMessage(BasicDeliverEventArgs deliveryEventArgs) : this()
        {
            Verify.RequireNotNull(deliveryEventArgs, "deliveryEventArgs");

            var rawBody = deliveryEventArgs.Body;
            using (var reader = new StreamReader(new MemoryStream(rawBody)))
            {
                Body = reader.ReadToEnd();
            }
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
        [JsonProperty]
        public IDictionary<string, string> Headers { get; protected set; }

        [JsonProperty]
        public string Body { get; protected set; }
    }
}