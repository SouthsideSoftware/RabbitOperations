using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.MessageRetry.Interfaces
{
    public interface ISendMessages
    {
        string Send(IRawMessage message, string queueName, bool replayToExchange, string applicationId, IBasicProperties basicProperties);
    }
}
