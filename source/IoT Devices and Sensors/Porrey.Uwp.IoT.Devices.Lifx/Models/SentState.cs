using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class SentState
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "power")]
		public string Power { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "color")]
		public string Color { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "brightness")]
		public double Brightness { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "duration")]
		public double Duration { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "selector")]
		public string Selector { get; set; }
	}
}
