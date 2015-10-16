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
namespace Porrey.Uwp.IoT.Sensors
{
	/// <summary>
	/// Indicates that the SPI sensor has already been initialized
	/// and cannot be initialized again.
	/// </summary>
	public sealed class AlreadyInitializedException : SpiException
	{
		/// <summary>
		/// Initializes a new instance of the Porrey.Uwp.IoT.Sensors.AlreadyInitializedException class.
		/// </summary>
		public AlreadyInitializedException()
			: base("The SPI interface has already been initialized.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the Porrey.Uwp.IoT.Sensors.AlreadyInitializedException class.
		/// </summary>
		/// <param name="sensorName">The name of the sensor being initialized.</param>
		public AlreadyInitializedException(string sensorName)
			: base(string.Format("The {0} has already been initialized.", sensorName))
		{
		}
	}
}
