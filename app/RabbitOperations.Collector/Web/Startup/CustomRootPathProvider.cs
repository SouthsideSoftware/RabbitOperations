using System.IO;
using System.Reflection;
using Nancy;
using NLog;
using RabbitOperations.Collector.Configuration;

namespace RabbitOperations.Collector.Web.Startup
{
    public class CustomRootPathProvider : IRootPathProvider
    {
        private readonly string appLocation;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public CustomRootPathProvider()
        {
            //Web root points at web directory in project if source code is present otherwise
            //points at the web folder in the same location as the executable
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var devFileDirectory = Path.Combine(binDirectory, "..\\..\\Web");
            var devFile = Path.Combine(devFileDirectory, "Views\\layout.cshtml");
            appLocation = File.Exists(devFile) && Settings.StaticAllowDevelopmentMode ? Path.GetFullPath(devFileDirectory) : Path.Combine(binDirectory, "Web");
            logger.Info("The root of the web is {0}", appLocation);
        }

        public string GetRootPath()
        {
            return appLocation;
        }
    }
}