using System;
using System.Globalization;

namespace RabbitOperations.Collector.MessageBusTechnologies.NServiceBus
{
    public static class Headers
    {
        public const string MessageType = "NServiceBus.EnclosedMessageTypes";
        public const string SagaInfo = "NServiceBus.InvokedSagas";
        public const string TimeSent = "NServiceBus.TimeSent";
        public const string DateTime = "yyyy-MM-dd HH:mm:ss:ffffff Z";
        public const string ContentType = "NServiceBus.ContentType";
        public const string ProcessingStarted = "NServiceBus.ProcessingStarted";
        public const string ProcessingEnded = "NServiceBus.ProcessingEnded";
        public const string TimeOfFailure = "NServiceBus.TimeOfFailure";
        public const string ExceptionType = "NServiceBus.ExceptionInfo.ExceptionType";
	    public const string FailedQ = "NServiceBus.FailedQ";
	    public const string ProcessingEndpoint = "NServiceBus.ProcessingEndpoint";
    }
}
