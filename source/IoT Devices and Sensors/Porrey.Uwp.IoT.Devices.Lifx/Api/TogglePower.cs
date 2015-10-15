using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class TogglePower : ApiMethod<ApiResponse, Transition>
	{
		public TogglePower(ILifxIdentity identity)
			: base(identity, "lights/{0}/toggle", HttpMethod.Post)
		{
		}

		public async Task<ApiResponse> Set(ISelector selector, Transition transition)
		{
            return await this.Execute(transition, selector.GetSelectorText());
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
