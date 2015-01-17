using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Domain.Configuration;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Configuration
{
    public class Settings : ISettings
    {
        private ConfigurationDocument configurationDocument;
        private readonly IDocumentStore documentStore;

        public Settings(IDocumentStore documentStore)
        {
            Verify.RequireNotNull(documentStore, "documentStore");
            configurationDocument = new ConfigurationDocument();

            this.documentStore = documentStore;
            Load();
        }

        public static string StaticDefaultRavenDBTenant
        {
            get { return GetString("DefaultRavenDBTenant", "RabbitOperations"); }
        }

        public string DefaultRavenDBTenant
        {
            get { return StaticDefaultRavenDBTenant; }
        }

        public static bool StaticEmbedRavenDB
        {
            get { return GetBoolean("EmbedRavenDB", true); }
        }

        public static int StaticEmbeddedRavenDBManagementPort
        {
            get { return GetInt("EmbeddedRavenDBManagementPort", 8080).Value; }
        }

        public bool EmbedRavenDB
        {
            get { return StaticEmbedRavenDB; }
        }

        public int EmbeddedRavenDBManagementPort
        {
            get { return StaticEmbeddedRavenDBManagementPort; }
        }

        public string AuditQueue
        {
            get { return configurationDocument.AuditQueue; }
            set { configurationDocument.AuditQueue = value; }
        }

        public int PollingTimeout
        {
            get { return configurationDocument.PollingTimeout; }
            set { configurationDocument.PollingTimeout = value; }
        }

        public int MaxMessagesPerRun
        {
            get { return configurationDocument.MaxMessagesPerRun; }
            set { configurationDocument.MaxMessagesPerRun = value; }
        }

        public string RabbitConnectionString
        {
            get { return configurationDocument.RabbitConnectionString; }
            set { configurationDocument.RabbitConnectionString = value; }
        }

        public IList<MessageTypeHandling> MessageHandlingInstructions
        {
            get { return configurationDocument.MessageHandlingInstructions; }
            set { configurationDocument.MessageHandlingInstructions = value; }
        }

        public string ErrorQueue
        {
            get { return configurationDocument.ErrorQueue; }
            set { configurationDocument.ErrorQueue = value; }
        }

        public MessageTypeHandling MessageTypeHandlingFor(string type)
        {
            Verify.RequireStringNotNullOrWhitespace(type, "type");

            return MessageHandlingInstructions.FirstOrDefault(x => x.MessageTypes.Contains(type));
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

        protected static bool GetBoolean(string key, bool defaultValue = false, bool throwExceptionIfNotFound = false)
        {
            Verify.RequireStringNotNullOrWhitespace(key, "key");

            var val = defaultValue;
            var rawValue = ConfigurationManager.AppSettings.Get(key);
            if (rawValue != null)
            {
                bool.TryParse(rawValue, out val);
            }
            else if (throwExceptionIfNotFound)
            {
                throw new ConfigurationErrorsException(
                    string.Format("appsetting value required in application configuration file for key {0}", key));
            }

            return val;
        }

        protected static decimal? GetDecimal(string key, decimal? defaultValue = null,
            bool throwExceptionIfNotFound = false)
        {
            Verify.RequireStringNotNullOrWhitespace(key, "key");

            var val = decimal.MinValue;
            var rawValue = ConfigurationManager.AppSettings.Get(key);
            if (rawValue != null)
            {
                decimal.TryParse(rawValue, out val);
            }
            else if (throwExceptionIfNotFound)
            {
                throw new ConfigurationErrorsException(
                    string.Format("appsetting value required in application configuration file for key {0}", key));
            }

            return val != decimal.MinValue ? val : defaultValue;
        }

        protected static int? GetInt(string key, int? defaultValue, bool throwExceptionIfNotFound = false)
        {
            Verify.RequireStringNotNullOrWhitespace(key, "key");

            var val = int.MinValue;
            var rawValue = ConfigurationManager.AppSettings.Get(key);
            if (rawValue != null)
            {
                int.TryParse(rawValue, out val);
            }
            else if (throwExceptionIfNotFound)
            {
                throw new ConfigurationErrorsException(
                    string.Format("appsetting value required in application configuration file for key {0}", key));
            }

            return val != int.MinValue ? val : defaultValue;
        }

        protected static string GetString(string key, string defaultValue = null, bool throwExceptionIfNotFound = false)
        {
            var val = ConfigurationManager.AppSettings[key];

            if (val == null && throwExceptionIfNotFound)
            {
                throw new ConfigurationErrorsException(
                    string.Format("appsetting value required in application configuration file for key {0}", key));
            }

            return !string.IsNullOrWhiteSpace(val) ? val : defaultValue;
        }

        protected static double? GetDouble(string key, double? defaultValue, bool throwExceptionIfNotFound = false)
        {
            Verify.RequireStringNotNullOrWhitespace(key, "key");

            var val = 0d;
            var rawValue = ConfigurationManager.AppSettings.Get(key);
            if (rawValue != null)
            {
                double.TryParse(rawValue, out val);
            }
            else if (throwExceptionIfNotFound)
            {
                throw new ConfigurationErrorsException(
                    string.Format("appsetting value required in application configuration file for key {0}", key));
            }

            return Math.Abs(val - double.MinValue) > .000001 ? val : defaultValue;
        }
    }
}