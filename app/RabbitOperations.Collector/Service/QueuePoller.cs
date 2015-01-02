using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    public class QueuePoller : IQueuePoller
    {
        private readonly IRawMessage rawMessage;

        public QueuePoller(string queueName, IRawMessage rawMessage)
        {
            Verify.RequireStringNotNullOrWhitespace(queueName, "queueName");
            Verify.RequireNotNull(rawMessage, "rawMessage");

            this.rawMessage = rawMessage;
            this.QueueName = queueName;

        }

        public void Dispose()
        {
            Console.WriteLine("Disposed");
        }

        public string QueueName { get; private set; }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void HandleMessage(IRawMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
