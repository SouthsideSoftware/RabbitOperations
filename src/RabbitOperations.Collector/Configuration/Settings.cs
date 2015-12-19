using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Extensions.OptionsModel;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Domain.Configuration;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Configuration
{
    public class Settings : ISettings
    {
        private readonly IDocumentStore documentStore;
        private ConfigurationDocument configurationDocument;
        private AppSettings appSettings;

        public Settings(IDocumentStore documentStore, IOptions<AppSettings> appSettingsConfig)
        {
            Verify.RequireNotNull(documentStore, "documentStore");
            Verify.RequireNotNull(appSettingsConfig, "appSettingsConfig");

            configurationDocument = new ConfigurationDocument();
            appSettings = appSettingsConfig.Value;
            this.documentStore = documentStore;
            Load();
        }

        /// <summary>
        /// Returns true if the application should use views and other web client side components
        /// from development directories if present.
        /// </summary>
        public bool AllowDevelopmentMode => appSettings.AllowDevelopmntMode;

        public int DatabaseSchemaVersion
        {
            get { return configurationDocument.DatabaseSchemaVersion; }
            set { configurationDocument.DatabaseSchemaVersion = value; }
        }

        public bool AutoStartQueuePolling
        {
            get { return configurationDocument.AutoStartQueuePolling; }
            set { configurationDocument.AutoStartQueuePolling = value; }
        }

        public IList<MessageTypeHandling> GlobalMessageHandlingInstructions
        {
            get { return configurationDocument.GlobalMessageHandlingInstructions; }
            set { configurationDocument.GlobalMessageHandlingInstructions = value; }
        }

        public IList<ApplicationConfiguration> Applications
        {
            get { return configurationDocument.Applications; }
            set { configurationDocument.Applications = value; }
        }

        public MessageTypeHandling MessageTypeHandlingFor(string type)
        {
            Verify.RequireStringNotNullOrWhitespace(type, "type");

            return GlobalMessageHandlingInstructions.FirstOrDefault(x => x.MessageTypes.Contains(type));
        }

        [JsonIgnore]
        public IList<RabbitServer> RabbitServers
        {
            get
            {
                return Applications.Select(
                    x =>
                        new RabbitServer
                        {
                            Name = x.ApplicationName,
                            Url = $"http://{new Uri(x.RabbitConnectionString).Host}:{x.RabbitManagementPort}"
                        }).OrderBy(x => x.Name).ToList();
            }
        }

        public IList<string> Environments
        {
            get { return Applications.Select(x => x.Environment).Distinct().OrderBy(x => x).ToList(); }
        }

        public void Load()
        {
            using (var session = documentStore.OpenSessionForDefaultTenant())
            {
                configurationDocument = session.Load<ConfigurationDocument>(1);
                if (configurationDocument == null)
                {
                    configurationDocument = new ConfigurationDocument();
                    session.Store(configurationDocument);
                    session.SaveChanges();
                }
            }
        }

        public void Save()
        {
            using (var session = documentStore.OpenSessionForDefaultTenant())
            {
                session.Store(configurationDocument);
                session.SaveChanges();
            }
        }
    }
}