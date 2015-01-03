using System;
using System.Collections.Generic;
using System.Globalization;
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
        private const string TimeSentHeader = "TimeSent";
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss:ffffff Z";

        public void AddHeaderInformation(IRawMessage rawMessage, MessageDocument document)
        {
            Verify.RequireNotNull(rawMessage, "rawMessage");
            Verify.RequireNotNull(document, "document");

            CaptureHeaders(rawMessage, document);
            CaptureMessageTypes(document);
            CaptureSagaInfo(document);

            if (document.Headers.ContainsKey(TimeSentHeader))
            {
                document.TimeSent =
                    DateTime.ParseExact(document.Headers[TimeSentHeader], DateTimeFormat, CultureInfo.InvariantCulture)
                        .ToUniversalTime();
            }

        }

        private static void CaptureSagaInfo(MessageDocument document)
        {
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

        private static void CaptureMessageTypes(MessageDocument document)
        {
            if (document.Headers.ContainsKey(MessageTypeHeader))
            {
                document.MessageTypes = document.Headers[MessageTypeHeader].Split(';').Select(pt => pt.Trim()).ToList();
            }
        }

        private static void CaptureHeaders(IRawMessage rawMessage, MessageDocument document)
        {
            foreach (var header in rawMessage.Headers)
            {
                if (header.Key.StartsWith(NservicebusHeaderPrefix))
                {
                    document.Headers.Add(header.Key.Substring(NservicebusHeaderPrefix.Length + 1), header.Value);
                }
            }
        }
    }
}
