using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Web
{
    [ExcludeFromCodeCoverage] //just a thin wrapper around FileVersionInfo
    public static class VersionHelper
    {
        private static readonly string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string Version()
        {
            return version;
        }
    }

}
