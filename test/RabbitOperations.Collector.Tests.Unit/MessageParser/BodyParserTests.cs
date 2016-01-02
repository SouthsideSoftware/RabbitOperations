using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using RabbitOperations.Domain;
using Xunit;
using Autofac.Extras.Moq;
using RabbitOperations.Collector.MessageParser;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    public class BodyParserTests
    {
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss:ffffffZ";
        private const string ApplicationJsonContentType = "application/json";

        [Fact]
        public void BodyParserCapturesRawBodyFromAudit()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("./TestData", "Audit.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            var mocker = AutoMock.GetLoose();
            var bodyParser = mocker.Create<BodyParser>();
            var doc = new MessageDocument();

            //act
            bodyParser.ParseBody(rawMessage, doc);

            //assert
            doc.Body.Should().Be("{\"CorrelationGuid\":\"f349702d-1be6-4c65-8f74-de8457ed4ccf\"}");
        }

        [Fact]
        public void BodyParserCapturesRawBodyFromError()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("./TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            var mocker = AutoMock.GetLoose();
            var bodyParser = mocker.Create<BodyParser>();
            var doc = new MessageDocument();

            //act
            bodyParser.ParseBody(rawMessage, doc);

            //assert
            doc.Body.Should().Be("{\"Ids\":[\"afecc831-34d4-47ca-b43b-56eb90d4e3b6\"]}");
        }

        [Fact]
        public void BodyParserHandlesMessagesWithEmptyBodies()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("./TestData", "EmptyBody.json")))
            {
                data = reader.ReadToEnd();
            }
            var mocker = AutoMock.GetLoose();
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            var bodyParser = mocker.Create<BodyParser>();
            var doc = new MessageDocument();

            //act
            bodyParser.ParseBody(rawMessage, doc);

            //assert
            doc.Body.Should().BeEmpty();
        }
    }
}