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
	/// Defines the configuration bits for the MCP9808.
	/// </summary>
	public static class Mcp9808ConfigurationBit
	{
		/// <summary>
		/// Hysteresis Upper Bit
		/// </summary>
		public const int HysteresisUpperBit = 10;

		/// <summary>
		/// Hysteresis Lower Bit
		/// </summary>
		public const int HysteresisLowerBit = 9;

		/// <summary>
		/// Shutdown Mode
		/// </summary>
		public const int ShutdownMode = 8;

		/// <summary>
		/// Critical Lock
		/// </summary>
		public const int CriticalLock = 7;

		/// <summary>
		/// Window Lock
		/// </summary>
		public const int WindowLock = 6;

		/// <summary>
		/// Interrupt Clear
		/// </summary>
		public const int InterruptClear = 5;

		/// <summary>
		/// Alert Output Status
		/// </summary>
		public const int AlertOutputStatus = 4;

		/// <summary>
		/// Alert Output Control
		/// </summary>
		public const int AlertOutputControl = 3;

		/// <summary>
		/// Alert Output Select
		/// </summary>
		public const int AlertOutputSelect = 2;

		/// <summary>
		/// Alert Output Polarity
		/// </summary>
		public const int AlertOutputPolarity = 1;

		/// <summary>
		/// Alert Output Mode
		/// </summary>
		public const int AlertOutputMode = 0;
	}
}
