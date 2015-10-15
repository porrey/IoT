using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class Selector
	{
		public static ISelector All
		{
			get
			{
				return new All();
			}
		}

		public static ISelector CreateList(params ISelector[] selectors)
		{
			SelectorList returnValue = new SelectorList();

			returnValue.AddRange(selectors);

			return returnValue;
		}
	}
}
