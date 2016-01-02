using RabbitOperations.Collector.Models;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.RavenDb.Query.Interfaces
{
    public interface IBasicSearch
    {
        SearchResult<MessageSearchResult> Search(SearchModel searchModel);
        MessageDocument Get(int id);
    }
}