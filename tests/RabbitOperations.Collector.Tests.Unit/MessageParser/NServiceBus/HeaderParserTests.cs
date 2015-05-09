using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageParser.NServiceBus;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser.NServiceBus
{
    [TestFixture]
    public class HeaderParserTests
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss:ffffffZ";
        private const string ApplicationJsonContentType = "application/json";

        [Test]
        public void HeaderParserGetsProperHeadersFromAudit()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.Headers.ShouldBeEquivalentTo(new Dictionary<string, string>
            {
                {"NServiceBus.MessageId", "33db72af-be36-4773-8c95-a41200279462"},
                {"NServiceBus.CorrelationId", "33db72af-be36-4773-8c95-a41200279462"},
                {"NServiceBus.MessageIntent", "Send"},
                {"NServiceBus.Version", "5.0.3"},
                {"NServiceBus.TimeSent", "2014-12-31 02:24:06:300190 Z"},
                {"NServiceBus.ContentType", "application/json"},
                {
                    "NServiceBus.EnclosedMessageTypes",
                    "Autobahn.Fulfillment.Contracts.Ordering.NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;Autobahn.Fulfillment.Contracts.Ordering.INotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;Veyron.Messages.Commands.ICommand, Veyron.Messages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                },
                {"NServiceBus.ConversationId", "84383232-6efc-4ca0-a304-a412002794da"},
                {"NServiceBus.OriginatingMachine", "QA-1-WEB-1"},
                {"NServiceBus.OriginatingEndpoint", "Ronaele.UI.Web"},
                {"NServiceBus.RabbitMQ.CallbackQueue", "Autobahn.Fulfillment.Host.QA-1-WEB-1"},
                {"NServiceBus.ReplyToAddress", "Ronaele.UI.Web"},
                {
                    "NServiceBus.InvokedSagas", "Autobahn.Fulfillment.Tasks.Sagas.FulfillmentSaga:3b654483-a8ea-470b-8c78-a4110184fa8c"
                },
                {"NServiceBus.ProcessingStarted", "2014-12-31 02:24:06:794413 Z"},
                {"NServiceBus.ProcessingEnded", "2014-12-31 02:24:07:074692 Z"},
                {"NServiceBus.ProcessingMachine", "QA-1-WEB-1"},
                {"NServiceBus.ProcessingEndpoint", "Autobahn.Fulfillment.Host"},
                {"NServiceBus.OriginatingAddress", "Ronaele.UI.Web@QA-1-WEB-1"},
                {"WinIdName", "GCC-DEV\\developer"},
                {"$.diagnostics.hostid","0ba7c10aa289498b4d9d6936935f51f7"},
                {"$.diagnostics.hostdisplayname", "QA-1-WEB-1"},
                {"$.diagnostics.license.expired", "false"},
                {"$.diagnostics.originating.hostid","6da45a72687753278b930fe38a91a4ad"}

            });
        }

        [Test]
        public void HeaderParserGetsProperNServiceSagaInfoFromAudit()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.SagaInfo.Should().NotBeNull();
            doc.SagaInfo.Class.Should().Be("Autobahn.Fulfillment.Tasks.Sagas.FulfillmentSaga");
            doc.SagaInfo.Key.Should().Be("3b654483-a8ea-470b-8c78-a4110184fa8c");
        }

        [Test]
        public void HeaderParserHasNullSagaOnMessageNotInvolvingSaga()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
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
            var rawMessage = MessageTestHelpers.GetAuditMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.MessageTypes.ShouldBeEquivalentTo(new List<TypeName>
            {
                new TypeName("Autobahn.Fulfillment.Contracts.Ordering.NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
                 new TypeName("Autobahn.Fulfillment.Contracts.Ordering.INotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
                 new TypeName("Veyron.Messages.Commands.ICommand, Veyron.Messages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")
            });
        }

        [Test]
        public void HeaderParserGetsProperNServiceBusMessageTypesError()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.MessageTypes.ShouldBeEquivalentTo(new List<TypeName>
            {
                 new TypeName("Autobahn.Configurations.Contracts.Commands.ValidateConfigurations, Autobahn.Configurations.Contracts, Version=1.1.12.0, Culture=neutral, PublicKeyToken=null")
            });
        }

        [Test]
        public void HeaderParserGetsProperHeadersFromError()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.Headers.ShouldBeEquivalentTo(new Dictionary<string, string>
            {
                {"NServiceBus.MessageId", "695742b4-58d0-4e3a-83a9-a4120116c48d"},
                {"NServiceBus.CorrelationId", "695742b4-58d0-4e3a-83a9-a4120116c48d"},
                {"NServiceBus.MessageIntent", "Send"},
                {"NServiceBus.Version", "5.0.3"},
                {"NServiceBus.TimeSent", "2014-12-31 16:54:57:747221 Z"},
                {"NServiceBus.ContentType", "application/json"},
                {
                    "NServiceBus.EnclosedMessageTypes",
                    "Autobahn.Configurations.Contracts.Commands.ValidateConfigurations, Autobahn.Configurations.Contracts, Version=1.1.12.0, Culture=neutral, PublicKeyToken=null"
                },
                {"NServiceBus.ConversationId", "d95771ad-eb89-46b3-b8b3-a4120116c48d"},
                {"NServiceBus.OriginatingMachine", "QA1-THD-WEB-1"},
                {"NServiceBus.OriginatingEndpoint", "Autobahn.Configuration.WebAPI"},
                {"NServiceBus.RabbitMQ.CallbackQueue", "Autobahn.Configuration.Host.QA1-APP-01"},
                {"NServiceBus.ReplyToAddress", "Autobahn.Configuration.WebAPI"},
                {"NServiceBus.ExceptionInfo.ExceptionType", "Raven.Abstractions.Exceptions.ConcurrencyException"},
                {
                    "NServiceBus.ExceptionInfo.Message",
                    "PUT attempted on : ConfigurationResponseDocuments/afecc831-34d4-47ca-b43b-56eb90d4e3b6 while it is being locked by another transaction"
                },
                {"NServiceBus.ExceptionInfo.Source", "Raven.Client.Lightweight"},
                {
                    "NServiceBus.ExceptionInfo.StackTrace",
                    "   at Raven.Client.Connection.ServerClient.DirectBatch(IEnumerable`1 commandDatas, OperationMetadata operationMetadata)\r\n   at Raven.Client.Connection.ServerClient.<>c__DisplayClass97.<Batch>b__96(OperationMetadata u)\r\n   at Raven.Client.Connection.ReplicationInformer.TryOperation[T](Func`2 operation, OperationMetadata operationMetadata, OperationMetadata primaryOperationMetadata, Boolean avoidThrowing, T& result, Boolean& wasTimeout)\r\n   at Raven.Client.Connection.ReplicationInformer.ExecuteWithReplication[T](String method, String primaryUrl, OperationCredentials primaryCredentials, Int32 currentRequest, Int32 currentReadStripingBase, Func`2 operation)\r\n   at Raven.Client.Connection.ServerClient.ExecuteWithReplication[T](String method, Func`2 operation)\r\n   at Raven.Client.Connection.ServerClient.Batch(IEnumerable`1 commandDatas)\r\n   at Raven.Client.Document.DocumentSession.SaveChanges()\r\n   at Autobahn.Configurations.Tasks.Handlers.NServiceBus.ValidateConfigurationsHandler.Handle(ValidateConfigurations message) in z:\\BuildAgent\\Work\\fdf076c7251fe009\\app\\Autobahn.Configurations.Tasks\\Handlers\\NServiceBus\\ValidateConfigurationsHandler.cs:line 74\r\n   at lambda_method(Closure , Object , Object )\r\n   at NServiceBus.Unicast.MessageHandlerRegistry.Invoke(Object handler, Object message, Dictionary`2 dictionary) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\MessageHandlerRegistry.cs:line 126\r\n   at NServiceBus.Unicast.MessageHandlerRegistry.InvokeHandle(Object handler, Object message) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\MessageHandlerRegistry.cs:line 84\r\n   at NServiceBus.LoadHandlersBehavior.<Invoke>b__1(Object handlerInstance, Object message) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\LoadHandlersBehavior.cs:line 41\r\n   at NServiceBus.InvokeHandlersBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\InvokeHandlersBehavior.cs:line 24\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.SetCurrentMessageBeingHandledBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\SetCurrentMessageBeingHandledBehavior.cs:line 17\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.LoadHandlersBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\LoadHandlersBehavior.cs:line 46\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.ApplyIncomingMessageMutatorsBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\MessageMutator\\ApplyIncomingMessageMutatorsBehavior.cs:line 23\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.ExecuteLogicalMessagesBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Messages\\ExecuteLogicalMessagesBehavior.cs:line 24\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.CallbackInvocationBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\CallbackInvocationBehavior.cs:line 23\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.DeserializeLogicalMessagesBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Messages\\DeserializeLogicalMessagesBehavior.cs:line 49\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.ApplyIncomingTransportMessageMutatorsBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\MessageMutator\\ApplyIncomingTransportMessageMutatorsBehavior.cs:line 20\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.SubscriptionReceiverBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Subscriptions\\MessageDrivenSubscriptions\\SubscriptionReceiverBehavior.cs:line 31\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.UnitOfWorkBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\UnitOfWork\\UnitOfWorkBehavior.cs:line 43\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.Transports.RabbitMQ.OpenPublishChannelBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\41ea81d808fdfd62\\src\\NServiceBus.RabbitMQ\\OpenPublishChannelBehavior.cs:line 19\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.ChildContainerBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Behaviors\\ChildContainerBehavior.cs:line 17\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.ProcessingStatisticsBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Monitoring\\ProcessingStatisticsBehavior.cs:line 23\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.AuditBehavior.Invoke(IncomingContext context, Action next) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Audit\\AuditBehavior.cs:line 20\r\n   at NServiceBus.BehaviorChain`1.Invoke() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\BehaviorChain.cs:line 39\r\n   at NServiceBus.Pipeline.PipelineExecutor.Execute[T](BehaviorChain`1 pipelineAction, T context) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\PipelineExecutor.cs:line 127\r\n   at NServiceBus.Pipeline.PipelineExecutor.InvokePipeline[TContext](IEnumerable`1 behaviors, TContext context) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\PipelineExecutor.cs:line 74\r\n   at NServiceBus.Pipeline.PipelineExecutor.InvokeReceivePhysicalMessagePipeline() in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Pipeline\\PipelineExecutor.cs:line 100\r\n   at NServiceBus.Unicast.UnicastBus.TransportMessageReceived(Object sender, TransportMessageReceivedEventArgs e) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\UnicastBus.cs:line 826\r\n   at NServiceBus.Unicast.Transport.TransportReceiver.OnTransportMessageReceived(TransportMessage msg) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Transport\\TransportReceiver.cs:line 410\r\n   at NServiceBus.Unicast.Transport.TransportReceiver.ProcessMessage(TransportMessage message) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Transport\\TransportReceiver.cs:line 343\r\n   at NServiceBus.Unicast.Transport.TransportReceiver.TryProcess(TransportMessage message) in c:\\BuildAgent\\work\\1b05a2fea6e4cd32\\src\\NServiceBus.Core\\Unicast\\Transport\\TransportReceiver.cs:line 227\r\n   at NServiceBus.Transports.RabbitMQ.RabbitMqDequeueStrategy.ConsumeMessages(Object state) in c:\\BuildAgent\\work\\41ea81d808fdfd62\\src\\NServiceBus.RabbitMQ\\RabbitMqDequeueStrategy.cs:line 186"
                },
                {"NServiceBus.FailedQ", "Autobahn.Configuration.Host@QA1-APP-01"},
                {"NServiceBus.TimeOfFailure", "2014-12-31 16:56:02:891973 Z"},
                {"NServiceBus.Retries.Timestamp", "2014-12-31 16:54:58:516940 Z"},
                {"NServiceBus.Timeout.RouteExpiredTimeoutTo", "Autobahn.Configuration.Host@QA1-APP-02"},
                {"NServiceBus.Timeout.Expire", "2014-12-31 16:56:01:204458 Z"},
                {"WinIdName", ""},
                {"$.diagnostics.hostid","dd0264d6ebaaa79264f4875f0cd3cc9c"},
                {"$.diagnostics.hostdisplayname", "QA1-APP-01"},
                {"$.diagnostics.originating.hostid","e2ee89227f61d89c49f32dec35116165"}
            });
        }

        [Test]
        public void HeaderParserGetsProperBusTechnologyFromAudit()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();
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
            var rawMessage = MessageTestHelpers.GetErrorMessage();
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
            var rawMessage = MessageTestHelpers.GetAuditMessage();
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
            var rawMessage = MessageTestHelpers.GetErrorMessage();
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
            var rawMessage = MessageTestHelpers.GetAuditMessage();
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
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.ContentType.Should().Be(ApplicationJsonContentType);
        }

        [Test]
        public void HeaderParserGetsProperProcessingTimeFromAudit()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            var processingStarted =
                DateTime.ParseExact("2014-12-31T02:24:06:794413Z", DateTimeFormat, CultureInfo.InvariantCulture)
                    .ToUniversalTime();
            var processingEnded =
                DateTime.ParseExact("2014-12-31T02:24:07:074692Z", DateTimeFormat, CultureInfo.InvariantCulture)
                    .ToUniversalTime();
            doc.ProcessingTime.Should().Be(processingEnded - processingStarted);
        }

        [Test]
        public void HeaderParserGetsProperProcessingTimeFromError()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.ProcessingTime.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void HeaderParserGetsProperIsErrorFromAudit()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.IsError.Should().BeFalse();
        }

        [Test]
        public void HeaderParserGetsProperIsErrorFromError()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.IsError.Should().BeTrue();
        }

        [Test]
        public void HeaderParserGetsProperAdditionalErrorStatusFromAudit()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.NotAnError);
        }

        [Test]
        public void HeaderParserGetsProperAdditionalErrorStatusFromError()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.Unresolved);
        }

        [Test]
        public void HeaderParserGetsProperAdditionalErrorStatusFromErrorThatIsRetry()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();
            doc.Headers.Add(AddRetryTrackingHeadersService.RetryHeader, "foo");

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.IsRetry);
        }

        [Test]
        public void HeaderParserGetsProperAdditionalErrorStatusFromAuditThatIsRetry()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();
            doc.Headers.Add(AddRetryTrackingHeadersService.RetryHeader, "foo");

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.IsRetry);
        }

        [Test]
        public void HeaderParserGetsProperTotalTimeFromAudit()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            var timeSent =
                DateTime.ParseExact("2014-12-31T02:24:06:300190Z", DateTimeFormat, CultureInfo.InvariantCulture)
                    .ToUniversalTime();
            var processingEnded =
                DateTime.ParseExact("2014-12-31T02:24:07:074692Z", DateTimeFormat, CultureInfo.InvariantCulture)
                    .ToUniversalTime();
            doc.TotalTime.Should().Be(processingEnded - timeSent);
        }

        [Test]
        public void HeaderParserGetsProperTotalTimeFromError()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var headerParser = new HeaderParser();
            var doc = new MessageDocument();

            //act
            headerParser.AddHeaderInformation(rawMessage, doc);

            //assert
            var timeSent =
                DateTime.ParseExact("2014-12-31T16:54:57:747221Z", DateTimeFormat, CultureInfo.InvariantCulture)
                    .ToUniversalTime();
            var timeOfFailure =
                DateTime.ParseExact("2014-12-31T16:56:02:891973Z", DateTimeFormat, CultureInfo.InvariantCulture)
                    .ToUniversalTime();
            doc.TotalTime.Should().Be(timeOfFailure - timeSent);
        }
    }
}