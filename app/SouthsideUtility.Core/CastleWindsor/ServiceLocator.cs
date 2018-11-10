using Castle.Windsor;

namespace SouthsideUtility.Core.CastleWindsor
{
    public static class ServiceLocator
    {
        static ServiceLocator()
        {
            Container = new WindsorContainer();
        }
        public static IWindsorContainer Container { get; private set; }

        public static void ClearContainer()
        {
            if (Container != null)
            {
                Container.Dispose();
            }
            Container = new WindsorContainer();
        }
    }
}
