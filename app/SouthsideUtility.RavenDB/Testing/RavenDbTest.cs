using System;
using Raven.Client;

namespace SouthsideUtility.RavenDB.Testing
{
    public class RavenDbTest : IDisposable
    {
        private RavenTestDocumentStore noStaleQueryWrapper;

        protected internal RavenDbTest(bool ravenInMemory = true)
        {
            noStaleQueryWrapper = new RavenTestDocumentStore(ravenInMemory);
        }

        /// <summary>
        /// Gets the current document store.
        /// </summary>
        public IDocumentStore Store { get { return noStaleQueryWrapper.Store; } }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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