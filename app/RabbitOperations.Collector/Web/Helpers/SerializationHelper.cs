using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace RabbitOperations.Collector.Web.Helpers
{
	public static class SerializationHelper
	{
		public static void CaseSensitiveJson(this Response res, dynamic obj, bool useJavaScriptNaming = true)
		{
			var serializer = new JsonSerializer();

			res.WithContentType("application/json")
				.WithStatusCode(HttpStatusCode.OK)
				.Contents(JObject.FromObject(obj, serializer).ToString());
		}
	}
}
