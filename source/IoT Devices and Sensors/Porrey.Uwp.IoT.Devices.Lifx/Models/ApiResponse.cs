using System.Collections.Generic;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class ApiResponse
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "results")]
		public IEnumerable<Result> Results { get; set; }
	}
}
