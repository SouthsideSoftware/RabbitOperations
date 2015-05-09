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
            var queueSettings = new QueueSettings(queueName, new ApplicationConfiguration());

            //assert
            queueSettings.QueueName.Should().Be(queueName);
        }

        [Test]
        public void ShouldSetApplicationIdFromConstructor()
        {
            //arrange and act
            const string applicationId = "one";
            var settings = new QueueSettings("x", new ApplicationConfiguration {ApplicationId = applicationId});
            var queueSettings = settings;

            //assert
            queueSettings.ApplicationId.Should().Be(applicationId);
        }

        [Test]
        public void ShouldSetDocumentExpirationInHoursFromConstructor()
        {
            //arrange and act
            const int documentExpirationInHours = 3;
            var settings = new QueueSettings("x", new ApplicationConfiguration { ApplicationName = "one", DocumentExpirationInHours = documentExpirationInHours});
            var queueSettings = settings;

            //assert
            queueSettings.DocumentExpirationInHours.Should().Be(documentExpirationInHours);
        }

        [Test]
        public void ShouldSetApplicationNameFromConstructor()
        {
            //arrange and act
            const string applicationName = "one";
            var settings = new QueueSettings("x", new ApplicationConfiguration {ApplicationName = applicationName});
            var queueSettings = settings;

            //assert
            queueSettings.ApplicationName.Should().Be(applicationName);
        }

        [Test]
        public void ShouldSetRabbitConnectionStringFromConstructor()
        {
            //arrange and act
            const string rabbitConnectionString = "one";
            var settings = new QueueSettings("x",
                new ApplicationConfiguration {RabbitConnectionString = rabbitConnectionString});
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
                new ApplicationConfiguration { RabbitConnectionString = rabbitConnectionString });
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
                new ApplicationConfiguration { RabbitConnectionString = rabbitConnectionString, RabbitManagementPort = 101});
            var queueSettings = settings;

            //assert
            queueSettings.RabbitManagementWebUrl.Should().Be("http://host:101");
        }

        [Test]
        public void ShouldSetMaxMessagesPerRunFromConstructor()
        {
            //arrange and act
            const int maxMessagesPerRun = 11;
            var settings = new QueueSettings("x", new ApplicationConfiguration {MaxMessagesPerRun = maxMessagesPerRun});
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
                new ApplicationConfiguration {PollingTimeoutMilliseconds = pollingTimeoutMilliseconds});
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
                new ApplicationConfiguration {HeartbeatIntervalSeconds = heartbeatIntervalSeconds});
            var queueSettings = settings;

            //assert
            queueSettings.HeartbeatIntervalSeconds.Should().Be(heartbeatIntervalSeconds);
        }
    }
}