using System.Linq;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.MessageRetry.NServiceBus
{
    public class CreateRetryMessageFromOriginalService : ICreateRetryMessagesFromOriginal
    {
        public void PrepareMessageForRetry(IRawMessage rawMessage)
        {
            Verify.RequireNotNull(rawMessage, "rawMessage");

            RemoveDiagnosticHeaders(rawMessage);
            RemoveExceptionHeaders(rawMessage);
            RemoveTimeoutHeaders(rawMessage);
            RemoveRetryHeaders(rawMessage);
            RemoveProcessingHeaders(rawMessage);
        }

        private static void RemoveDiagnosticHeaders(IRawMessage rawMessage)
        {
            var keysToRemove =
                rawMessage.Headers.Where(x => x.Key.StartsWith("$.diagnostics")).Select(x => x.Key).ToList();
            foreach (var key in keysToRemove)
            {
                rawMessage.Headers.Remove(key);
            }
        }

        private static void RemoveExceptionHeaders(IRawMessage rawMessage)
        {
            var keysToRemove =
                rawMessage.Headers.Where(x => x.Key.StartsWith("NServiceBus.ExceptionInfo")).Select(x => x.Key).ToList();
            foreach (var key in keysToRemove)
            {
                rawMessage.Headers.Remove(key);
            }
        }

        private static void RemoveTimeoutHeaders(IRawMessage rawMessage)
        {
            var keysToRemove =
                rawMessage.Headers.Where(x => x.Key.StartsWith("NServiceBus.Timeout")).Select(x => x.Key).ToList();
            foreach (var key in keysToRemove)
            {
                rawMessage.Headers.Remove(key);
            }
        }

        private static void RemoveRetryHeaders(IRawMessage rawMessage)
        {
            var keysToRemove =
                rawMessage.Headers.Where(x => x.Key.StartsWith("NServiceBus.Retries")).Select(x => x.Key).ToList();
            foreach (var key in keysToRemove)
            {
                rawMessage.Headers.Remove(key);
            }

            keysToRemove =
                rawMessage.Headers.Select(x => x.Key)
                    .Intersect(new[]
                    {
                        "NServiceBus.FLRetries",
                        "NServiceBus.FailedQ",
                        "NServiceBus.TimeOfFailure"
                    })
                    .ToList();
            foreach (var key in keysToRemove)
            {
                rawMessage.Headers.Remove(key);
            }
        }

        private static void RemoveProcessingHeaders(IRawMessage rawMessage)
        {
            var keysToRemove =
                rawMessage.Headers.Select(x => x.Key)
                    .Intersect(new[]
                    {
                        "NServiceBus.ProcessingStarted",
                        "NServiceBus.ProcessingEnded",
                        "NServiceBus.ProcessingEndpoint",
                        "NServiceBus.ProcessingMachine"
                    }).ToList();

            foreach (var key in keysToRemove)
            {
                rawMessage.Headers.Remove(key);
            }
        }
    }
}