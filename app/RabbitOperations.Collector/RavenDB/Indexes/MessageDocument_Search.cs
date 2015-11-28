using System;
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
                    Any = new Object[]
                    {
                        messageDocument.Body,
                        messageDocument.MessageTypes.Select(x => x.ClassName),
                        messageDocument.Headers.Select(x => x.Value),
                        messageDocument.AdditionalErrorStatus,
                        messageDocument.ApplicationId,
                        messageDocument.Id
                    },
                    messageDocument.Id,
                    messageDocument.TimeSent,
                    ClassName = messageDocument.MessageTypes.Select(x => x.ClassName),
                    messageDocument.IsError,
                    messageDocument.ApplicationId,
                    Header = messageDocument.Headers.Select(x => x.Value),
                    messageDocument.AdditionalErrorStatus
                };

            Sort(x => x.Id, SortOptions.String);
            Index(x => x.Any, FieldIndexing.Analyzed);
            Analyzers.Add(x => x.Any, "StandardAnalyzer");
        }
    }
}