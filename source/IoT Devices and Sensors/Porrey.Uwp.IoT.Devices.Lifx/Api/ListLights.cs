using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class ListLights : ApiMethod<IEnumerable<Light>>
	{
		public ListLights(ILifxIdentity identity)
			: base(identity, "lights/{0}")
		{
		}

		public async Task<IEnumerable<Light>> Get(ISelector selector)
		{
			return await this.Execute(selector.GetSelectorText());
		}

		protected async override Task<IEnumerable<Light>> OnExecuted(HttpResponseMessage response)
		{
			IEnumerable<Light> returnValue = new Light[0];

			string json = await response.Content.ReadAsStringAsync();
			returnValue = JsonConvert.DeserializeObject<IEnumerable<Light>>(json);

			return returnValue;
		}
	}
}
