using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    public class QueuePoller : IQueuePoller
    {
        private readonly CancellationToken cancellationToken;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public QueuePoller(string queueName, CancellationToken cancellationToken)
        {
            Verify.RequireStringNotNullOrWhitespace(queueName, "queueName");
            Verify.RequireNotNull(cancellationToken, "cancellationToken");

            this.QueueName = queueName;
            this.cancellationToken = cancellationToken;

        }

        public void Dispose()
        {
            Console.WriteLine("Disposed");
        }

        public string QueueName { get; private set; }

        public void Poll()
        {
            logger.Info("Started queue poller for {0}", QueueName);
            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(500);
            }
            logger.Info("Shutting down queue poller for {0} because of cancellation request", QueueName);
        }

        public void HandleMessage(IRawMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
