using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public abstract class NamedEntity : Entity
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "name")]
		public virtual string Name { get; set; }

		public override string ToString()
		{
			return this.Name;
		}
	}
}
