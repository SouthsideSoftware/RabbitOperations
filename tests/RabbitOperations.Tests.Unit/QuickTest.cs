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
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.Service;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.Core.CastleWindsor;

namespace RabbitOperations.Tests.Unit
{
    [TestFixture]
    public class QuickTest
    {
        [Ignore("Spike")]
        [Test]
        public void RawJson()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Audit.json")))
            {
                data = reader.ReadToEnd();
            }

            //act
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var j = JObject.Parse(rawMessage.Body);
            var keys = j.Properties().Select(p => p.Name).ToList();
            foreach (var k in keys)
            {
                Console.WriteLine("{0} = {1}", k ?? "null", j[k] ?? "Null");
            }

        }
        [Test]
        [Ignore("Spike")]
        public void FactorySpike()
        {
            ServiceLocator.Container.Install(new RabbitOperations.Collector.CastleWindsor.Installer());
            var factory = ServiceLocator.Container.Resolve<IApplicationPollerFactory>();
            var poller = factory.Create(new QueueSettings("Test", new ApplicationConfiguration()), new CancellationToken());
            poller.QueueSettings.QueueName.Should().Be("Test");
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
#pragma warning disable 618
                    var consumer = new QueueingBasicConsumer(channel);
#pragma warning restore 618
                    channel.BasicConsume("audit", false, consumer);

                    Console.WriteLine(" [*] Waiting for messages. " +
                                      "To exit press CTRL+C");
                    while (true)
                    {
                        var ea =
                            (BasicDeliverEventArgs) consumer.Queue.Dequeue();
                        var rawMessage = new RawMessage(ea);
                        var jsonData = JsonConvert.SerializeObject(rawMessage, Formatting.Indented);
                        Console.Write(jsonData);
                        using (var outFile = new StreamWriter("out.json", false, Encoding.UTF8))
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
#pragma warning disable 618
                    var consumer = new QueueingBasicConsumer(channel);
#pragma warning restore 618
                    channel.BasicConsume("audit", false, consumer);

                    Console.WriteLine(" [*] Waiting for messages. " +
                                      "To exit press CTRL+C");
                    while (true)
                    {
                        var ea =
                            (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.Unicode.GetString(body);
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
