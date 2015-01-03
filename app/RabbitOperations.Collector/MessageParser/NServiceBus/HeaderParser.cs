using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Domain;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.MessageParser.NServiceBus
{
    public class HeaderParser : IHeaderParser
    {
        private const string MessageTypeHeader = "EnclosedMessageTypes";
        private const string NservicebusHeaderPrefix = "NServiceBus";
        private const string SagaInfoHeader = "InvokedSagas";

        public void AddHeaderInformation(IRawMessage rawMessage, MessageDocument document)
        {
            Verify.RequireNotNull(rawMessage, "rawMessage");
            Verify.RequireNotNull(document, "document");

            foreach (var header in rawMessage.Headers)
            {
                if (header.Key.StartsWith(NservicebusHeaderPrefix))
                {
                    document.Headers.Add(header.Key.Substring(NservicebusHeaderPrefix.Length + 1), header.Value);
                }
            }
            if (document.Headers.ContainsKey(MessageTypeHeader))
            {
                document.MessageTypes = document.Headers[MessageTypeHeader].Split(';').Select(pt => pt.Trim()).ToList();
            }
            if (document.Headers.ContainsKey(SagaInfoHeader))
            {
                var parts = document.Headers[SagaInfoHeader].Split(':');
                if (parts.Length > 1)
                {
                    document.SagaInfo = new SagaInfo
                    {
                        Class = parts[0],
                        Key = parts[1]
                    };
                }
            }
        }
    }
}
