using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IStoreMessages
    {
        long Store(IRawMessage message, IQueueSettings queueSettings);
    }
}
