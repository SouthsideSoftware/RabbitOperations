using System.Collections.Generic;
using System.Linq;
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
            Verify.RequireStringNotNullOrWhitespace(path, "path");

            Parts = new List<string>(path.Split('.').Select(x => x.Trim()).Where(x => x.Length > 0));
            
        }
        public IList<string> Parts { get; set; }
        public override string ToString()
        {
            return string.Join(".", Parts);
        }
    }
}