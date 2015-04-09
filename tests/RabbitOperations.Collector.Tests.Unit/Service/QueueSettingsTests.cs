using System.Net;
using FluentAssertions;
using NUnit.Framework;
using RabbitOperations.Collector.Service;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Tests.Unit.Service
{
    [TestFixture]
    public class QueueSettingsTests
    {
        [Test]
        public void ShouldSetQueueNameFromConstructor()
        {
            //arrange and act
            const string queueName = "queueName1";
            var queueSettings = new QueueSettings(queueName, new EnvironmentConfiguration());

            //assert
            queueSettings.QueueName.Should().Be(queueName);
        }

        [Test]
        public void ShouldSetEnvironmentIdFromConstructor()
        {
            //arrange and act
            const string environmentId = "one";
            var settings = new QueueSettings("x", new EnvironmentConfiguration {EnvironmentId = environmentId});
            var queueSettings = settings;

            //assert
            queueSettings.EnvironmentId.Should().Be(environmentId);
        }

        [Test]
        public void ShouldSetDocumentExpirationInHoursFromConstructor()
        {
            //arrange and act
            const int documentExpirationInHours = 3;
            var settings = new QueueSettings("x", new EnvironmentConfiguration { EnvironmentName = "one", DocumentExpirationInHours = documentExpirationInHours});
            var queueSettings = settings;

            //assert
            queueSettings.DocumentExpirationInHours.Should().Be(documentExpirationInHours);
        }

        [Test]
        public void ShouldSetEnvironmentNameFromConstructor()
        {
            //arrange and act
            const string environmentName = "one";
            var settings = new QueueSettings("x", new EnvironmentConfiguration {EnvironmentName = environmentName});
            var queueSettings = settings;

            //assert
            queueSettings.EnvironmentName.Should().Be(environmentName);
        }

        [Test]
        public void ShouldSetRabbitConnectionStringFromConstructor()
        {
            //arrange and act
            const string rabbitConnectionString = "one";
            var settings = new QueueSettings("x",
                new EnvironmentConfiguration {RabbitConnectionString = rabbitConnectionString});
            var queueSettings = settings;

            //assert
            queueSettings.RabbitConnectionString.Should().Be(rabbitConnectionString);
        }

        [Test]
        public void ShouldSetRabbitManagementWebUrlWhenPortIsDefault()
        {
            //arrange and act
            const string rabbitConnectionString = "http://host:99";
            var settings = new QueueSettings("x",
                new EnvironmentConfiguration { RabbitConnectionString = rabbitConnectionString });
            var queueSettings = settings;

            //assert
            queueSettings.RabbitManagementWebUrl.Should().Be("http://host:15672");
        }

        [Test]
        public void ShouldSetRabbitManagementWebUrlWhenPortIsNotDefault()
        {
            //arrange and act
            const string rabbitConnectionString = "http://host:99";
            var settings = new QueueSettings("x",
                new EnvironmentConfiguration { RabbitConnectionString = rabbitConnectionString, RabbitManagementPort = 101});
            var queueSettings = settings;

            //assert
            queueSettings.RabbitManagementWebUrl.Should().Be("http://host:101");
        }

        [Test]
        public void ShouldSetMaxMessagesPerRunFromConstructor()
        {
            //arrange and act
            const int maxMessagesPerRun = 11;
            var settings = new QueueSettings("x", new EnvironmentConfiguration {MaxMessagesPerRun = maxMessagesPerRun});
            var queueSettings = settings;

            //assert
            queueSettings.MaxMessagesPerRun.Should().Be(maxMessagesPerRun);
        }

        [Test]
        public void ShouldSetPollingTimeoutMillisecondsFromConstructor()
        {
            //arrange and act
            const int pollingTimeoutMilliseconds = 11000;
            var settings = new QueueSettings("x",
                new EnvironmentConfiguration {PollingTimeoutMilliseconds = pollingTimeoutMilliseconds});
            var queueSettings = settings;

            //assert
            queueSettings.PollingTimeoutMilliseconds.Should().Be(pollingTimeoutMilliseconds);
        }

        [Test]
        public void ShouldSetHeartbeatIntervalSecondsFromConstructor()
        {
            //arrange and act
            const int heartbeatIntervalSeconds = 11000;
            var settings = new QueueSettings("x",
                new EnvironmentConfiguration {HeartbeatIntervalSeconds = heartbeatIntervalSeconds});
            var queueSettings = settings;

            //assert
            queueSettings.HeartbeatIntervalSeconds.Should().Be(heartbeatIntervalSeconds);
        }
    }
}