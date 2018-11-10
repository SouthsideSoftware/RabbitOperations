using FluentAssertions;
using NUnit.Framework;
using AutoFixture;
using AutoFixture.AutoMoq;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.MessageBusTechnologies.Common;
using RabbitOperations.Collector.MessageBusTechnologies.NServiceBus;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Collector.Service;
using RabbitOperations.Domain;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.RavenDB.Testing;
using Headers = RabbitOperations.Collector.MessageBusTechnologies.Common.Headers;

namespace RabbitOperations.Collector.Tests.Unit.Service
{
    [TestFixture]
    public class StoreMessagesThatAreRetriesServiceTests : RavenDbTest
    {
        [OneTimeSetUp]
        public void TestFixtireSetup()
        {
            new RavenTenantInitializer(Store).InitializeTenant(Settings.StaticDefaultRavenDBTenant);
        }

        [Test]
        public void ShouldStoreErrorWithProperErrorStatusWhenOriginalDoesNotExist()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Add(Headers.Retry, "test");

            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreRetriesService>();

            //act
            var id = service.Store(rawMessage,
                new QueueSettings("test", new ApplicationConfiguration {ApplicationId = "test"}));

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                doc.Should().NotBeNull("Stored document should not be null");
                doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.IsRetry, "AdditionalErrorStatus should be IsRetry");
                doc.IsError.Should().BeTrue("IsError should be true");
                doc.CanRetry.Should().BeFalse("Can retry should be true");
            }
        }

        [Test]
        public void ShouldStoreAuditWithProperErrorStatusWhenOriginalDoesNotExist()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();
            rawMessage.Headers.Add(Headers.Retry, "test");

            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreRetriesService>();

            //act
            var id = service.Store(rawMessage,
                new QueueSettings("test", new ApplicationConfiguration { ApplicationId = "test" }));

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                doc.Should().NotBeNull("Stored document should not be null");
                doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.IsRetry, "AdditionalErrorStatus should be IsRetry");
                doc.IsError.Should().BeFalse("IsError should be false");
                doc.CanRetry.Should().BeFalse("Can retry should be false");
            }
        }

        [Test]
        public void ShouldNotStoreErrorRetryAsRootDocumentWhenOriginalDoesExist()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreRetriesService>();
            var originalMessageStorageService = fixture.Create<StoreMessagesThatAreNotRetriesService>();
            var queueSettings = new QueueSettings("test", new ApplicationConfiguration {ApplicationId = "test"});

            var originalMessage = MessageTestHelpers.GetErrorMessage();
            var originalId = originalMessageStorageService.Store(originalMessage, queueSettings);

            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Add(Headers.Retry, originalId.ToString());

            //act
            var id = service.Store(rawMessage, queueSettings);

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                doc.Should().NotBeNull("Document should exist");
                doc.Id.Should().Be(originalId, "the id of the original document should be returned");
            }
        }

        [Test]
        public void ShouldNotStoreAuditRetryAsRootDocumentWhenOriginalDoesExist()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreRetriesService>();
            var originalMessageStorageService = fixture.Create<StoreMessagesThatAreNotRetriesService>();
            var queueSettings = new QueueSettings("test", new ApplicationConfiguration {ApplicationId = "test"});

            var originalMessage = MessageTestHelpers.GetErrorMessage();
            var originalId = originalMessageStorageService.Store(originalMessage, queueSettings);

            var rawMessage = MessageTestHelpers.GetAuditMessage();
            rawMessage.Headers.Add(Headers.Retry, originalId.ToString());

            //act
            var id = service.Store(rawMessage, queueSettings);

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                doc.Should().NotBeNull("Document should exist");
                doc.Id.Should().Be(originalId, "the id of the original document should be returned");
            }
        }

        [Test]
        public void ShouldHaveProperErrorStatusAfterStoringAnErrorRetry()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreRetriesService>();
            var originalMessageStorageService = fixture.Create<StoreMessagesThatAreNotRetriesService>();
            var queueSettings = new QueueSettings("test", new ApplicationConfiguration { ApplicationId = "test" });

            var originalMessage = MessageTestHelpers.GetErrorMessage();
            var originalId = originalMessageStorageService.Store(originalMessage, queueSettings);

            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Add(Headers.Retry, originalId.ToString());

            //act
            var id = service.Store(rawMessage, queueSettings);

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                doc.Should().NotBeNull("Document should exist");
                doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.Unresolved, "additional error status should be unresolved");
                doc.IsError.Should().BeTrue("IsError should be true");
                doc.CanRetry.Should().BeTrue("Can retry should be true");
            }
        }

        [Test]
        public void ShouldHaveProperErrorStatusAfterStoringAnAuditRetry()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreRetriesService>();
            var originalMessageStorageService = fixture.Create<StoreMessagesThatAreNotRetriesService>();
            var queueSettings = new QueueSettings("test", new ApplicationConfiguration { ApplicationId = "test" });

            var originalMessage = MessageTestHelpers.GetErrorMessage();
            var originalId = originalMessageStorageService.Store(originalMessage, queueSettings);

            var rawMessage = MessageTestHelpers.GetAuditMessage();
            rawMessage.Headers.Add(Headers.Retry, originalId.ToString());

            //act
            var id = service.Store(rawMessage, queueSettings);

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                doc.Should().NotBeNull("Document should exist");
                doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.Resolved, "additional error status should be resolved");
                doc.IsError.Should().BeTrue("IsError should be true");
                doc.CanRetry.Should().BeFalse("Can retry should be false");
            }
        }

         [Test]
        public void ShouldStoreRetryHistoryAfterStoringAnErrorRetry()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreRetriesService>();
            var originalMessageStorageService = fixture.Create<StoreMessagesThatAreNotRetriesService>();
            var queueSettings = new QueueSettings("test", new ApplicationConfiguration { ApplicationId = "test" });

            var originalMessage = MessageTestHelpers.GetErrorMessage();
            var originalId = originalMessageStorageService.Store(originalMessage, queueSettings);

            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Add(Headers.Retry, originalId.ToString());

            //act
            var id = service.Store(rawMessage, queueSettings);

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                doc.Should().NotBeNull("Document should exist");
                doc.Retries.Count.Should().Be(1);
            }
        }

        [Test]
        public void ShouldStoreRetryHistoryAfterStoringAnAuditRetry()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreRetriesService>();
            var originalMessageStorageService = fixture.Create<StoreMessagesThatAreNotRetriesService>();
            var queueSettings = new QueueSettings("test", new ApplicationConfiguration { ApplicationId = "test" });

            var originalMessage = MessageTestHelpers.GetErrorMessage();
            var originalId = originalMessageStorageService.Store(originalMessage, queueSettings);

            var rawMessage = MessageTestHelpers.GetAuditMessage();
            rawMessage.Headers.Add(Headers.Retry, originalId.ToString());

            //act
            var id = service.Store(rawMessage, queueSettings);

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                doc.Should().NotBeNull("Document should exist");
                doc.Retries.Count.Should().Be(1);
            }
        }
    }
}
