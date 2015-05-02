using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using RabbitMQ.Client;
using RabbitOperations.Collector.Service;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Tests.Unit.Service
{
    [TestFixture]
    public class RabbitConnectionFactoryTests
    {
        [Test]
        public void ShouldStoreFactoryInDictionaryByRabbitUri()
        {
            //arrange
            const string rabbitConnectionString = "amqp://localhost2";
            var queueSettings = new QueueSettings("queue",
                new ApplicationConfiguration {RabbitConnectionString = rabbitConnectionString});
            var factory = new RabbitConnectionFactory();
            
            //act
            factory.Create(queueSettings.RabbitConnectionString, (ushort) queueSettings.HeartbeatIntervalSeconds);

            //assert
            factory.connectionFactories.ContainsKey(rabbitConnectionString).Should().BeTrue();

        }

        [Test]
        public void ShouldReuseInstanceOfConnectionFactoryWhenItExists()
        {
            //arrange
            const string rabbitConnectionString = "amqp://localhost2";
            var queueSettings = new QueueSettings("queue",
                new ApplicationConfiguration { RabbitConnectionString = rabbitConnectionString });
            var factory = new RabbitConnectionFactory();
            var originalInstance = factory.Create(queueSettings.RabbitConnectionString, (ushort) queueSettings.HeartbeatIntervalSeconds);

            //act
            var secondInstance = factory.Create(queueSettings.RabbitConnectionString, (ushort)queueSettings.HeartbeatIntervalSeconds);

            //assert
            originalInstance.Should().BeSameAs(secondInstance);

        }

        [Test]
        public void ShouldHandleMultipleConnections()
        {
            //arrange
            const string rabbitHost1 = "localhost1";
            var rabbitConnectionString1 = string.Format("amqp://{0}",rabbitHost1);
            var queueSettings1 = new QueueSettings("queue",
                new ApplicationConfiguration { RabbitConnectionString = rabbitConnectionString1 });

            const string rabbitHost2 = "localhost2";
            var rabbitConnectionString2 = string.Format("amqp://{0}", rabbitHost2);
            var queueSettings2 = new QueueSettings("queue",
                new ApplicationConfiguration { RabbitConnectionString = rabbitConnectionString2 });
            var factory = new RabbitConnectionFactory();

            //act
            var instance1 = factory.Create(queueSettings1.RabbitConnectionString, (ushort)queueSettings1.HeartbeatIntervalSeconds);
            var instance2 = factory.Create(queueSettings2.RabbitConnectionString, (ushort)queueSettings2.HeartbeatIntervalSeconds);

            //assert
            instance1.Should().NotBeSameAs(instance2);
            instance1.Should().BeOfType<ConnectionFactory>();
            instance2.Should().BeOfType<ConnectionFactory>();
            (instance1 as ConnectionFactory).HostName.Should().Be(rabbitHost1);
            (instance2 as ConnectionFactory).HostName.Should().Be(rabbitHost2);

        }
    }
}
