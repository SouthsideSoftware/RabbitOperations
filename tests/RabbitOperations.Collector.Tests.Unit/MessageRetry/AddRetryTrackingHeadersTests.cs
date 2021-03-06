﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RabbitOperations.Collector.MessageBusTechnologies.Common;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageRetry;

namespace RabbitOperations.Collector.Tests.Unit.MessageRetry
{
    [TestFixture]
    public class AddRetryTrackingHeadersTests
    {
        [Test]
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

        [Test]
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
