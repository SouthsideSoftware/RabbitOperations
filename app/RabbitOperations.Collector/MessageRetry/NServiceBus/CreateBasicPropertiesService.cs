using System;
using RabbitMQ.Client;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;

namespace RabbitOperations.Collector.MessageRetry.NServiceBus
{
    public class CreateBasicPropertiesService : ICreateBasicProperties
    {
        public IBasicProperties Create(IRawMessage rawMessage)
        {
            //todo: get values from NServiceBus headers
            //messageId, correlationId, type, contentType
            //set DeliveryMode = 2 (persistent)
            throw new NotImplementedException();
        }
    }
}