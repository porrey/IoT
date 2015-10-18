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
namespace System
{
	/// <summary>
	/// Helper methods for temperature conversion
	/// </summary>
	public static class Temperature
	{
		/// <summary>
		/// Converts a temperature in Celsius to Fahrenheit.
		/// </summary>
		/// <param name="fahrenheit">The temperature in Celsius.</param>
		/// <returns>Returns the temperature in Fahrenheit.</returns>
		public static float ConvertToCelsius(this float fahrenheit) => (fahrenheit - 32f) * 5f / 9f;

		/// <summary>
		/// Converts a temperature in Fahrenheit to Celsius.
		/// </summary>
		/// <param name="celsius">The temperature in Fahrenheit.</param>
		/// <returns>Returns the temperature in Celsius.</returns>
		public static float ConvertToFahrenheit(this float celsius) => (celsius * 9f / 5f) + 32f;
	}
}
