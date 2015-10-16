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
using System.Linq;
using System.Threading.Tasks;
using Porrey.Uwp.IoT.Sensors.I2C;
using Windows.Devices.I2c;

namespace Porrey.Uwp.IoT.Sensors
{
	public class Htu21df : I2c, IHtu21df
	{
		public enum Resolution
		{
			Rh12Temp14,
			Rh8Temp12,
			Rh10Temp13,
			Rh11Temp11
		}

		public enum BatteryStatus
		{
			Unknown,
			Good,
			Low
		}

		public enum HeaterStatus
		{
			On,
			Off
		}

		#region Constants
		private const byte I2C_ADDRESS = 0x40;
		private const byte READ_TEMP_HOLD = 0xE3;
		private const byte READ_HUMIDITY_HOLD = 0xE5;
		private const byte READ_TEMP_NOHOLD = 0xF3;
		private const byte READ_HUMIDITY_NOHOLD = 0xF5;
		private const byte WRITE_REGISTER = 0xE6;
		private const byte READ_REGISTER = 0xE7;
		private const byte RESET = 0xFE;

		private const int REGISTER_BIT_RESOLUTION_HIGH = 7;
		private const int REGISTER_BIT_BATTERY = 6;
		private const int REGISTER_BIT_RESERVED1 = 5;
		private const int REGISTER_BIT_RESERVED2 = 4;
		private const int REGISTER_BIT_RESERVED3 = 3;
		private const int REGISTER_BIT_HEATER = 2;
		private const int REGISTER_BIT_OTP = 1;
		private const int REGISTER_BIT_RESOLUTION_LOW = 0;
		#endregion

		public Htu21df() : base(I2C_ADDRESS, I2cBusSpeed.StandardMode)
		{
		}

		protected async override Task OnInitializeAsync()
		{
			await this.ResetAsync();
		}

		protected async override Task OnResetAsync()
		{
			await this.WriteAsync(new byte[] { RESET });
		}

		public async Task<float> ReadTemperatureAsync()
		{
			float returnValue = 0f;

			byte[] writeBuffer = new byte[] { READ_TEMP_HOLD };
			byte[] readBuffer = new byte[] { 0, 0 };

			await this.WriteAsync(writeBuffer);
			await Task.Delay(50);
			await this.ReadAsync(readBuffer);

			int reading = BitConverter.ToInt16(readBuffer.Reverse().ToArray(), 0);

			returnValue = -46.85f + 175.72f * (reading / 65536f);

			return returnValue;
		}

		public async Task<float> ReadHumidityAsync()
		{
			float returnValue = 0f;

			byte[] writeBuffer = new byte[] { READ_HUMIDITY_HOLD };
			byte[] readBuffer = new byte[] { 0, 0 };

			await this.WriteAsync(writeBuffer);
			await Task.Delay(50);
			await this.ReadAsync(readBuffer);

			int reading = BitConverter.ToInt16(readBuffer.Reverse().ToArray(), 0);

			returnValue = -6f + 125f * (reading / 65536f);

			return returnValue;
		}

		public async Task<Resolution> GetResolution()
		{
			Resolution returnValue = Resolution.Rh12Temp14;

			// ***
			// *** Get the current register
			// ***
			byte register = await this.GetRegister();

			if (RegisterConverter.BitIsLow(register, REGISTER_BIT_RESOLUTION_HIGH) &&
				RegisterConverter.BitIsLow(register, REGISTER_BIT_RESOLUTION_LOW))
			{
				returnValue = Resolution.Rh12Temp14;
			}
			else if (RegisterConverter.BitIsLow(register, REGISTER_BIT_RESOLUTION_HIGH) &&
					 RegisterConverter.BitIsHigh(register, REGISTER_BIT_RESOLUTION_LOW))
			{
				returnValue = Resolution.Rh8Temp12;
			}
			else if (RegisterConverter.BitIsHigh(register, REGISTER_BIT_RESOLUTION_HIGH) &&
					 RegisterConverter.BitIsLow(register, REGISTER_BIT_RESOLUTION_LOW))
			{
				returnValue = Resolution.Rh10Temp13;
			}
			else if (RegisterConverter.BitIsHigh(register, REGISTER_BIT_RESOLUTION_HIGH) &&
					 RegisterConverter.BitIsHigh(register, REGISTER_BIT_RESOLUTION_LOW))
			{
				returnValue = Resolution.Rh11Temp11;
			}

			return returnValue;
		}

		public async Task<bool> SetResolution(Resolution resolution)
		{
			bool returnValue = false;

			// ***
			// *** Get the current register
			// ***
			byte register = await this.GetRegister();

			// ***
			// *** Set the bits
			// ***
			register = RegisterConverter.SetBit(register, REGISTER_BIT_RESOLUTION_HIGH, resolution == Resolution.Rh10Temp13 || resolution == Resolution.Rh11Temp11);
			register = RegisterConverter.SetBit(register, REGISTER_BIT_RESOLUTION_LOW, resolution == Resolution.Rh8Temp12 || resolution == Resolution.Rh11Temp11);

			// ***
			// *** Write the register
			// ***
			await this.WriteRegister(register);

			// ***
			// *** Check the register
			// ***
			returnValue = (await this.GetResolution()) == resolution;

			return returnValue;
		}

		public async Task<HeaterStatus> GetHeaterStatus()
		{
			HeaterStatus returnValue = HeaterStatus.Off;

			// ***
			// *** Get the current register
			// ***
			byte register = await this.GetRegister();

			if (RegisterConverter.BitIsHigh(register, REGISTER_BIT_HEATER))
			{
				returnValue = HeaterStatus.On;
			}
			else
			{
				returnValue = HeaterStatus.Off;
			}

			return returnValue;
		}

		public async Task<bool> SetHeaterStatus(HeaterStatus heaterStatus)
		{
			bool returnValue = false;

			// ***
			// *** Get the current register
			// ***
			byte register = await this.GetRegister();

			// ***
			// *** Set the bit
			// ***
			register = RegisterConverter.SetBit(register, REGISTER_BIT_HEATER, heaterStatus == HeaterStatus.On);

			// ***
			// *** Write the register
			// ***
			await this.WriteRegister(register);

			// ***
			// *** Check the register
			// ***
			returnValue = (await this.GetHeaterStatus()) == heaterStatus;

			return returnValue;
		}

		public async Task<BatteryStatus> GetBatteryStatus()
		{
			BatteryStatus returnValue = BatteryStatus.Unknown;

			// ***
			// *** Get the current register
			// ***
			byte register = await this.GetRegister();

			if (RegisterConverter.BitIsHigh(register, REGISTER_BIT_BATTERY))
			{
				returnValue = BatteryStatus.Low;
			}
			else
			{
				returnValue = BatteryStatus.Good;
			}

			return returnValue;
		}

		private async Task<byte> GetRegister()
		{
			byte[] readBuffer = new byte[] { 0 };
			await this.WriteReadAsync(new byte[] { READ_REGISTER }, readBuffer);
			return readBuffer[0];
		}

		private async Task<bool> WriteRegister(byte register)
		{
			byte[] readBuffer = new byte[] { 0 };
			await this.WriteAsync(new byte[] { WRITE_REGISTER, register });
			return (await this.GetRegister()) == register;
		}
	}
}
