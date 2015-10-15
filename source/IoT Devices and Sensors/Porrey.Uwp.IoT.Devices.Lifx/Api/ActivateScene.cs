using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class ActivateScene : ApiMethod<ApiResponse, Transition>
	{
		public ActivateScene(ILifxIdentity identity)
			: base(identity, "scenes/{0}/activate", HttpMethod.Put)
		{
		}

		public async Task<ApiResponse> Set(Scene scene, Transition transition)
		{
            return await this.Execute(transition, scene.GetSelectorText());
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
