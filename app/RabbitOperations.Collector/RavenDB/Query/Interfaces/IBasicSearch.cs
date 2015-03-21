using System.Collections.Generic;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.RavenDB.Query.Interfaces
{
    public interface IBasicSearch
    {
        SearchResult<MessageDocument> Search(string searchString, int take, int page);
    }
}