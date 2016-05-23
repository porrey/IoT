// Copyright © 2015-2106 Daniel Porrey. All Rights Reserved.
//
// This file is part of the IoT Devices and Sensors project.
// 
// IoT Devices and Sensors is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// IoT Devices and Sensors is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with IoT Devices and Sensors. If not, 
// see http://www.gnu.org/licenses/.
//
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
