#define RavenInMemory
using System;
using System.Collections.Generic;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Listeners;

namespace SouthsideUtility.RavenDB.Testing
{
    /// <summary>
    ///     This class creates and initializes and Embeddable document store for RavenDB and is safe for use in unit tests
    ///     only.
    /// </summary>
    public class RavenTestDocumentStore
    {
        public IDocumentStore Store;

        public RavenTestDocumentStore(bool ravenInMemory = true)
        {
            if (ravenInMemory)
            {
                Store = new EmbeddableDocumentStore {RunInMemory = true};
                // Force queries to wait for indexes to catch up. Unit Testing only :P
                (Store as EmbeddableDocumentStore).RegisterListener(new NoStaleQueriesListener());
                Store.Initialize();
            }
            else
            {
                Store = new DocumentStore {ConnectionStringName = "RavenDB"};
                // Force queries to wait for indexes to catch up. Unit Testing only :P
                (Store as DocumentStore).RegisterListener(new NoStaleQueriesListener());
                Store.Initialize();
            }
        }

        internal class NoStaleQueriesListener : IDocumentQueryListener
        {
            public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
            {
                queryCustomization.WaitForNonStaleResults(TimeSpan.FromSeconds(60));
            }
        }
    }
}