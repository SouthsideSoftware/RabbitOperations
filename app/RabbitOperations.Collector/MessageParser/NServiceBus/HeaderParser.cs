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
        private const string MessageType = "EnclosedMessageTypes";
        private const string NservicebusHeaderPrefix = "NServiceBus";

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
            if (document.Headers.ContainsKey(MessageType))
            {
                document.MessageTypes = document.Headers[MessageType].Split(';').Select(pt => pt.Trim()).ToList();
            }
        }
    }
}
