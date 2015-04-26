using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;

namespace RabbitOperations.Collector.MessageRetry.NServiceBus
{
    public class CreateRetryMessageFromOriginal : ICreateRetryMessagesFromOriginal
    {
        public void PrepareMessageForRetry(IRawMessage rawMessage)
        {
         
        }
    }
}