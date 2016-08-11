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
		IApplicationConfiguration ApplicationConfiguration { get; }
	    void Start();
	    void Stop();

        Guid Key { get; }

        /// <summary>
        /// Handle a message
        /// </summary>
        /// <param name="message"></param>
        void HandleMessage(IRawMessage message);
    }
}