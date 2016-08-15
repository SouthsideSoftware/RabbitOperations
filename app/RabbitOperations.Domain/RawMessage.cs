using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Domain
{
    public class RawMessage : IRawMessage
    {
        private static readonly UTF8Encoding utf8EncodingNoByteOrderMark = new UTF8Encoding(true);
		public long Id { get; set; }
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
            if (deliveryEventArgs.BasicProperties.Headers != null)
            {
                foreach (var header in deliveryEventArgs.BasicProperties.Headers)
                {
                    if (header.Value != null)
                    {
                        Headers.Add(header.Key, Encoding.UTF8.GetString((byte[]) header.Value));
                    }
                }
            }
        }

        public RawMessage(MessageDocument messageDocument) 
        {
            Verify.RequireNotNull(messageDocument, "messageDocument");
            CreateFromHeadersAndBody(messageDocument.Headers, messageDocument.Body);
        }

        private void CreateFromHeadersAndBody(IDictionary<string, string> headers, string body)
        {
            Verify.RequireNotNull(body, "body");
            Verify.RequireNotNull(headers, "headers");

            Headers = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                Headers.Add(header.Key, header.Value);
            }
            Body = body;
        }

        [JsonProperty]
        public IDictionary<string, string> Headers { get; protected set; }

        [JsonProperty]
        public string Body { get; protected set; }

        public Tuple<byte[], Dictionary<string, object>> GetEelementsForRabbitPublish()
        {
            var headers = new Dictionary<string, object>();
            foreach (var header in Headers)
            {
                headers.Add(header.Key, System.Text.Encoding.UTF8.GetBytes(header.Value));
            }
            return new Tuple<byte[], Dictionary<string, object>>(GetBytesFromString(Body),
                headers);
        }

        private byte [] GetBytesFromString(string s)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(true));
                writer.Write(s);
                writer.Flush();
                return stream.ToArray();
            }
        }
    }
}