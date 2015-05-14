using System.Linq;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Domain;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.MessageBusTechnologies.NServiceBus
{
    public class HeaderParser : IHeaderParser
    {
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
                if (document.Headers.ContainsKey(Headers.TimeOfFailure) && document.Headers.ContainsKey(Headers.TimeSent))
                {
                    document.TotalTime = Helpers.ToUniversalDateTime(document.Headers[Headers.TimeOfFailure]) -
                                         Helpers.ToUniversalDateTime(document.Headers[Headers.TimeSent]);
                }
            }
            else
            {
                if (document.Headers.ContainsKey(Headers.ProcessingEnded) && document.Headers.ContainsKey(Headers.TimeSent))
                {
                    document.TotalTime = Helpers.ToUniversalDateTime(document.Headers[Headers.ProcessingEnded]) -
                                         Helpers.ToUniversalDateTime(document.Headers[Headers.TimeSent]);
                }
            }
        }

        private static void CaptureIsError(MessageDocument document)
        {
            document.IsError = document.Headers.ContainsKey(Headers.ExceptionType);
            if (document.IsError)
            {
                if (document.Headers.ContainsKey(MessageBusTechnologies.Common.Headers.Retry))
                {
                    document.AdditionalErrorStatus = AdditionalErrorStatus.IsRetry;
                }
                else
                {
                    document.AdditionalErrorStatus = AdditionalErrorStatus.Unresolved;
                }
            }
            else
            {
                if (document.Headers.ContainsKey(MessageBusTechnologies.Common.Headers.Retry))
                {
                    document.AdditionalErrorStatus = AdditionalErrorStatus.IsRetry;
                }
                else
                {
                    document.AdditionalErrorStatus = AdditionalErrorStatus.NotAnError;
                }
            }
        }

        private static void CaptureProcessingTime(MessageDocument document)
        {
            if (document.Headers.ContainsKey(Headers.ProcessingStarted) &&
                document.Headers.ContainsKey(Headers.ProcessingEnded))
            {
                document.ProcessingTime = Helpers.ToUniversalDateTime(document.Headers[Headers.ProcessingEnded]) -
                                          Helpers.ToUniversalDateTime(document.Headers[Headers.ProcessingStarted]);
            }
        }

        private static void CaptureContentType(MessageDocument document)
        {
            if (document.Headers.ContainsKey(Headers.ContentType))
            {
                document.ContentType = document.Headers[Headers.ContentType];
            }
        }

        private static void CaptureTimeSent(MessageDocument document)
        {
            if (document.Headers.ContainsKey(Headers.TimeSent))
            {
                document.TimeSent =
                    Helpers.ToUniversalDateTime(document.Headers[Headers.TimeSent]);
            }
        }

        private static void CaptureSagaInfo(MessageDocument document)
        {
            if (document.Headers.ContainsKey(Headers.SagaInfo))
            {
                var parts = document.Headers[Headers.SagaInfo].Split(':');
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
            if (document.Headers.ContainsKey(Headers.MessageType))
            {
                document.MessageTypes = document.Headers[Headers.MessageType].Split(';').Select(pt =>
                {
                    return new TypeName(pt);
                }).ToList();
            }
        }

        private static void CaptureHeaders(IRawMessage rawMessage, MessageDocument document)
        {
            foreach (var header in rawMessage.Headers)
            {
                document.Headers.Add(header.Key, header.Value);
            }
        }
    }
}