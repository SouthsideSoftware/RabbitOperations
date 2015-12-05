using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Collector.RavenDB.Interfaces;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database.Server;
using SouthsideUtility.Core.DesignByContract;
using Module = Autofac.Module;

namespace RabbitOperations.Collector.RavenDb
{
    public class RavenDbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((context, parameters) =>
            {
                IDocumentStore docStore = null;
                var settings = context.Resolve<IOptions<RavenDbSettings>>().Value;
                if (settings.UseEmbedded)
                {
                    var port = settings.EmbeddedRavenDb.ManagementPort;
                    NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(port);
                    docStore = new EmbeddableDocumentStore
                    {
                        UseEmbeddedHttpServer = true
                    };
                    (docStore as EmbeddableDocumentStore).Configuration.Port = port;
                    (docStore as EmbeddableDocumentStore).Configuration.DataDirectory =
                        settings.EmbeddedRavenDb.DataDirectory;
                }
                else
                {
                    docStore = new DocumentStore
                    {
                        Url = settings.ExternalRavenDb.Url
                    };
                }
                docStore.Conventions.DisableProfiling = true;
                docStore.Initialize();

                CreateDefaultDatabaseWithExpirationBundleIfNotExists(docStore, settings);

                return docStore;
            }).As<IDocumentStore>().SingleInstance();

            builder.RegisterType<SchemaUpdater>().As<ISchemaUpdater>().SingleInstance();
            builder.RegisterType<QualifiedSchemaUpdatersFactory>()
                .As<IQualifiedSchemaUpdatersFactory>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => typeof (IUpdateSchemaVersion).IsAssignableFrom(t))
                .As<IUpdateSchemaVersion>()
                .SingleInstance();
        }

        private static void CreateDefaultDatabaseWithExpirationBundleIfNotExists(IDocumentStore docStore, RavenDbSettings settings)
        {
            if (
                docStore.DatabaseCommands.GlobalAdmin.GetDatabaseNames(1000)
                    .All(dbName => dbName != settings.DefaultTenant))
            {
                var databaseDocument = new DatabaseDocument
                {
                    Id = string.Format("Raven/Databases/{0}", settings.DefaultTenant),
                    Settings = new Dictionary<string, string>
                    {
                        {"Raven/ActiveBundles", "DocumentExpiration"},
                        {"Raven/DataDir", string.Format("~/Databases/{0}", settings.DefaultTenant)}
                    }
                };
                docStore.DatabaseCommands.GlobalAdmin.CreateDatabase(databaseDocument);
            }
        }
    }
}
