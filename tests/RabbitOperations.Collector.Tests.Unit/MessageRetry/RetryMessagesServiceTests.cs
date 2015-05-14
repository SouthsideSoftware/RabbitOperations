using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using RabbitOperations.Collector.MessageRetry;
using Raven.Abstractions.Data;
using SouthsideUtility.RavenDB.Testing;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.MessageBusTechnologies.Common;
using RabbitOperations.Collector.MessageBusTechnologies.NServiceBus;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Domain;
using Raven.Client;
using Headers = RabbitOperations.Collector.MessageBusTechnologies.Common.Headers;

namespace RabbitOperations.Collector.Tests.Unit.MessageRetry
{
    [TestFixture]
    public class RetryMessagesServiceTests : RavenDbTest
    {
        [TestFixtureSetUp]
        public void TestFixtireSetup()
        {
            new RavenTenantInitializer(Store).InitializeTenant(Settings.StaticDefaultRavenDBTenant);
        }

        [Test]
        public void ShouldReturnResultWithMeaningfulAdditionalInfoWhenOriginalMessageMissing()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {-1}
            });

            //assert
            result.RetryMessageItems.First()
                .AdditionalInfo.Should()
                .ContainEquivalentOf("original message does not exist");
        }

        [Test]
        public void ShouldReturnResultIndicatingNoRetryWhenOriginalMessageMissing()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { -1 }
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Test]
        public void ShouldChangeStatusOfOriginalMessageToRetryPendingWhenRetryStarts()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.Unresolved;
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { originalMessge.Id }
            });

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var message = session.Load<MessageDocument>(originalMessge.Id);
                message.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.RetryPending);
            }
        }

        public void ShouldLeaveStatusOfOriginalMessageUnchangedWhenRetryIsNotAllowed()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.NotAnError;
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { originalMessge.Id }
            });

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var message = session.Load<MessageDocument>(originalMessge.Id);
                message.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.NotAnError);
            }
        }

        [Test]
        public void ShouldNotRetryWhenOriginalMessageIsResolved()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.Resolved;
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { originalMessge.Id }
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Test]
        public void ShouldNotRetryWhenOriginalMessageIsNotAnError()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.NotAnError;
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { originalMessge.Id }
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Test]
        public void ShouldNotRetryWhenOriginalMessageIsClosed()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.Closed;
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { originalMessge.Id }
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Test]
        public void ShouldNotRetryWhenOriginalMessageIsRetryPending()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.RetryPending;
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { originalMessge.Id }
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Test]
        public void ShouldRetainAllHeadersOfOriginalMessageAfterRetry()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.Unresolved;
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { originalMessge.Id }
            });

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var message = session.Load<MessageDocument>(originalMessge.Id);
                message.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.RetryPending);
            }
        }

        [Test]
        public void ShouldReturnAdditionalErrorStatusStringRetryPendingWhenItWorks()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<ICreateRetryMessagesFromOriginal>(() => new CreateRetryMessageFromOriginalService());
            var service = fixture.Create<RetryMessagesService>();

            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var originalMessge = new MessageDocument();
            new HeaderParser().AddHeaderInformation(rawMessage, originalMessge);
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { originalMessge.Id }
            });

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var message = session.Load<MessageDocument>(originalMessge.Id);
                AssertRetriedMessageHeadersExcludingRetryIdSameAsOriginal(message, originalMessge);
            }
        }

        private static void AssertRetriedMessageHeadersExcludingRetryIdSameAsOriginal(MessageDocument message,
            MessageDocument originalMessge)
        {
            message.Headers.Remove(Headers.Retry);
            message.Headers.ShouldBeEquivalentTo(originalMessge.Headers);
        }

        [Test]
        public void ShouldReturnCanRetryFalseWhenItWorks()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            var originalMessge = new MessageDocument();
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { originalMessge.Id }
            });

            //assert
            result.RetryMessageItems.First().CanRetryOriginalMessage.Should().BeFalse();
        }
    }
}
