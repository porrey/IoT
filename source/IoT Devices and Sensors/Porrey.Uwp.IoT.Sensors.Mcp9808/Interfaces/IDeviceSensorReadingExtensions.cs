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
using System;

namespace Porrey.Uwp.IoT.Sensors
{
	/// <summary>
	/// Defines extensions for a IDeviceSensorReading instance.
	/// </summary>
	public static class IDeviceSensorReadingExtensions
	{
		/// <summary>
		/// Converts the temperature in an instance of IDeviceSensorReading
		/// from Celsius to Fahrenheit.
		/// </summary>
		/// <param name="sensorReading">An instance of IDeviceSensorReading.</param>
		/// <returns>Returns the temperature in Fahrenheit.</returns>
		public static float ToFahrenheit(this IDeviceSensorReading sensorReading) => Temperature.ConvertToFahrenheit(sensorReading.Temperature);
	}
}
