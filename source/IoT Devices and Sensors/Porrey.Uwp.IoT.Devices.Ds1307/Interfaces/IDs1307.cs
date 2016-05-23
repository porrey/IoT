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
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Porrey.Uwp.IoT.Devices
{
	public interface IDs1307
	{
		Ds1307.Frequency OscillatorFrequency { get; set; }
		Ds1307.OscillatorOutputMode OscillatorMode { get; set; }
		GpioPinValue OscillatorValue { get; set; }

		Task<DateTimeOffset> GetAsync();
		Task Halt();
		Task<byte[]> ReadMemory();
		Task Resume();
		Task SetAsync(DateTimeOffset value);
		Task WriteMemory(byte[] data);
	}
}