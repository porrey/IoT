using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class Color : NamedEntity
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "hue")]
		public double? Hue { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "saturation")]
		public string Saturation { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "brightness")]
		public double? Brightness { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "kelvin")]
		public int? Kelvin { get; set; }

		public override string ToString()
		{
			return string.Format("{0:0.00}, {1:0.00}, {2}", this.Hue, this.Saturation, this.Kelvin);
		}
	}
}
