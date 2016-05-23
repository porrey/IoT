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
