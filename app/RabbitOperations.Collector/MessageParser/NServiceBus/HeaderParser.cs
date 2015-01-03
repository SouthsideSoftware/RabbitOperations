using System;
using System.Globalization;
using System.Linq;
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
        private const string ContentTypeHeader = "ContentType";
        private const string ProcessingStartedHeader = "ProcessingStarted";
        private const string ProcessingEndedHeader = "ProcessingEnded";
        private const string TimeOfFailureHeader = "TimeOfFailure";
        private const string ExceptionTypeHeader = "ExceptionInfo.ExceptionType";

        public void AddHeaderInformation(IRawMessage rawMessage, MessageDocument document)
        {
            Verify.RequireNotNull(rawMessage, "rawMessage");
            Verify.RequireNotNull(document, "document");

            CaptureHeaders(rawMessage, document);
            CaptureMessageTypes(document);
            CaptureSagaInfo(document);
            CaptureTimeSent(document);
            CaptureContentType(document);
            CaptureProcessingTime(document);
            CaptureIsError(document);
            if (document.IsError)
            {
                if (document.Headers.ContainsKey(TimeOfFailureHeader) && document.Headers.ContainsKey(TimeSentHeader))
                {
                    document.TotalTime = ToUniversalDateTime(document.Headers[TimeOfFailureHeader]) -
                                         ToUniversalDateTime(document.Headers[TimeSentHeader]);
                }
            }
            else
            {
                if (document.Headers.ContainsKey(ProcessingEndedHeader) && document.Headers.ContainsKey(TimeSentHeader))
                {
                    document.TotalTime = ToUniversalDateTime(document.Headers[ProcessingEndedHeader]) -
                                         ToUniversalDateTime(document.Headers[TimeSentHeader]);
                }
            }
        }

        private static void CaptureIsError(MessageDocument document)
        {
            document.IsError = document.Headers.ContainsKey(ExceptionTypeHeader);
        }

        private static void CaptureProcessingTime(MessageDocument document)
        {
            if (document.Headers.ContainsKey(ProcessingStartedHeader) &&
                document.Headers.ContainsKey(ProcessingEndedHeader))
            {
                document.ProcessingTime = ToUniversalDateTime(document.Headers[ProcessingEndedHeader]) -
                                          ToUniversalDateTime(document.Headers[ProcessingStartedHeader]);
            }
        }

        private static void CaptureContentType(MessageDocument document)
        {
            if (document.Headers.ContainsKey(ContentTypeHeader))
            {
                document.ContentType = document.Headers[ContentTypeHeader];
            }
        }

        private static void CaptureTimeSent(MessageDocument document)
        {
            if (document.Headers.ContainsKey(TimeSentHeader))
            {
                document.TimeSent =
                    ToUniversalDateTime(document.Headers[TimeSentHeader]);
            }
        }

        private static DateTime ToUniversalDateTime(string nServiceBusDateTime)
        {
            return DateTime.ParseExact(nServiceBusDateTime, DateTimeFormat, CultureInfo.InvariantCulture)
                .ToUniversalTime();
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