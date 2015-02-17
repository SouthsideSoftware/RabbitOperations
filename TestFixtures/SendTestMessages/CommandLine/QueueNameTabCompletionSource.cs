using System.Collections.Generic;
using PowerArgs;

namespace SendTestMessages.CommandLine
{
    public class QueueNameTabCompletionSource : SimpleTabCompletionSource
    {
        public QueueNameTabCompletionSource() : base(QueueNameTabCompletionSource.GetWords())
        {
        }

        private static IEnumerable<string> GetWords()
        {
            return new string[] {"audit", "error"};
        }
    }
}