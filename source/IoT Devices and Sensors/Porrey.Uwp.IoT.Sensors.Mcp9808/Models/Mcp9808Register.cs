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
namespace Porrey.Uwp.IoT.Sensors
{
	/// <summary>
	/// Defines the register locations for the MCP9808.
	/// </summary>
	public static class Mcp9808Register
	{
		/// <summary>
		/// Configuration
		/// </summary>
		public const byte Configuration = 0x01;

		/// <summary>
		/// Upper Temperature
		/// </summary>
		public const byte UpperTemperature = 0x02;

		/// <summary>
		/// Lower Temperature
		/// </summary>
		public const byte LowerTemperature = 0x03;

		/// <summary>
		/// Critical Temperature
		/// </summary>
		public const byte CriticalTemperature = 0x04;

		/// <summary>
		/// Ambient Temperature
		/// </summary>
		public const byte AmbientTemperature = 0x05;

		/// <summary>
		/// Manufacturer ID
		/// </summary>
		public const byte ManufacturerId = 0x06;

		/// <summary>
		/// Device ID
		/// </summary>
		public const byte DeviceId = 0x07;

		/// <summary>
		/// Resolution
		/// </summary>
		public const byte Resolution = 0x08;
	}
}
