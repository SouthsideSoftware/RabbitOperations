using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SouthsideUtility.Testing
{
    public static class TestHelper
    {
        public static string FullPath(string fileName)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
        }
    }
}
