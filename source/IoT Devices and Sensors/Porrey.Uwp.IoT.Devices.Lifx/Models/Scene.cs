using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class Scene : NamedEntity, ISelector
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "uuid")]
		public Guid Uuid { get; set; } = Guid.Empty;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "account")]
		public Account Account { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "states")]
		public IEnumerable<RetrievedState> States { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "created_at")]
		public long CreatedAt { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "updated_at")]
		public long UpdatedAt { get; set; }

		public string GetSelectorText()
		{
			return string.Format("scene_id:{1}", this.Uuid.ToString());
		}
	}
}
