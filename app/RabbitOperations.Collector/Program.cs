using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RabbitOperations.Collector.CastleWindsor;
using RabbitOperations.Collector.Host.Interfaces;
using SouthsideUtility.Core.CastleWindsor;
using Topshelf;
using RabbitOperations.Collector.Service.Interfaces;

namespace RabbitOperations.Collector
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            ServiceLocator.Container.Install(new Installer());
            logger.Info("Starting app in {0}", Environment.Is64BitProcess ? "64bit process" : "32bit process");
            HostFactory.Run(x =>                                 
            {
                x.Service<IHost>(s =>                        
                {
                    s.ConstructUsing(name => ServiceLocator.Container.Resolve<IHost>());     
                    s.WhenStarted(mr => mr.Start());              
                    s.WhenStopped(mr => mr.Stop());               
                });
                x.RunAsLocalSystem();                            

                x.SetDescription("RabbitOperations Collector");        
                x.SetDisplayName("RabbitOperations Collector");                       
                x.SetServiceName("RabbitOperations.Collector");
                x.StartAutomatically();
            }); 
        }
    }
}
