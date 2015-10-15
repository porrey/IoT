using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class Transition
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "duration")]
		public double Duration { get; set; }
	}
}
