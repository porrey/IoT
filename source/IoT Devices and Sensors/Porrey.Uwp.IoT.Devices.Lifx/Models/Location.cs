using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class Location : NamedEntity, ISelector
	{
		public string GetSelectorText()
		{
			return string.Format("location_id:{1}", this.Id);
		}
	}
}
