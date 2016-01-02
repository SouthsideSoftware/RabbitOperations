using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.MessageBusTechnologies.NServiceBus
{
    public class CreateBasicPropertiesService : ICreateBasicProperties
    {
        private const byte persistentDelivery = 2;
        public IBasicProperties Create(IRawMessage rawMessage)
        {
            var basicProperties = new BasicProperties();
            basicProperties.ContentType = GetHeaderValueIfExists(rawMessage, "NServiceBus.ContentType");
            basicProperties.MessageId = GetHeaderValueIfExists(rawMessage, "NServiceBus.MessageId");
            basicProperties.CorrelationId = GetHeaderValueIfExists(rawMessage, "NServiceBus.CorrelationId");
            SetTypeIfHeaderExists(rawMessage, basicProperties);
            basicProperties.DeliveryMode = persistentDelivery;

            return basicProperties;
        }

        private void SetTypeIfHeaderExists(IRawMessage rawMessage, BasicProperties basicProperties)
        {
            string types = GetHeaderValueIfExists(rawMessage, "NServiceBus.EnclosedMessageTypes");
            if (types != null)
            {
                var typeList = types.Split(';').Select(pt => { return new TypeName(pt); }).ToList();
                if (typeList.Count > 0)
                {
                    basicProperties.Type = string.Format("{0}.{1}", typeList[0].Namespace, typeList[0].ClassName);
                }
            }
        }

        private string GetHeaderValueIfExists(IRawMessage rawMessage, string key)
        {
            string val = null;
            if (rawMessage.Headers.ContainsKey(key))
            {
                val = rawMessage.Headers[key];
            }

            return val;
        }
    }
}