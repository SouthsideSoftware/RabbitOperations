using System.Linq;
using RabbitOperations.Domain;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace RabbitOperations.Collector.RavenDB.Indexes
{
    public class MessageDocument_Search : AbstractIndexCreationTask<MessageDocument>
    {
        public MessageDocument_Search()
        {
            
            Map = messageDocuments => from messageDocument in messageDocuments
                select new
                {
                    Any = messageDocument.Body,
                    messageDocument.TimeSent
                };

            Index(x=> x.Any, FieldIndexing.Analyzed);
            Analyzers.Add(x => x.Any, "StandardAnalyzer");
        }
    }
}