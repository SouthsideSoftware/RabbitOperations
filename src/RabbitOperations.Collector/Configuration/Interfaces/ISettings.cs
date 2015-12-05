using System.Collections.Generic;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Configuration.Interfaces
{
    public interface ISettings
    {  
        IList<MessageTypeHandling> GlobalMessageHandlingInstructions { get; set; }

        IList<ApplicationConfiguration> Applications { get; set; } 

        void Load();
        void Save();
        MessageTypeHandling MessageTypeHandlingFor(string type);

        bool AutoStartQueuePolling { get; set; }

        /// <summary>
        /// Returns true if the application should use views and other web client side components
        /// from development directories if present.
        /// </summary>
        bool AllowDevelopmentMode { get; }

        int DatabaseSchemaVersion { get; set; }
    }
}
