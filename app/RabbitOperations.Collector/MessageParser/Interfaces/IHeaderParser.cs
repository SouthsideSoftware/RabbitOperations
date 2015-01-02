using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.MessageParser.Interfaces
{
    public interface IHeaderParser
    {
        void AddHeaderInformation(IRawMessage rawMessage, MessageDocument document);
    }
}
