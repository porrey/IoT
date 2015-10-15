using System.Collections.Generic;
using System.Threading.Tasks;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class LifxApi
	{
		private LifxIdentityV1 _identity = null;

		public LifxApi(string token)
		{
			this.Identity = new LifxIdentityV1(token);
		}

		public LifxIdentityV1 Identity
		{
			get
			{
				return _identity;
			}
			private set
			{
				_identity = value;
			}
		}

		public async Task<IEnumerable<Light>> ListLights(ISelector selector)
		{
			ListLights api = new ListLights(this.Identity);
			return await api.Get(selector);
		}

		public async Task<IEnumerable<Scene>> ListScenes()
		{
			ListScenes api = new ListScenes(this.Identity);
			return await api.Get();
		}

		public async Task SetState(ISelector selector, SentState state)
		{
			SetState api = new SetState(this.Identity);
			await api.Set(selector, state);
		}

		public async Task SetStates(Mix mix)
		{
			SetStates api = new SetStates(this.Identity);
			await api.Set(mix);
		}

		public async Task TogglePower(ISelector selector, double duration)
		{
			TogglePower api = new TogglePower(this.Identity);
			await api.Set(selector, new Transition() { Duration = duration });
		}

		public async Task ActivateScene(Scene scene, double duration)
		{
			ActivateScene api = new ActivateScene(this.Identity);
			await api.Set(scene, new Transition() { Duration = duration });
		}

		public async Task<Color> ValidateColor(string colorText)
		{
			Color returnValue = null;

			ValidateColor api = new ValidateColor(this.Identity);
			returnValue = await api.Get(colorText);

			return returnValue;
		}

		public async Task Cycle(ISelector selector, Mix mix)
		{
			Cycle api = new Cycle(this.Identity);
			await api.Set(selector, mix);
		}
	}
}
