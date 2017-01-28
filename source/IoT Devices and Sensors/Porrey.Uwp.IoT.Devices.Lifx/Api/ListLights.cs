// Copyright © 2015-2017 Daniel Porrey. All Rights Reserved.
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
