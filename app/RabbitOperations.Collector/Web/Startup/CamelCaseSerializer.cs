using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RabbitOperations.Collector.Web.Startup
{
	public class CamelCaseSerializer : JsonSerializer { 
		public CamelCaseSerializer()
		{
			this.ContractResolver = new CamelCasePropertyNamesContractResolver();
			this.Formatting = Formatting.Indented;
		}
	}
}
