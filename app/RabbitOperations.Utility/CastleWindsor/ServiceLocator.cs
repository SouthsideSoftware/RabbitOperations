using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;

namespace RabbitOperations.Utility.CastleWindsor
{
    public static class ServiceLocator
    {
        static ServiceLocator()
        {
            Container = new WindsorContainer();
        }
        public static IWindsorContainer Container { get; private set; }
    }
}
