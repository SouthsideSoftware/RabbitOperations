using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageParser.NServiceBus;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser.NServiceBus
{
    [TestFixture]
    public class HeaderParserTests
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss:ffffffZ";
        private const string ApplicationJsonContentType = "application/json";

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
            var headerParser = new HeaderParser();
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
                {
                    "EnclosedMessageTypes",
                    "Autobahn.Fulfillment.Contracts.Ordering.NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;Autobahn.Fulfillment.Contracts.Ordering.INotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;Veyron.Messages.Commands.ICommand, Veyron.Messages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                },
                {"ConversationId", "84383232-6efc-4ca0-a304-a412002794da"},
                {"OriginatingMachine", "QA-1-WEB-1"},
                {"OriginatingEndpoint", "Ronaele.UI.Web"},
                {"RabbitMQ.CallbackQueue", "Autobahn.Fulfillment.Host.QA-1-WEB-1"},
                {"ReplyToAddress", "Ronaele.UI.Web"},
                {
                    "InvokedSagas", "Autobahn.Fulfillment.Tasks.Sagas.FulfillmentSaga:3b654483-a8ea-470b-8c78-a4110184fa8c"
                },
                {"ProcessingStarted", "2014-12-31 02:24:06:794413 Z"},
                {"ProcessingEnded", "2014-12-31 02:24:07:074692 Z"},
                {"ProcessingMachine", "QA-1-WEB-1"},
                {"ProcessingEndpoint", "Autobahn.Fulfillment.Host"},
                {"OriginatingAddress", "Ronaele.UI.Web@QA-1-WEB-1"}
            });
        }

        [Test]
        public void HeaderParserGetsProperNServiceSagaInfoFromAudit()
        {
            //arrange
            var rawMessage = LoadRawMessage("Audit");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.SagaInfo.Should().NotBeNull();
            doc.SagaInfo.Class.Should().Be("Autobahn.Fulfillment.Tasks.Sagas.FulfillmentSaga");
            doc.SagaInfo.Key.Should().Be("3b654483-a8ea-470b-8c78-a4110184fa8c");
        }

        private static RawMessage LoadRawMessage(string fileName)
        {
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", string.Format("{0}.json", fileName))))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            return rawMessage;
        }

        [Test]
        public void HeaderParserHasNullSagaOnMessageNotInvolvingSaga()
        {
            //arrange
            var rawMessage = LoadRawMessage("Error");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.SagaInfo.Should().BeNull();
        }

        [Test]
        public void HeaderParserGetsProperNServiceBusMessageTypesAudit()
        {
            //arrange
            var rawMessage = LoadRawMessage("Audit");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.MessageTypes.ShouldBeEquivalentTo(new List<string>
            {
                "Autobahn.Fulfillment.Contracts.Ordering.NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                "Autobahn.Fulfillment.Contracts.Ordering.INotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                "Veyron.Messages.Commands.ICommand, Veyron.Messages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
            });
        }

        [Test]
        public void HeaderParserGetsProperNServiceBusMessageTypesError()
        {
            //arrange
            var rawMessage = LoadRawMessage("Error");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.MessageTypes.ShouldBeEquivalentTo(new List<string>
            {
                "Autobahn.Configurations.Contracts.Commands.ValidateConfigurations, Autobahn.Configurations.Contracts, Version=1.1.12.0, Culture=neutral, PublicKeyToken=null"
            });
        }

        [Test]
        public void HeaderParserGetsProperNServiceBusHeadersFromError()
        {
            //arrange
            var rawMessage = LoadRawMessage("Error");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.Headers.ShouldBeEquivalentTo(new Dictionary<string, string>
            {
                {"MessageId", "695742b4-58d0-4e3a-83a9-a4120116c48d"},
                {"CorrelationId", "695742b4-58d0-4e3a-83a9-a4120116c48d"},
                {"MessageIntent", "Send"},
                {"Version", "5.0.3"},
                {"TimeSent", "2014-12-31 16:54:57:747221 Z"},
                {"ContentType", "application/json"},
                {
                    "EnclosedMessageTypes",
                    "Autobahn.Configurations.Contracts.Commands.ValidateConfigurations, Autobahn.Configurations.Contracts, Version=1.1.12.0, Culture=neutral, PublicKeyToken=null"
                },
                {"ConversationId", "d95771ad-eb89-46b3-b8b3-a4120116c48d"},
                {"OriginatingMachine", "QA1-THD-WEB-1"},
                {"OriginatingEndpoint", "Autobahn.Configuration.WebAPI"},
                {"RabbitMQ.CallbackQueue", "Autobahn.Configuration.Host.QA1-APP-01"},
                {"ReplyToAddress", "Autobahn.Configuration.WebAPI"},
                {"ExceptionInfo.ExceptionType", "Raven.Abstractions.Exceptions.ConcurrencyException"},
                {
                    "ExceptionInfo.Message",
                    "PUT attempted on : ConfigurationResponseDocuments/afecc831-34d4-47ca-b43b-56eb90d4e3b6 while it is being locked by another transaction"
                },
                {"ExceptionInfo.Source", "Raven.Client.Lightweight"},
                {
                    "ExceptionInfo.StackTrace",
                    "   at Raven.Client.Connection.ServerClient.DirectBatch(IEnumerable`1 commandDatas, OperationMetadata operationMetadata)\r\n   at Raven.Client.Connection.ServerClient.<>c__DisplayClass97.<Batch>b__96(OperationMetadata u)\r\n   at Raven.Client.Connection.ReplicationInformer.TryOperation[T](Func`2 operation, OperationMetadata operationMetadata, OperationMetadata primaryOperationMetadata, Boolean avoidThrowing, T& result, Boolean& wasTimeout)\r\n   at Raven.Client.Connection.ReplicationInformer.ExecuteWithReplication[T](String method, String primaryUrl, OperationCredentials primaryCredentials, Int32 currentRequest, Int32 currentReadStripingBase, Func`2 operation)\r\n   at Raven.Client.Connection.ServerClient.ExecuteWithReplication[T](String method, Func`2 operation)\r\n   at Raven.Client.Connection.ServerClient.Batch(IEnumerable`1 commandDatas)\r\n   at Raven.Client.Document.DocumentSession.SaveChanges()\r\n   at Autobahn.Configurations.Tasks.Handlers.NServiceBus.ValidateConfigurationsHandler.Handle(ValidateConfigurations message) in z:\\BuildAgent\\Work\\fdf076c7251fe009\\app\\Autobahn.Configurations.Tasks\\Handlers\\NServiceBus\\ValidateConfigurationsHandler.cs:line 74\r\n   at lambda_method(Closure , Object , Object )\r\n   at NServiceBus.Unicast.MessageHandlerRegistry.Invoke(Object handler, Object message, Dictionary`2 dictionary) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\MessageHandlerRegistry.cs:line 126\r\n   at NServiceBus.Unicast.MessageHandlerRegistry.InvokeHandle(Object handler, Object message) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\MessageHandlerRegistry.cs:line 84\r\n   at NServiceBus.LoadHandlersBehavior.<Invoke>b__1(Object handlerInstance, Object message) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\LoadHandlersBehavior.cs:line 41\r\n   at NServiceBus.InvokeHandlersBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\InvokeHandlersBehavior.cs:line 24\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.SetCurrentMessageBeingHandledBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\SetCurrentMessageBeingHandledBehavior.cs:line 17\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.LoadHandlersBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\LoadHandlersBehavior.cs:line 46\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.ApplyIncomingMessageMutatorsBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\MessageMutator\\ApplyIncomingMessageMutatorsBehavior.cs:line 23\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.ExecuteLogicalMessagesBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Messages\\ExecuteLogicalMessagesBehavior.cs:line 24\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.CallbackInvocationBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\CallbackInvocationBehavior.cs:line 23\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.DeserializeLogicalMessagesBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Messages\\DeserializeLogicalMessagesBehavior.cs:line 49\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.ApplyIncomingTransportMessageMutatorsBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\MessageMutator\\ApplyIncomingTransportMessageMutatorsBehavior.cs:line 20\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.SubscriptionReceiverBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Subscriptions\\MessageDrivenSubscriptions\\SubscriptionReceiverBehavior.cs:line 31\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.UnitOfWorkBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\UnitOfWork\\UnitOfWorkBehavior.cs:line 43\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.Transports.RabbitMQ.OpenPublishChannelBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\41ea81d808fdfd62\\src\\NServiceBus.RabbitMQ\\OpenPublishChannelBehavior.cs:line 19\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.ChildContainerBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\ChildContainerBehavior.cs:line 17\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.ProcessingStatisticsBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Monitoring\\ProcessingStatisticsBehavior.cs:line 23\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.AuditBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Audit\\AuditBehavior.cs:line 20\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.Pipeline.PipelineExecutor.Execute[T](BehaviorChain`1 pipelineAction, T context) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\PipelineExecutor.cs:line 127\r\n   at NServiceBus.Pipeline.PipelineExecutor.InvokePipeline[TContext](IEnumerable`1 behaviors, TContext context) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\PipelineExecutor.cs:line 74\r\n   at NServiceBus.Pipeline.PipelineExecutor.InvokeReceivePhysicalMessagePipeline() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\PipelineExecutor.cs:line 100\r\n   at NServiceBus.Unicast.UnicastBus.TransportMessageReceived(Object sender, TransportMessageReceivedEventArgs e) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\UnicastBus.cs:line 826\r\n   at NServiceBus.Unicast.Transport.TransportReceiver.OnTransportMessageReceived(TransportMessage msg) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Transport\\TransportReceiver.cs:line 410\r\n   at NServiceBus.Unicast.Transport.TransportReceiver.ProcessMessage(TransportMessage message) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Transport\\TransportReceiver.cs:line 343\r\n   at NServiceBus.Unicast.Transport.TransportReceiver.TryProcess(TransportMessage message) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Transport\\TransportReceiver.cs:line 227\r\n   at NServiceBus.Transports.RabbitMQ.RabbitMqDequeueStrategy.ConsumeMessages(Object state) in c:\\BuildAgent\\work\\41ea81d808fdfd62\\src\\NServiceBus.RabbitMQ\\RabbitMqDequeueStrategy.cs:line 186"
                },
                {"FailedQ", "Autobahn.Configuration.Host@QA1-APP-01"},
                {"TimeOfFailure", "2014-12-31 16:56:02:891973 Z"},
                {"Retries.Timestamp", "2014-12-31 16:54:58:516940 Z"},
                {"Timeout.RouteExpiredTimeoutTo", "Autobahn.Configuration.Host@QA1-APP-02"},
                {"Timeout.Expire", "2014-12-31 16:56:01:204458 Z"}
            });
        }

        [Test]
        public void HeaderParserGetsProperBusTechnologyFromAudit()
        {
            //arrange
            var rawMessage = LoadRawMessage("Audit");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.BusTechnology.Should().Be("NServiceBus");
        }

        [Test]
        public void HeaderParserGetsProperBusTechnologyFromError()
        {
            //arrange
            var rawMessage = LoadRawMessage("Error");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.BusTechnology.Should().Be("NServiceBus");
        }

        [Test]
        public void HeaderParserGetsProperTimeSentFromAudit()
        {
            //arrange
            var rawMessage = LoadRawMessage("Audit");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.TimeSent.Should().Be(DateTime.ParseExact("2014-12-31T02:24:06:300190Z", DateTimeFormat, CultureInfo.InvariantCulture).ToUniversalTime());
        }

        [Test]
        public void HeaderParserGetsProperTimeSentFromError()
        {
            //arrange
            var rawMessage = LoadRawMessage("Error");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.TimeSent.Should().Be(DateTime.ParseExact("2014-12-31T16:54:57:747221Z", DateTimeFormat, CultureInfo.InvariantCulture).ToUniversalTime());
        }

        [Test]
        public void HeaderParserGetsProperContentTypeAudit()
        {
            //arrange
            var rawMessage = LoadRawMessage("Audit");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.ContentType.Should().Be(ApplicationJsonContentType);
        }

        [Test]
        public void HeaderParserGetsProperContentTypeError()
        {
            //arrange
            var rawMessage = LoadRawMessage("Error");
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.ContentType.Should().Be(ApplicationJsonContentType);
        }
    }
}