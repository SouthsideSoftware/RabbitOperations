using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Releasers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Nancy.ViewEngines;
using RabbitMQ.Client;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Host;
using RabbitOperations.Collector.Host.Interfaces;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageParser.NServiceBus;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.MessageRetry.NServiceBus;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Collector.RavenDB.Interfaces;
using RabbitOperations.Collector.RavenDB.Query;
using RabbitOperations.Collector.RavenDB.Query.Interfaces;
using RabbitOperations.Collector.RavenDB.SchemaUpdates;
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
#pragma warning disable 618
            container.Kernel.ReleasePolicy = new NoTrackingReleasePolicy();
#pragma warning restore 618
            container.Kernel.AddFacility<TypedFactoryFacility>();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
            container.Register(Component.For<IQueuePoller>().ImplementedBy<QueuePoller>().LifestyleTransient(),
                Component.For<IQueuePollerFactory>().AsFactory(),
                Component.For<IRabbitConnectionFactory>().ImplementedBy<RabbitConnectionFactory>().LifestyleSingleton(),
                Component.For<IRawMessage>().ImplementedBy<RawMessage>().LifestyleTransient(),
                Component.For<IHost>().ImplementedBy<Host.Host>().LifestyleSingleton(),
                Component.For<IQueuePollerHost>().ImplementedBy<QueuePollerHost>().LifestyleTransient(),
                Component.For<IWebHost>().ImplementedBy<WebHost>().LifestyleTransient(),
                Component.For<ISubHostFactory>().AsFactory().LifestyleSingleton(),
                Component.For<ISchemaUpdater>().ImplementedBy<SchemaUpdater>().LifestyleTransient(),
                Component.For<IUpdateSchemaVersion>().ImplementedBy<ToVersion0002>().LifestyleTransient(),
                Component.For<IUpdateSchemaVersion>().ImplementedBy<ToVersion0006>().LifestyleTransient(),
                Component.For<IUpdateSchemaVersion>().ImplementedBy<ToVersion0007>().LifestyleTransient(),
                Component.For<IUpdateSchemaVersion>().ImplementedBy<ToVersion0008>().LifestyleTransient(),
                Component.For<IUpdateSchemaVersion>().ImplementedBy<ToVersion0010>().LifestyleTransient(),
                Component.For<IUpdateSchemaVersion>().ImplementedBy<ToVersion0012>().LifestyleTransient(),
                Component.For<IActiveQueuePollers>().ImplementedBy<ActiveQueuePollers>().LifestyleSingleton(),
                Component.For<IBasicSearch>().ImplementedBy<BasicSearch>().LifestyleSingleton(),
                Component.For<ICancellationTokenSource>()
                    .ImplementedBy<CancellationTokenSourceWrapper>()
                    .LifestyleTransient(),
                Component.For<CancellationTokenSource>().LifestyleTransient(),
                Component.For<ISettings>().ImplementedBy<Settings>().LifestyleSingleton(),
                Component.For<IHeaderParser>().ImplementedBy<HeaderParser>().LifestyleSingleton(),
                Component.For<IRavenTenantInitializer>().ImplementedBy<RavenTenantInitializer>().LifestyleSingleton(),
                Component.For<IQualifiedSchemaUpdatersFactory>().ImplementedBy<QualifiedSchemaUpdatersFactory>().LifestyleSingleton(),
                Component.For<IRetryMessages>().ImplementedBy<RetryMessagesService>().LifestyleSingleton(),
                Component.For<ICreateRetryMessagesFromOriginal>().ImplementedBy<CreateRetryMessageFromOriginalService>().LifestyleSingleton(),
                Component.For<IDetermineRetryDestination>().ImplementedBy<DetermineRetryDestinationService>().LifestyleSingleton(),
                Component.For<IAddRetryTrackingHeaders>().ImplementedBy<AddRetryTrackingHeadersService>().LifestyleSingleton(),
                Component.For<ISendMessages>().ImplementedBy<SendMessagesService>().LifestyleSingleton(),
                Component.For<IStoreMessages>().ImplementedBy<StoreMessagesThatAreRetriesService>().LifestyleSingleton(),
                Component.For<IStoreMessages>().ImplementedBy<StoreMessagesThatAreNotRetriesService>().LifestyleSingleton(),
                Component.For<IStoreMessagesFactory>().ImplementedBy<StoreMessagesFactory>().LifestyleSingleton(),
                Component.For<ICreateBasicProperties>().ImplementedBy<CreateBasicPropertiesService>().LifestyleSingleton(),
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
                }).LifestyleSingleton());
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