using System;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;

namespace RabbitOperations.Collector.MessageRetry
{
    public class SendMessagesService : ISendMessages
    {
        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="queueName"></param>
        /// <returns>Null on success or the text of an error message</returns>
        public string Send(IRawMessage message, string queueName)
        {
            return "Not implemented";
        }
    }
}