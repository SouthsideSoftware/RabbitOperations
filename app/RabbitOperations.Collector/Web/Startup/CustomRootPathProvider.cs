using System.IO;
using System.Reflection;
using Nancy;

namespace RabbitOperations.Collector.Web.Startup
{
    public class CustomRootPathProvider : IRootPathProvider
    {
        private static readonly string appLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Web");
        public string GetRootPath()
        {
            return appLocation;
        }
    }
}