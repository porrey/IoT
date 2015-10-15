using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class ListScenes : ApiMethod<IEnumerable<Scene>>
	{
		public ListScenes(ILifxIdentity identity)
			: base(identity, "scenes")
		{
		}

		public async Task<IEnumerable<Scene>> Get()
		{
			return await this.Execute();
		}

		protected async override Task<IEnumerable<Scene>> OnExecuted(HttpResponseMessage response)
		{
			IEnumerable<Scene> returnValue = new Scene[0];

			string json = await response.Content.ReadAsStringAsync();
			returnValue = JsonConvert.DeserializeObject<IEnumerable<Scene>>(json);

			return returnValue;
		}
	}
}
