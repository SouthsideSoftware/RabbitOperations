using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace RabbitOperations.Collector
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("Starting app");
            while (1 == 1)
            {
                Thread.Sleep(100);
            }
        }
    }
}
