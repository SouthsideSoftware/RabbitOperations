using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using FluentAssertions;
using RabbitOperations.Collector.MessageBusTechnologies.NServiceBus;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.Tests.Unit.RavenDb;
using RabbitOperations.Domain;
using SouthsideUtility.Core.DesignByContract;
using Xunit;
using Headers = RabbitOperations.Collector.MessageBusTechnologies.Common.Headers;
using RabbitOperations.Collector.RavenDb;

namespace RabbitOperations.Collector.Tests.Unit.MessageRetry
{
    public class RetryMessagesServiceTests : IClassFixture<RavenDbInMemory>
    {
        private readonly RavenDbInMemory ravenDbInMemory;

        public RetryMessagesServiceTests(RavenDbInMemory ravenDbInMemory)
        {
            Verify.RequireNotNull(ravenDbInMemory, "ravenDbInMemory");
            this.ravenDbInMemory = ravenDbInMemory;
        }

        [Fact]
        public void ShouldReturnResultWithMeaningfulAdditionalInfoWhenOriginalMessageMissing()
        {
            //arrange
            var service = CreateRetryMessageService();

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

        private RetryMessagesService CreateRetryMessageService()
        {
            var mocker = AutoMock.GetLoose();
            mocker.Provide(ravenDbInMemory.Store);
            mocker.Provide(new CreateRetryMessageFromOriginalService());
            var service = mocker.Create<RetryMessagesService>();
            return service;
        }

        [Fact]
        public void ShouldReturnResultIndicatingNoRetryWhenOriginalMessageMissing()
        {
            //arrange
            var service = CreateRetryMessageService();

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {-1}
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Fact]
        public void ShouldChangeStatusOfOriginalMessageToRetryPendingWhenRetryStarts()
        {
            //arrange
            var service = CreateRetryMessageService();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.Unresolved;
            using (var session = ravenDbInMemory.Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {originalMessge.Id}
            });

            //assert
            using (var session = ravenDbInMemory.Store.OpenSessionForDefaultTenant())
            {
                var message = session.Load<MessageDocument>(originalMessge.Id);
                message.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.RetryPending);
            }
        }

        [Fact]
        public void ShouldLeaveStatusOfOriginalMessageUnchangedWhenRetryIsNotAllowed()
        {
            //arrange
            var service = CreateRetryMessageService();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.NotAnError;
            using (var session =ravenDbInMemory. Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {originalMessge.Id}
            });

            //assert
            using (var session =ravenDbInMemory. Store.OpenSessionForDefaultTenant())
            {
                var message = session.Load<MessageDocument>(originalMessge.Id);
                message.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.NotAnError);
            }
        }

        [Fact]
        public void ShouldNotRetryWhenOriginalMessageIsResolved()
        {
            //arrange
            var service = CreateRetryMessageService();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.Resolved;
            using (var session = ravenDbInMemory.Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {originalMessge.Id}
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Fact]
        public void ShouldNotRetryWhenOriginalMessageIsNotAnError()
        {
            //arrange
            var service = CreateRetryMessageService();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.NotAnError;
            using (var session = ravenDbInMemory.Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {originalMessge.Id}
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Fact]
        public void ShouldNotRetryWhenOriginalMessageIsClosed()
        {
            //arrange
            var service = CreateRetryMessageService();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.Closed;
            using (var session = ravenDbInMemory.Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {originalMessge.Id}
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Fact]
        public void ShouldNotRetryWhenOriginalMessageIsRetryPending()
        {
            //arrange
            var service = CreateRetryMessageService();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.RetryPending;
            using (var session = ravenDbInMemory.Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {originalMessge.Id}
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Fact]
        public void ShouldRetainAllHeadersOfOriginalMessageAfterRetry()
        {
            //arrange
            var service = CreateRetryMessageService();

            var originalMessge = new MessageDocument();
            originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.Unresolved;
            using (var session = ravenDbInMemory.Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {originalMessge.Id}
            });

            //assert
            using (var session = ravenDbInMemory.Store.OpenSessionForDefaultTenant())
            {
                var message = session.Load<MessageDocument>(originalMessge.Id);
                message.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.RetryPending);
            }
        }

        [Fact]
        public void ShouldReturnAdditionalErrorStatusStringRetryPendingWhenItWorks()
        {
            //arrange
            var service = CreateRetryMessageService();

            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var originalMessge = new MessageDocument();
            new HeaderParser().AddHeaderInformation(rawMessage, originalMessge);
            using (var session = ravenDbInMemory.Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {originalMessge.Id}
            });

            //assert
            using (var session =ravenDbInMemory.Store.OpenSessionForDefaultTenant())
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

        [Fact]
        public void ShouldReturnCanRetryFalseWhenItWorks()
        {
            //arrange
            var service = CreateRetryMessageService();

            var originalMessge = new MessageDocument();
            using (var session = ravenDbInMemory.Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {originalMessge.Id}
            });

            //assert
            result.RetryMessageItems.First().CanRetryOriginalMessage.Should().BeFalse();
        }
    }
}