using System;
using System.Collections.Generic;
using System.Linq;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class SelectorList : List<ISelector>, ISelector
	{
		public string GetSelectorText()
		{
			return String.Join(",", this.Select(t => t.GetSelectorText()).ToArray());
		}
	}
}
