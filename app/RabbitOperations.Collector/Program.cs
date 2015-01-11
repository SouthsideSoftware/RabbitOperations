using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RabbitOperations.Collector.CastleWindsor;
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
            logger.Info("Starting app");
            HostFactory.Run(x =>                                 
            {
                x.Service<IMessageReader>(s =>                        
                {
                    s.ConstructUsing(name => ServiceLocator.Container.Resolve<IMessageReader>());     
                    s.WhenStarted(mr => mr.Start());              
                    s.WhenStopped(mr => mr.Stop());               
                });
                x.RunAsLocalSystem();                            

                x.SetDescription("RabbitOperations Collector");        
                x.SetDisplayName("RabbitOperations Collector");                       
                x.SetServiceName("RabbitOperations.Collector");
                x.StartAutomaticallyDelayed();
            }); 
        }
    }
}
