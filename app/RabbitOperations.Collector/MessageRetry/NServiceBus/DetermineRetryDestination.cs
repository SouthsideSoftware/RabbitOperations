using System;
using System.Linq;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.MessageRetry.NServiceBus
{
    public class DetermineRetryDestination : IDetermineRetryDestination
    {
        public string GetRetryDestination(IRawMessage rawMessage, string userSuppliedRetryDestination)
        {
            Verify.RequireNotNull(rawMessage, "rawMessage");

            if (!string.IsNullOrWhiteSpace(userSuppliedRetryDestination))
            {
                return userSuppliedRetryDestination;
            }

            if (rawMessage.Headers.ContainsKey("NServiceBus.FailedQ"))
            {
                var failedQ = rawMessage.Headers["NServiceBus.FailedQ"];
                return failedQ.Split('@')[0];
            }
            return null;
        }
    }
}