using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class SetStates : ApiMethod<IEnumerable<OperationResult>, Mix>
	{
		public SetStates(ILifxIdentity identity)
			: base(identity, "lights/states", HttpMethod.Put)
		{
		}

		public async Task<IEnumerable<OperationResult>> Set(Mix mix)
		{
			return await this.Execute(mix);
        }

		protected async override Task<IEnumerable<OperationResult>> OnExecuted(HttpResponseMessage response)
		{
			IEnumerable<OperationResult> returnValue = null;

			string json = await response.Content.ReadAsStringAsync();
			returnValue = JsonConvert.DeserializeObject<IEnumerable<OperationResult>>(json);

			return returnValue;
		}
	}
}
