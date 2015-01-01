using System.Threading;

namespace RabbitOperations.Utility.TestableSystem.Interfaces
{
    public interface ICancellationTokenSource {
        void Cancel();
        CancellationToken Token { get; }
    }
}