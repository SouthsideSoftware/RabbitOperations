using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IActiveQueuePollers
    {
        IReadOnlyCollection<IQueuePoller> ActivePollers { get; }
        void Add(IQueuePoller queuePoller);
        void Remove(IQueuePoller queuePoller);
    }
}
