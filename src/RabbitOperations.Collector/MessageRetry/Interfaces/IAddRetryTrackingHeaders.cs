using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.MessageRetry.Interfaces
{
    public interface IAddRetryTrackingHeaders
    {
        void AddTrackingHeaders(IRawMessage rawMessage, long retryId);
    }
}
