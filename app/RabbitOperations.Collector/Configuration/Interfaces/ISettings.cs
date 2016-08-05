using System.Collections.Generic;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Configuration.Interfaces
{
    public interface ISettings
    {
        int EmbeddedRavenDBManagementPort { get; }
       

        IList<IApplicationConfiguration> Applications { get; set; } 

        void Load();
        void Save();
        bool EmbedRavenDB { get; }
        string DefaultRavenDBTenant { get; }

		bool SuppressPolling { get; }

        int WebPort { get; }
        bool AutoStartQueuePolling { get; set; }

        /// <summary>
        /// Returns true if the application should use views and other web client side components
        /// from development directories if present.
        /// </summary>
        bool AllowDevelopmentMode { get; }

        int DatabaseSchemaVersion { get; set; }
        string RavenDBConnectionString { get; }
    }
}
