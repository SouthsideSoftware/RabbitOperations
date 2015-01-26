using System.Collections.Generic;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Configuration.Interfaces
{
    public interface ISettings
    {
        int EmbeddedRavenDBManagementPort { get; }
       
        IList<MessageTypeHandling> GlobalMessageHandlingInstructions { get; set; }

        IList<EnvironmentConfiguration> Environments { get; set; } 

        void Load();
        void Save();
        MessageTypeHandling MessageTypeHandlingFor(string type);
        bool EmbedRavenDB { get; }
        string DefaultRavenDBTenant { get; }

        int WebPort { get; }
    }
}
