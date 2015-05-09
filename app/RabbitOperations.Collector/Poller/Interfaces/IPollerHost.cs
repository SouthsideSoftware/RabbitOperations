using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitOperations.Collector.Poller.Interfaces
{
    public interface IPollerHost
    {
        void Start();
        void Stop();
        string Name { get; }
    }
}
