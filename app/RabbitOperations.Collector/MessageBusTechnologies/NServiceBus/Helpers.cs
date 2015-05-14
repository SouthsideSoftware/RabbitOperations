using System;
using System.Globalization;

namespace RabbitOperations.Collector.MessageBusTechnologies.NServiceBus
{
    public static class Helpers
    {
        public static DateTime ToUniversalDateTime(string nServiceBusDateTime)
        {
            return System.DateTime.ParseExact(nServiceBusDateTime, Headers.DateTime, CultureInfo.InvariantCulture)
                .ToUniversalTime();
        }

        public static string ToNServiceBusDateTime(DateTime dateTime)
        {
            return dateTime.ToString(Headers.DateTime);
        }
    }
}