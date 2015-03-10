using System.Collections.Generic;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.RavenDB.Query.Interfaces
{
    public interface IBasicSearch
    {
        IList<MessageDocument> Search(string searchString, int take, int page);
    }
}