using System;
using Raven.Client;
using RabbitOperations.Collector.Configuration;

namespace RabbitOperations.Collector.Tests.Unit.RavenDb
{
    public class RavenDbTest : IDisposable
    {
        private RavenTestDocumentStore noStaleQueryWrapper;

        protected internal RavenDbTest(bool ravenInMemory = true)
        {
            RavenDbSettingsHelper.TestInstance = new RavenDbSettings
            {
                DefaultTenant = "RabbitOperations"
            };
      
            noStaleQueryWrapper = new RavenTestDocumentStore(ravenInMemory);
            Store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(RavenDbSettingsHelper.Instance.DefaultTenant);
        }

        /// <summary>
        ///     Gets the current document store.
        /// </summary>
        public IDocumentStore Store
        {
            get { return noStaleQueryWrapper.Store; }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (noStaleQueryWrapper != null)
                {
                    Store.Dispose();
                    noStaleQueryWrapper = null;
                }
            }
        }
    }
}