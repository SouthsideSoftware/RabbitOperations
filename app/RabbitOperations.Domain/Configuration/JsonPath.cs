using System.Collections.Generic;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Domain.Configuration
{
    public class JsonPath
    {
        protected JsonPath()
        {
            Parts = new List<string>();
        }
        public JsonPath(string path)
        {
            Verify.RequireNotNull(path, "path");

            Parts = new List<string>(path.Split('.'));
            
        }
        public IList<string> Parts { get; set; }
        public override string ToString()
        {
            return string.Join(".", Parts);
        }
    }
}