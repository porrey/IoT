using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class Result : Entity
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "label")]
		public string Label { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "status")]
		public string Status { get; set; }
	}
}
