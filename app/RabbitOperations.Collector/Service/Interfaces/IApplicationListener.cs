using System;
using System.Security.Cryptography.X509Certificates;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Domain;
using RabbitOperations.Domain.Configuration;
using Topshelf.Builders;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IApplicationListener
    {
		IApplicationConfiguration Application { get; }
	    void Start();
	    void Stop();
        /// <summary>
        /// Gets the queue name for this poller
        /// </summary>
        IQueueSettings QueueSettings { get; }

        Guid Key { get; }

        /// <summary>
        /// Start polling the indicated queue
        /// </summary>
        void Poll();
        /// <summary>
        /// Handle a message
        /// </summary>
        /// <param name="message"></param>
        void HandleMessage(IRawMessage message);
    }
}