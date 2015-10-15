using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public abstract class Entity : IEntity
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "id")]
		public virtual string Id { get; set; }

		public override string ToString()
		{
			return this.Id;
		}
	}
}
