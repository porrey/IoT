using System;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class Account
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "uuid")]
		public Guid Uuid { get; set; } = Guid.Empty;

		public override string ToString()
		{
			return this.Uuid.ToString();
		}
	}
}
