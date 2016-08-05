using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IActiveQueuePollers
    {
        IReadOnlyCollection<IApplicationPoller> ActivePollers { get; }
        void Add(IApplicationPoller applicationPoller);
        void Remove(IApplicationPoller applicationPoller);
    }
}
