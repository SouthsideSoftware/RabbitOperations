using System.Collections.Generic;
using RabbitOperations.Collector.Service.Interfaces;

namespace RabbitOperations.Collector.Service
{
    public class RawMessage : IRawMessage
    {
        public Dictionary<string, string> Headers
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Data
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}