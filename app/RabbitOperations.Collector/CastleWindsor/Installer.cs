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
using RabbitOperations.Collector.Service;
using RabbitOperations.Collector.Service.Interfaces;
using Raven.Client;
using Raven.Client.Document;
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
                Component.For<IDocumentStore>().UsingFactoryMethod(x =>
                {
                    var docStore = new DocumentStore
                    {
                        ConnectionStringName = "RavenDB"
                    };
                    docStore.Initialize();
                    return docStore;
                }).LifestyleSingleton(),
                Component.For<IConnectionFactory>().UsingFactoryMethod(x => new ConnectionFactory()
                {
                    uri = new Uri(container.Resolve<ISettings>().RabbitConnectionString),
                    RequestedHeartbeat = 30
                }));
        }
    }
}