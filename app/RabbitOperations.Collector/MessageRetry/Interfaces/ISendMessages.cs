using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitOperations.Collector.MessageParser.Interfaces;

namespace RabbitOperations.Collector.MessageRetry.Interfaces
{
    public interface ISendMessages
    {
        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="queueName"></param>
        /// <returns>Null on success or the text of an error message</returns>
        string Send(IRawMessage message, string queueName);
    }
}
