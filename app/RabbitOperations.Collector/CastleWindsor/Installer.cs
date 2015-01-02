using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.Service;
using RabbitOperations.Collector.Service.Interfaces;

namespace RabbitOperations.Collector.CastleWindsor
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.AddFacility<TypedFactoryFacility>();
            container.Register(Component.For<IQueuePoller>().ImplementedBy<QueuePoller>().LifestyleTransient(),
                Component.For<IQueuePollerFactory>().AsFactory(),
                Component.For<IRawMessage>().ImplementedBy<RawMessage>().LifestyleTransient());
        }
    }
}
