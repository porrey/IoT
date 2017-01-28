// Copyright © 2015-2017 Daniel Porrey. All Rights Reserved.
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
using Porrey.Uwp.IoT.Sensors;
using Porrey.Uwp.IoT.Sensors.Models;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;

namespace Porrey.Uwp.IoT.Devices
{
    /// <summary>
    /// Implements a custom I2C interface for the
    /// DS1307 DTC.
    /// </summary>
    public class Ds1307 : I2c, IDs1307
	{
		public enum OscillatorOutputMode
		{
			SquareWave,
			Gpio
		}

		public enum Frequency
		{
			None,
			OneHertz,
			FourKiloHertz,
			EightKiloHertz,
			ThirtyTwoKiloHertz
		};

		private const byte I2C_ADDRESS = 0x68;
		private const byte RTC_ADDRESS = 0x00;
		private const byte CONTROL_ADDRESS = 0x07;
		private const byte MEMORY_ADDRESS = 0x08;
		private const byte MEMORY_SIZE = 56;

		private OscillatorOutputMode _oscillatorMode = OscillatorOutputMode.SquareWave;
		private Frequency _oscillatorFrequency = Frequency.OneHertz;
		private GpioPinValue _oscillatorValue = GpioPinValue.Low;

		public Ds1307() : base(I2C_ADDRESS, I2cBusSpeed.FastMode)
		{
		}

		public Ds1307(I2cBusSpeed busSpeed) : base(I2C_ADDRESS, busSpeed)
		{
		}

		public OscillatorOutputMode OscillatorMode
		{
			get
			{
				return _oscillatorMode;
			}
			set
			{
				_oscillatorMode = value;
				this.SetControlRegister(this.OscillatorValue, this.OscillatorMode == OscillatorOutputMode.SquareWave, this.OscillatorFrequency).Wait();
			}
		}

		public Frequency OscillatorFrequency
		{
			get
			{
				return _oscillatorFrequency;
			}
			set
			{
				_oscillatorFrequency = value;
				this.SetControlRegister(this.OscillatorValue, this.OscillatorMode == OscillatorOutputMode.SquareWave, this.OscillatorFrequency).Wait();
			}
		}

		public GpioPinValue OscillatorValue
		{
			get
			{
				return _oscillatorValue;
			}
			set
			{
				this._oscillatorValue = value;
				this.SetControlRegister(this.OscillatorValue, this.OscillatorMode == OscillatorOutputMode.SquareWave, this.OscillatorFrequency).Wait();
			}
		}

		/// <summary>
		/// Get the current Date and Time from the DS1307.
		/// </summary>
		/// <returns>The current date and time as a System.DateTime object.</returns>
		public Task<DateTimeOffset> GetAsync()
		{
			DateTimeOffset returnValue = DateTime.MinValue;

			byte[] readBuffer = new byte[7];
			byte[] writeBuffer = new byte[] { RTC_ADDRESS };

			this.WriteReadAsync(writeBuffer, readBuffer);

			returnValue = new DateTime
			(
				Bcd.ToInt(readBuffer[6]) + 2000,            // Year
				Bcd.ToInt(readBuffer[5]),                   // Month
				Bcd.ToInt(readBuffer[4]),                   // Day
				Bcd.ToInt((byte)(readBuffer[2] & 0x3f)),    // Hours over 24 hours (bit 6 is 24/12 hour format; 1 = 12, 0 = 24)
				Bcd.ToInt(readBuffer[1]),                   // Minutes
				Bcd.ToInt((byte)(readBuffer[0] & 0x7f))     // Seconds (bit 7 is the clock halt bit; 0 = enabled, 1 = halted)
			);

			return Task<DateTimeOffset>.FromResult(returnValue);
		}

		/// <summary>
		/// Sets the Date and Time on the DS1307.
		/// </summary>
		/// <param name="value">The date and time as a System.DateTime object
		/// that will be saved on the DS1307.</param>
		public async Task SetAsync(DateTimeOffset value)
		{
			byte[] writeBuffer = new byte[]
			{
				RTC_ADDRESS,
				Bcd.FromInt(value.Second),
				Bcd.FromInt(value.Minute),
				Bcd.FromInt(value.Hour),
				Bcd.FromInt((int)value.DayOfWeek),
				Bcd.FromInt(value.Day),
				Bcd.FromInt(value.Month),
				Bcd.FromInt(value.Year >= 2000 ? value.Year - 2000: 0),
			};

			await this.WriteAsync(writeBuffer);
		}

		protected async Task SetControlRegister(GpioPinValue outBit, bool sqweBit, Frequency frequency)
		{
			byte writeValue = 0;

			// ***
			// *** Bit 7
			// ***
			if (outBit == GpioPinValue.High)
			{
				writeValue |= 0x80;
			}

			// ***
			// *** Bit 4
			// ***
			if (sqweBit)
			{
				writeValue |= 0x10;
			}

			// ***
			// *** Bits 1 & 0
			// ***
			switch (frequency)
			{
				case Frequency.None:
				case Frequency.OneHertz:
					writeValue |= 0x0;
					break;
				case Frequency.FourKiloHertz:
					writeValue |= 0x1;
					break;
				case Frequency.EightKiloHertz:
					writeValue |= 0x2;
					break;
				case Frequency.ThirtyTwoKiloHertz:
					writeValue |= 0x3;
					break;
			}

			byte[] writeBuffer = new byte[] { CONTROL_ADDRESS, writeValue };
			await this.WriteAsync(writeBuffer);
		}

		public async Task Halt()
		{
			byte[] readBuffer = new byte[1];
			byte[] writeBuffer = new byte[] { RTC_ADDRESS };

			// ***
			// *** Read the seconds
			// ***
			await this.WriteReadAsync(writeBuffer, readBuffer);
			int seconds = Bcd.ToInt((byte)(readBuffer[0] & 0x7f));

			// ***
			// *** Set bit 7
			// ***
			writeBuffer = new byte[] { RTC_ADDRESS, (byte)(seconds | 0x80) };
			await this.WriteAsync(writeBuffer);
		}

		public async Task Resume()
		{
			byte[] readBuffer = new byte[1];
			byte[] writeBuffer = new byte[] { RTC_ADDRESS };

			// ***
			// *** Read the seconds
			// ***
			await this.WriteReadAsync(writeBuffer, readBuffer);
			int seconds = Bcd.ToInt((byte)(readBuffer[0] & 0x7f));

			// ***
			// *** Reset bit 7
			// ***
			writeBuffer = new byte[] { RTC_ADDRESS, (byte)(seconds & 0x80) };
			await this.WriteAsync(writeBuffer);
		}

		public async Task<byte[]> ReadMemory()
		{
			byte[] readBuffer = new byte[MEMORY_SIZE];
			byte[] writeBuffer = new byte[] { MEMORY_ADDRESS };
			await this.WriteReadAsync(writeBuffer, readBuffer);

			return readBuffer;
        }

		public async Task WriteMemory(byte[] data)
		{
			if (data.Length <= MEMORY_SIZE)
			{
				byte[] writeBuffer = new byte[MEMORY_SIZE + 1];
				writeBuffer[0] = MEMORY_ADDRESS;
                data.CopyTo(writeBuffer, 1);
				await this.WriteAsync(writeBuffer);
			}
			else
			{
				throw new ArgumentOutOfRangeException(string.Format("The buffer size cannot exceed {0} bytes.", MEMORY_SIZE));
			}
		}
	}
}