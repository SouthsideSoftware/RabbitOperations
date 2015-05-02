using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using RabbitOperations.Collector.MessageParser.Interfaces;

namespace RabbitOperations.Collector.MessageRetry.Interfaces
{
    public interface ICreateBasicProperties
    {
        //todo: implement for NServiceBus
        IBasicProperties Create(IRawMessage rawMessage);
    }
}
