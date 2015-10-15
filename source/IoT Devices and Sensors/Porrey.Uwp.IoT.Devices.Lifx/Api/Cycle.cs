using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class Cycle : ApiMethod<ApiResponse, Mix>
	{
		public Cycle(ILifxIdentity identity)
			: base(identity, "lights/{0}/cycle", HttpMethod.Post)
		{
		}

		public async Task<ApiResponse> Set(ISelector selector, Mix mix)
		{
			return await this.Execute(mix, selector.GetSelectorText());
        }

		protected async override Task<ApiResponse> OnExecuted(HttpResponseMessage response)
		{
			ApiResponse returnValue = null;

			string json = await response.Content.ReadAsStringAsync();
			returnValue = JsonConvert.DeserializeObject<ApiResponse>(json);

			return returnValue;
		}
	}
}
