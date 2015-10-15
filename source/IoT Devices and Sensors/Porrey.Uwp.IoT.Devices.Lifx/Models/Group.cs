using System;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class Group : NamedEntity, ISelector
	{
		public string GetSelectorText()
		{
			return string.Format("group_id:{1}", this.Id);
		}
	}
}
