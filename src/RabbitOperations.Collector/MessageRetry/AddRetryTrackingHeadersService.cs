using System;
using RabbitOperations.Collector.MessageBusTechnologies.Common;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.MessageRetry
{
    public class AddRetryTrackingHeadersService : IAddRetryTrackingHeaders
    {
        public void AddTrackingHeaders(IRawMessage rawMessage, long retryId)
        {
            rawMessage.Headers.Remove(Headers.Retry);
            rawMessage.Headers.Add(Headers.Retry, retryId.ToString());
        }
    }
}