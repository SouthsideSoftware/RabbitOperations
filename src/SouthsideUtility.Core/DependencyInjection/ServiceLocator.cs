using System;
using Autofac;
using Autofac.Features.GeneratedFactories;

namespace SouthsideUtility.Core.DependencyInjection
{
    public static class ServiceLocator
    {
        private static IContainer internalContainer;
        public static object locker = new object();

        public static IContainer Container
        {
            get { return internalContainer;}
            set
            {
                lock (locker)
                {
                    internalContainer = value;
                }
            }
        }
    }
}
