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
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser.NServiceBus
{
    [TestFixture]
    public class HeaderParserTests
    {
        [Test]
        public void HeaderParserGetsProperNServiceBusHeadersFromAudit()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Audit.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            var headerParser = new RabbitOperations.Collector.MessageParser.NServiceBus.HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.Headers.ShouldBeEquivalentTo(new Dictionary<string, string>
            {
                {"MessageId", "33db72af-be36-4773-8c95-a41200279462"},
                {"CorrelationId", "33db72af-be36-4773-8c95-a41200279462"},
                {"MessageIntent", "Send"},
                {"Version", "5.0.3"},
                {"TimeSent", "2014-12-31 02:24:06:300190 Z"},
                {"ContentType", "application/json"},
                {"EnclosedMessageTypes", "Autobahn.Fulfillment.Contracts.Ordering.NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;Autobahn.Fulfillment.Contracts.Ordering.INotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;Veyron.Messages.Commands.ICommand, Veyron.Messages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},
                {"ConversationId", "84383232-6efc-4ca0-a304-a412002794da"},
                {"OriginatingMachine", "QA-1-WEB-1"},
                {"OriginatingEndpoint", "Ronaele.UI.Web"},
                {"RabbitMQ.CallbackQueue", "Autobahn.Fulfillment.Host.QA-1-WEB-1"},
                {"ReplyToAddress", "Ronaele.UI.Web"},
                {"InvokedSagas", "Autobahn.Fulfillment.Tasks.Sagas.FulfillmentSaga:3b654483-a8ea-470b-8c78-a4110184fa8c"},
                {"ProcessingStarted", "2014-12-31 02:24:06:794413 Z"},
                {"ProcessingEnded", "2014-12-31 02:24:07:074692 Z"},
                {"ProcessingMachine", "QA-1-WEB-1"},
                {"ProcessingEndpoint", "Autobahn.Fulfillment.Host"},
                {"OriginatingAddress", "Ronaele.UI.Web@QA-1-WEB-1"}
            });

        }
    }
}
