using System.Threading;

namespace SouthsideUtility.Core.TestableSystem.Interfaces
{
    public interface ICancellationTokenSource {
        void Cancel();
        CancellationToken Token { get; }
    }
}