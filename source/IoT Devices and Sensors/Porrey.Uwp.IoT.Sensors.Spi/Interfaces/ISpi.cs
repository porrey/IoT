// Copyright © 2015 Daniel Porrey. All Rights Reserved.
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
using System;
using System.Threading.Tasks;
using Windows.Devices.Spi;

namespace Porrey.Uwp.IoT.Sensors
{
	/// <summary>
	/// Defines an intergration point to the SPI interface.
	/// </summary>
	public interface ISpi : IDisposable
	{
		/// <summary>
		/// Gets the underlying SpiDevice instance used by this instance
		/// of Porrey.Uwp.IoT.Sensors.Spi.
		/// </summary>
		SpiDevice Device { get; }

		/// <summary>
		/// Gets the settings used on the SPI interface
		/// to communicate to the sensor.
		/// </summary>
		SpiConnectionSettings Settings { get; }

		/// <summary>
		/// Initializes the sensor by establishing a connection on the SPI interface.
		/// </summary>
		Task Initialize();
	}
}