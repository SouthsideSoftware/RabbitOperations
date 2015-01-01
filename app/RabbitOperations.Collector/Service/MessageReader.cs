using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Utility.DesignByContract;
using RabbitOperations.Utility.TestableSystem.Interfaces;

namespace RabbitOperations.Collector.Service
{
    public class MessageReader : IMessageReader
    {
        private readonly ICancellationTokenSource cancellationTokenSource;
        private readonly ISettings settings;
        private CancellationToken cancellationToken;
        private IList<Task> queuePollers = new List<Task>();

        public MessageReader(ICancellationTokenSource cancellationTokenSource, ISettings settings)
        {
            Verify.RequireNotNull(cancellationTokenSource, "cancellationTokenSource");
            Verify.RequireNotNull(settings, "settings");

            this.cancellationTokenSource = cancellationTokenSource;
            this.settings = settings;

            cancellationToken = cancellationTokenSource.Token;
        }

        public void Start()
        {
            //queuePollers.Add(Task.Factory.StartNew(() =>
            //{
            //    StartPollingQueue(settings.AuditQueue);
            //}, TaskCreationOptions.LongRunning, cancellationToken, TaskScheduler.Default));
            throw new NotImplementedException();
        }

        private void StartPollingQueue(string queueName)
        {
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}