using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.MessageParser.Interfaces
{
    public interface IKeyExtractor
    {
        Dictionary<string, string> GetBusinessKeys(string body, string messageType);
    }
}
