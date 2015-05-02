using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
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
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var retryHeaderService = new AddRetryTrackingHeadersService();

            //act
            retryHeaderService.AddTrackingHeaders(rawMessage, 101);

            //assert
            rawMessage.Headers.Should()
                .Contain(new KeyValuePair<string, string>(AddRetryTrackingHeadersService.RetryHeader, "101"));
        }

        [Test]
        public void ShouldReplaceOldRetryHeaderIfExists()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            rawMessage.Headers.Add(AddRetryTrackingHeadersService.RetryHeader, "99");

            var retryHeaderService = new AddRetryTrackingHeadersService();

            //act
            retryHeaderService.AddTrackingHeaders(rawMessage, 101);

            //assert
            rawMessage.Headers.Should()
                .Contain(new KeyValuePair<string, string>(AddRetryTrackingHeadersService.RetryHeader, "101"));
        }
    }
}
