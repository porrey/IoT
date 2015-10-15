using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class ValidateColor : ApiMethod<Color>
	{
		public ValidateColor(ILifxIdentity identity)
			: base(identity, "color?string={0}")
		{
		}

		public async Task<Color> Get(string colorText)
		{
			return await this.Execute(colorText);
		}

		protected async override Task<Color> OnExecuted(HttpResponseMessage response)
		{
			Color returnValue = null;

			string json = await response.Content.ReadAsStringAsync();
			returnValue = JsonConvert.DeserializeObject<Color>(json);

			return returnValue;
		}
	}
}
