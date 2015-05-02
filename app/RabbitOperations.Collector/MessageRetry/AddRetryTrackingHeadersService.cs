using System;
using NLog;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;

namespace RabbitOperations.Collector.MessageRetry
{
    public class AddRetryTrackingHeadersService : IAddRetryTrackingHeaders
    {
        public static string RetryHeader = "RabbitOperations.RetryId";

        public void AddTrackingHeaders(IRawMessage rawMessage, long retryId)
        {
            rawMessage.Headers.Remove(RetryHeader);
            rawMessage.Headers.Add(RetryHeader, retryId.ToString());
        }
    }
}