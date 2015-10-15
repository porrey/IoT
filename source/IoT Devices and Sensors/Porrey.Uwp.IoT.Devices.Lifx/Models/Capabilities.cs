using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class Capabilities
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "has_color")]
		public bool HasColor { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "has_variable_color_temp")]
		public bool HasVariableColorTemp { get; set; }
	}
}
