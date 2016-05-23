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
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace Porrey.Uwp.IoT.Sensors
{
	/// <summary>
	/// Defines an I2C interface for devices that can be created
	/// by the sensor factory.
	/// </summary>
	public interface II2c
	{
		I2cBusSpeed BusSpeed { get; }
		byte DeviceAddress { get; }
		bool IsInitialized { get; }

		Task<InitializationResult> InitializeAsync();
		Task<bool> ReadAsync(byte[] readBuffer);
		Task<byte[]> ReadBytesAsync(int bufferSize);
		Task<byte[]> ReadRegisterBytesAsync(byte registerId, int bufferSize);
		Task ResetAsync();
		Task<bool> WriteAsync(byte[] writeBuffer);
		Task<bool> WriteReadAsync(byte[] writeBuffer, byte[] readBuffer);
	}
}
