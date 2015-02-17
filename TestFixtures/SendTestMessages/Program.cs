using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;
using SendTestMessages.CommandLine;

namespace SendTestMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            Args.InvokeMain<Arguments>(args);
        }
    }
}
