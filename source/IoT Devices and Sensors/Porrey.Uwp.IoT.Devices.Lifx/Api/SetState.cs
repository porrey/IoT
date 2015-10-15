using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class SetState : ApiMethod<ApiResponse, SentState>
	{
		public SetState(ILifxIdentity identity)
			: base(identity, "lights/{0}/state", HttpMethod.Put)
		{
		}

		public async Task<ApiResponse> Set(ISelector selector, SentState state)
		{
			return await this.Execute(state, selector.GetSelectorText());
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
