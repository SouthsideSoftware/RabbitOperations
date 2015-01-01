using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitOperations.Collector.Service;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.CastleWindsor;

namespace RabbitOperations.Tests.Unit
{
    [TestFixture]
    public class QuickTest
    {   
        [Test]
        public void ReadAudit()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Audit.json")))
            {
                data = reader.ReadToEnd();
            }

            //act
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            //assert
            rawMessage.Headers.Count.Should().BeGreaterThan(1);

        }

        [Test]
        public void ReadError()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }

            //act
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            //assert
            rawMessage.Headers.Count.Should().BeGreaterThan(1);

        }

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
            var factory = new ConnectionFactory() { uri = new Uri("amqp://test:test@internal-test-rabbit-1582700312.us-east-1.elb.amazonaws.com/qa1"), };
            uint openingCount;
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    openingCount = channel.QueueDeclare("audit", true, false, false, null).MessageCount;

                    channel.BasicQos(0, 1, false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("error", false, consumer);

                    Console.WriteLine(" [*] Waiting for messages. " +
                                      "To exit press CTRL+C");
                    while (true)
                    {
                        var ea =
                            (BasicDeliverEventArgs) consumer.Queue.Dequeue();
                        var rawMessage = new RawMessage(ea);
                        var jsonData = JsonConvert.SerializeObject(rawMessage, Formatting.Indented);
                        Console.Write(jsonData);
                        using (var outFile = new StreamWriter("out.json"))
                        {
                            outFile.Write(jsonData);
                        }

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
