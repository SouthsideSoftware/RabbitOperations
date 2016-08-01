using System.Collections.Generic;

namespace RabbitOperations.Collector.Models
{
	public class RetryDestinationResult
	{
		public RetryDestinationResult()
		{
			RetryIds = new List<long>();
			RetryDestinations = new List<string>();
		}

		public IList<long> RetryIds { get; set; }
		public IList<string> RetryDestinations { get; set; }
		public bool ForceRetry { get; set; }
	}
}