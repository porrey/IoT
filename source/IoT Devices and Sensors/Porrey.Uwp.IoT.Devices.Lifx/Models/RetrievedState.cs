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
using Newtonsoft.Json;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class RetrievedState
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "power")]
		public string Power { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "color")]
		public Color Color { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "brightness")]
		public double Brightness { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "duration")]
		public double Duration { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "selector")]
		public string Selector { get; set; }
	}
}
