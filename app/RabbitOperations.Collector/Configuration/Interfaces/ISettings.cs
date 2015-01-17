using System.Collections.Generic;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Configuration.Interfaces
{
    public interface ISettings
    {
        int EmbeddedRavenDBManagementPort { get; }
        string AuditQueue { get; set; }
        string ErrorQueue { get; set; }

        int PollingTimeout { get; set; }

        string RabbitConnectionString { get; }

        IList<MessageTypeHandling> MessageHandlingInstructions { get; set; }

        void Load();
        void Save();
        MessageTypeHandling MessageTypeHandlingFor(string type);
        int MaxMessagesPerRun { get; set; }
        bool EmbedRavenDB { get; }
        string DefaultRavenDBTenant { get; }
    }
}
