using System;
using Autofac.Features.GeneratedFactories;

namespace SouthsideUtility.Core.DependencyInjection
{
    public static class ServiceLocator
    {
        private static IServiceProvider internalServiceProvider;
        public static object locker = new object();

        public static IServiceProvider ServiceProvider
        {
            get { return internalServiceProvider;}
            set
            {
                lock (locker)
                {
                    internalServiceProvider = value;
                }
            }
        }
    }
}
