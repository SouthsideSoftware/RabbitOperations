using System.Collections.Generic;
using FluentAssertions;
using RabbitOperations.Collector.MessageBusTechnologies.Common;
using RabbitOperations.Collector.MessageRetry;
using Xunit;

namespace RabbitOperations.Collector.Tests.Unit.MessageRetry
{
    public class AddRetryTrackingHeadersTests
    {
        [Fact]
        public void ShouldAddHeaderForTrackingTheRetry()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();

            var retryHeaderService = new AddRetryTrackingHeadersService();

            //act
            retryHeaderService.AddTrackingHeaders(rawMessage, 101);

            //assert
            rawMessage.Headers.Should()
                .Contain(new KeyValuePair<string, string>(Headers.Retry, "101"));
        }

        [Fact]
        public void ShouldReplaceOldRetryHeaderIfExists()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Add(Headers.Retry, "99");

            var retryHeaderService = new AddRetryTrackingHeadersService();

            //act
            retryHeaderService.AddTrackingHeaders(rawMessage, 101);

            //assert
            rawMessage.Headers.Should()
                .Contain(new KeyValuePair<string, string>(Headers.Retry, "101"));
        }
    }
}