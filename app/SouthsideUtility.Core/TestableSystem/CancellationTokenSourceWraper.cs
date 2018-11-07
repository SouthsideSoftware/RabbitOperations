using System.Threading;
using SouthsideUtility.Core.TestableSystem.Interfaces;

namespace SouthsideUtility.Core.TestableSystem
{
    public class CancellationTokenSourceWrapper : ICancellationTokenSource
    {
        private readonly CancellationTokenSource source;
        public CancellationTokenSourceWrapper(CancellationTokenSource source)
        {
            this.source = source;
        }

        public CancellationToken Token { get { return source.Token; } }

        public void Cancel()
        {
            source.Cancel();
        }
    }
}
