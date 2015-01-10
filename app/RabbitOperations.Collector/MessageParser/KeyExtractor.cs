using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json.Linq;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.MessageParser
{
    public class KeyExtractor : IKeyExtractor
    {
        private readonly ISettings settings;
        public KeyExtractor(ISettings settings)
        {
            Verify.RequireNotNull(settings, "settings");

            this.settings = settings;
        }

        public Dictionary<string, string> GetBusinessKeys(string body, string messageType)
        {
            var result = new Dictionary<string, string>();
            var messageTypeHandling = settings.MessageTypeHandlingFor(messageType);
            if (messageTypeHandling != null)
            {
                var jObject = JObject.Parse(body);
                var propertyNames = jObject.Properties().Select(p => p.Name).ToList();
                foreach (var jsonPath in messageTypeHandling.KeyPaths)
                {
                    foreach (var pathPart in jsonPath.Parts)
                    {
                        var matchingPropertyName = propertyNames.FirstOrDefault(pn => pn == pathPart);
                        if (matchingPropertyName != null)
                        {
                            var token = jObject[matchingPropertyName];
                            if (token is JValue)
                            {
                                result.Add(matchingPropertyName, token.ToString());
                            }
                            else
                            {
                                foreach (var value in jObject[matchingPropertyName])
                                {
                                    result.Add(matchingPropertyName, value.ToString());
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}