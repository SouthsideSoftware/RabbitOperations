using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using RabbitMQ.Client;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageParser.NServiceBus;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Collector.RavenDB.Interfaces;
using RabbitOperations.Collector.Service;
using RabbitOperations.Collector.Service.Interfaces;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database.Server;
using SouthsideUtility.Core.TestableSystem;
using SouthsideUtility.Core.TestableSystem.Interfaces;

namespace RabbitOperations.Collector.CastleWindsor
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.AddFacility<TypedFactoryFacility>();
            container.Register(Component.For<IQueuePoller>().ImplementedBy<QueuePoller>().LifestyleTransient(),
                Component.For<IQueuePollerFactory>().AsFactory(),
                Component.For<IRawMessage>().ImplementedBy<RawMessage>().LifestyleTransient(),
                Component.For<IMessageReader>().ImplementedBy<MessageReader>().LifestyleSingleton(),
                Component.For<ICancellationTokenSource>().ImplementedBy<CancellationTokenSourceWrapper>().LifestyleTransient(),
                Component.For<CancellationTokenSource>().LifestyleTransient(),
                Component.For<ISettings>().ImplementedBy<Settings>().LifestyleSingleton(),
                Component.For<IHeaderParser>().ImplementedBy<HeaderParser>().LifestyleSingleton(),
                Component.For<IRavenTenantInitializer>().ImplementedBy<RavenTenantInitializer>().LifestyleSingleton(),
                Component.For<IDocumentStore>().UsingFactoryMethod(x =>
                {
                    IDocumentStore docStore = null;
                    if (Settings.StaticEmbedRavenDB)
                    {
                        var port = Settings.StaticEmbeddedRavenDBManagementPort;
                        NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(port);
                        docStore = new EmbeddableDocumentStore
                        {
                            UseEmbeddedHttpServer = true,
                            ConnectionStringName = "RavenDB"
                        };
                        (docStore as EmbeddableDocumentStore).Configuration.Port = port;
                    }
                    else
                    {
                        docStore = new DocumentStore
                        {
                            ConnectionStringName = "RavenDB"
                        };
                    }
                    docStore.Initialize();

                    CreateDefaultDatabaseWithExpirationBundleIfNotExists(docStore);

                    return docStore;
                }).LifestyleSingleton(),
                Component.For<IConnectionFactory>().UsingFactoryMethod(x => new ConnectionFactory()
                {
                    uri = new Uri(container.Resolve<ISettings>().RabbitConnectionString),
                    RequestedHeartbeat = 30
                }));
        }

        private static void CreateDefaultDatabaseWithExpirationBundleIfNotExists(IDocumentStore docStore)
        {
            if (
                docStore.DatabaseCommands.GlobalAdmin.GetDatabaseNames(1000)
                    .All(dbName => dbName != Settings.StaticDefaultRavenDBTenant))
            {
                var databaseDocument = new DatabaseDocument
                {
                    Id = string.Format("Raven/Databases/{0}", Settings.StaticDefaultRavenDBTenant),
                    Settings = new Dictionary<string, string>
                    {
                        {"Raven/ActiveBundles", "DocumentExpiration"},
                        {"Raven/DataDir", string.Format("~/Databases/{0}", Settings.StaticDefaultRavenDBTenant)}
                    }
                };
                docStore.DatabaseCommands.GlobalAdmin.CreateDatabase(databaseDocument);
            }
        }
    }
}