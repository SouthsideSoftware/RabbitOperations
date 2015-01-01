using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.CastleWindsor;

namespace RabbitOperations.Tests.Unit
{
    [TestFixture]
    public class QuickTest
    {
        [Test]
        [Ignore("Spike")]
        public void FactorySpike()
        {
            ServiceLocator.Container.Install(new RabbitOperations.Collector.CastleWindsor.Installer());
            var factory = ServiceLocator.Container.Resolve<IQueuePollerFactory>();
            var poller = factory.Create("Test");
            poller.QueueName.Should().Be("Test");
            factory.Destroy(poller);
        }
        [Test]
        [Ignore("Spike")]
        public void ReadOne()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", };
            uint openingCount;
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    openingCount = channel.QueueDeclare("audit", true, false, false, null).MessageCount;

                    channel.BasicQos(0, 1, false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("audit", false, consumer);

                    Console.WriteLine(" [*] Waiting for messages. " +
                                      "To exit press CTRL+C");
                    while (true)
                    {
                        var ea =
                            (BasicDeliverEventArgs) consumer.Queue.Dequeue();
          
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);

                        channel.BasicAck(ea.DeliveryTag, false);
                        break;
                    }

                    Console.WriteLine("Exit");
                }
            }
            factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var closingCount = channel.QueueDeclarePassive("audit").MessageCount;
                    closingCount.Should().Be(openingCount - 1);
                }
            }
        }

        [Test]
        [Ignore("Spike")]
        public void ReadOneAndNack()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            uint openingCount;
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    openingCount = channel.QueueDeclare("audit", true, false, false, null).MessageCount;

                    channel.BasicQos(0, 1, false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("audit", false, consumer);

                    Console.WriteLine(" [*] Waiting for messages. " +
                                      "To exit press CTRL+C");
                    while (true)
                    {
                        var ea =
                            (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);

                        channel.BasicNack(ea.DeliveryTag, false, true);
                        break;
                    }

                    Console.WriteLine("Exit");
                }
            }
            factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var closingCount = channel.QueueDeclarePassive("audit").MessageCount;
                    closingCount.Should().Be(openingCount);
                }
            }
        }

    }
}
