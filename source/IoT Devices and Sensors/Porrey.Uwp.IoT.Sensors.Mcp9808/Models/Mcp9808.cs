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
using System.Linq;
using System.Threading.Tasks;
using Porrey.Uwp.IoT.Sensors.I2C;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Porrey.Uwp.IoT.Sensors
{
	internal class Mcp9808 : IMcp9808
	{
		private byte _deviceAddress = 0;
		private I2cBusSpeed _busSpeed = I2cBusSpeed.FastMode;
		private bool _initialized = false;
		private I2cDevice _device = null;
		private IDeviceId _deviceId = null;
		private int? _manufacturingId = null;

		/// <summary>
		/// Creates an instance of IMcp9808 with the given
		/// device address and bus speed.
		/// </summary>
		/// <param name="deviceAddress">The address of the device.</param>
		/// <param name="busSpeed">The desired bus speed.</param>
		public Mcp9808(byte deviceAddress, I2cBusSpeed busSpeed)
		{
			this.DeviceAddress = deviceAddress;
			this.BusSpeed = busSpeed;
		}

		/// <summary>
		/// Gets the device address used when this instance 
		/// of the IMcp9808 was created.
		/// </summary>
		public byte DeviceAddress
		{
			get
			{
				return _deviceAddress;
			}
			private set
			{
				_deviceAddress = value;
			}
		}

		/// <summary>
		/// Gets the I2C bus speed set when this instance 
		/// of the IMcp9808 was created.
		/// </summary>
		public I2cBusSpeed BusSpeed
		{
			get
			{
				return _busSpeed;
			}
			private set
			{
				_busSpeed = value;
			}
		}

		/// <summary>
		/// Gets the Manufacturing ID from the device.
		/// </summary>
		public int ManufacturingId
		{
			get
			{
				if (!_manufacturingId.HasValue)
				{
					byte[] readBuffer = this.ReadFromRegister(0x06).Reverse().ToArray();
					_manufacturingId = (int)BitConverter.ToInt16(readBuffer, 0);
				}

				return _manufacturingId.Value;
			}
		}

		/// <summary>
		/// Gets the Device ID and Revision from the device.
		/// </summary>
		public IDeviceId DeviceId
		{
			get
			{
				if (_deviceId == null)
				{
					byte[] readBuffer = this.ReadFromRegister(0x07);
					_deviceId = new DeviceId() { Id = readBuffer[0], Revision = readBuffer[1] };
				}

				return _deviceId;
			}
		}		

		/// <summary>
		/// A reference to the I2cDevice retrieved from
		/// a call to I2cDevice.FromIdAsync.
		/// </summary>
		protected virtual I2cDevice Device
		{
			get
			{
				return _device;
			}
			set
			{
				_device = value;
			}
		}

		/// <summary>
		/// Initializes the MC9808.
		/// </summary>
		/// <returns>Returns an InitializationResult value indicating if the
		/// initialization was success or not.</returns>
		public async Task<InitializationResult> Initialize()
		{
			InitializationResult returnValue = InitializationResult.None;

			// ***
			// *** Get a selector string that will return all I2C controllers on the system
			// ***
			string aqs = I2cDevice.GetDeviceSelector();

			// ***
			// *** Find the I2C bus controller device with our selector string
			// ***
			var dis = await DeviceInformation.FindAllAsync(aqs).AsTask();

			if (dis.Count > 0)
			{
				var settings = new I2cConnectionSettings(this.DeviceAddress);
				settings.BusSpeed = this.BusSpeed;

				// ***
				// *** Create an I2cDevice with our selected bus controller and I2C settings
				// ***
				this.Device = await I2cDevice.FromIdAsync(dis[0].Id, settings);

				if (this.Device != null)
				{
					IsInitialized = true;

					// ***
					// *** Check the Manufacturing and Device ID
					// ***
					if (this.ManufacturingId == 0x54 && this.DeviceId.Id == 0x04)
					{
						returnValue = InitializationResult.Successful;						
					}
					else
					{
						IsInitialized = false;
						returnValue = InitializationResult.DeviceNotFound;
					}
				}
				else
				{
					// ***
					// *** Slave address n on I2C Controller  is currently in use by
					// *** another application. Please ensure that no other applications are using I2C.
					// ***
					returnValue = InitializationResult.DeviceInUse;
				}
			}
			else
			{
				// ***
				// *** No I2C controllers were found on the system
				// ***
				returnValue = InitializationResult.NoI2cController;
			}

			return returnValue;
		}

		/// <summary>
		/// Gets a value that indicates if the MCP9808 was initialized 
		/// successfully or not. True indicates that Initialize() was
		/// successfully called.
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				return _initialized;
			}
			private set
			{
				_initialized = value;
			}
		}

		/// <summary>
		/// Gets the current sensor reading from the device.
		/// </summary>
		/// <returns>Returns an ISensorReading instance that 
		/// indicates current temperature and whether or not
		/// the thresholds have been exceeded.</returns>
		public Task<IDeviceSensorReading> ReadSensor()
		{
			// ***
			// *** Initialize a value to hold the device temperature 
			// *** in Celsius
			// ***
			DeviceSensorReading returnValue = new DeviceSensorReading();

			// ***
			// *** The register/command for reading ambient temperature
			// *** from the device is 0x05
			// ***
			byte[] readBuffer = this.ReadFromRegister(Mcp9808Register.AmbientTemperature);

			// ***
			// *** Calculate the temperature value
			// ***
			returnValue.Temperature = RegisterConverter.ToFloat(readBuffer);

			// ***
			// *** Check the flags 
			// ***
			returnValue.IsCritical = (readBuffer[0] & 0x80) == 0x80;
			returnValue.IsAboveUpperThreshold = (readBuffer[0] & 0x40) == 0x40;
			returnValue.IsBelowLowerThreshold = (readBuffer[0] & 0x20) == 0x20;

			// ***
			// *** Return the value
			// ***
			return Task<IDeviceSensorReading>.FromResult((IDeviceSensorReading)returnValue);
		}

		/// <summary>
		/// Gets/sets the critical temperature used when 
		/// alert mode is enabled.
		/// </summary>
		public float CriticalTemperatureThreshold
		{
			get
			{
				return RegisterConverter.ToFloat(this.ReadFromRegister(Mcp9808Register.CriticalTemperature));
			}
			set
			{
				byte[] buffer = RegisterConverter.ToByteArray(value);
				this.WriteToRegister(Mcp9808Register.CriticalTemperature, buffer[0], buffer[1]);
			}
		}

		/// <summary>
		/// Gets/sets the lower temperature threshold used 
		/// when alert mode is enabled.
		/// </summary>
		public float LowerTemperatureThreshold
		{
			get
			{
				return RegisterConverter.ToFloat(this.ReadFromRegister(Mcp9808Register.LowerTemperature));
			}
			set
			{
				byte[] buffer = RegisterConverter.ToByteArray(value);
				this.WriteToRegister(Mcp9808Register.LowerTemperature, buffer[0], buffer[1]);
			}
		}

		/// <summary>
		/// Gets/sets the upper temperature threshold used 
		/// when alert mode is enabled. 
		/// </summary>
		public float UpperTemperatureThreshold
		{
			get
			{
				return RegisterConverter.ToFloat(this.ReadFromRegister(Mcp9808Register.UpperTemperature));
			}
			set
			{
				byte[] buffer = RegisterConverter.ToByteArray(value);
				this.WriteToRegister(Mcp9808Register.UpperTemperature, buffer[0], buffer[1]);
			}
		}

		/// <summary>
		/// Gets/sets the Hysteresis for the device.
		/// </summary>
		public Mcp9808Hysteresis Hysteresis
		{
			get
			{
				// ***
				// *** The hysteresis is a two bit value in the upper
				// *** byte of the configuration
				// ***
				byte[] buffer = this.ReadFromRegister(Mcp9808Register.Configuration);

				// ***
				// *** Shift the bits by 1
				// ***
				int hysteresis = (buffer[0] & 0x6) / 2;

				// ***
				// *** Cat the value to the enumeration
				// ***
				return (Mcp9808Hysteresis)hysteresis;
			}
			set
			{
				byte[] buffer = this.ReadFromRegister(Mcp9808Register.Configuration);

				// ***
				// *** Clear the two bits
				// ***
				buffer[0] &= 0xF9;

				// ***
				// *** Cast the value to int and shift the bits by 1
				// ***
				byte hysteresis = (byte)((int)value * 2);

				// ***
				// *** Add the value to the buffer
				// ***
				buffer[0] += hysteresis;

				this.WriteToRegister(Mcp9808Register.Configuration, buffer[0], buffer[1]);
			}
		}

		/// <summary>
		/// Gets/sets a value that indicates if the alert mode
		/// is enabled or disabled on the device.
		/// </summary>
		public bool AlertEnabled
		{
			get
			{
				return this.GetConfigurationBit(Mcp9808ConfigurationBit.AlertOutputControl);
			}
			set
			{
				this.SetConfigurationBit(Mcp9808ConfigurationBit.AlertOutputControl, value);
			}
		}

		/// <summary>
		/// Gets/sets the output level of the alert pin.
		/// This bit cannot be altered when either of the Lock bits 
		/// are set (bit 6 and bit 7). This bit can be programmed 
		/// in Shutdown mode, but the Alert output will not assert or deassert.
		/// </summary>
		public Mcp9808AlertOutputPolarity AlertOutputPolarity
		{
			get
			{
				return this.GetConfigurationBit(Mcp9808ConfigurationBit.AlertOutputPolarity) ? Mcp9808AlertOutputPolarity.ActiveHigh : Mcp9808AlertOutputPolarity.ActiveLow;
			}
			set
			{
				this.SetConfigurationBit(Mcp9808ConfigurationBit.AlertOutputPolarity, value == Mcp9808AlertOutputPolarity.ActiveHigh ? true : false);
			}
		}

		/// <summary>
		/// Gets/sets the alert output mode. When set to ComparatorMode
		/// the alert will automatically assert and deassert when the
		/// temperature falls in or out of the specified range. When set
		/// to InterruptMode once the alert is asserted it must be 
		/// manually cleared. This bit cannot be altered when either 
		/// of the Lock bits are set(bit 6 and bit 7). This bit 
		/// can be programmed in Shutdown mode, but the Alert output 
		/// will not assert or deassert.
		/// </summary>
		public Mcp9808AlertOutputMode AlertOutputMode
		{
			get
			{
				return this.GetConfigurationBit(Mcp9808ConfigurationBit.AlertOutputMode) ? Mcp9808AlertOutputMode.InterruptMode : Mcp9808AlertOutputMode.ComparatorMode;
			}
			set
			{
				this.SetConfigurationBit(Mcp9808ConfigurationBit.AlertOutputMode, value == Mcp9808AlertOutputMode.InterruptMode ? true : false);
			}
		}

		/// <summary>
		/// Gets/sets a value that indicates if the alert output
		/// is based on critical temperature only or on critical
		/// temperature and the upper and lower threshold temperatures.
		/// When the Alarm Window Lock bit is set, this bit cannot be 
		/// altered until unlocked (bit 6). This bit can be programmed 
		/// in Shutdown mode, but the Alert output will not assert or 
		/// deassert.
		/// </summary>
		public Mcp9808AlertOutputSelect AlertOutputSelect
		{
			get
			{
				return this.GetConfigurationBit(Mcp9808ConfigurationBit.AlertOutputSelect) ? Mcp9808AlertOutputSelect.CriticalTemperatureOnly : Mcp9808AlertOutputSelect.All;
			}
			set
			{
				this.SetConfigurationBit(Mcp9808ConfigurationBit.AlertOutputSelect, value == Mcp9808AlertOutputSelect.CriticalTemperatureOnly ? true : false);
			}
		}

		/// <summary>
		/// Gets or clears the output status of the alert pin. This bit 
		/// can not be set to ‘1’ or cleared to ‘0’ in Shutdown 
		/// mode. However, if the Alert output is configured as Interrupt 
		/// mode, and if the host controller clears to ‘0’, the interrupt, 
		/// using bit 5 while the device is in Shutdown mode, then this 
		/// bit will also be cleared ‘0’.
		/// </summary>
		public Mcp9808AlertOutputStatus AlertOutputStatus
		{
			get
			{
				return this.GetConfigurationBit(Mcp9808ConfigurationBit.AlertOutputStatus) ? Mcp9808AlertOutputStatus.Asserted : Mcp9808AlertOutputStatus.NotAsserted;
			}
			set
			{
				this.SetConfigurationBit(Mcp9808ConfigurationBit.AlertOutputStatus, value == Mcp9808AlertOutputStatus.Asserted ? true : false);
			}
		}

		/// <summary>
		/// When the alert is asserted in Interrupt mode, this method
		/// should be called to deassert. This bit can not be set to 
		/// ‘1’ in Shutdown mode, but it can be cleared after the 
		/// device enters Shutdown mode.
		/// </summary>
		public void ClearInterrupt()
		{
			this.SetConfigurationBit(Mcp9808ConfigurationBit.InterruptClear, true);
		}

		/// <summary>
		/// Gets/sets the operation mode of the device. In shutdown, 
		/// all power-consuming activities are disabled, though 
		/// all registers can be written to or read. This bit 
		/// cannot be set to ‘1’ when either of the Lock bits 
		/// is set(bit 6 and bit 7). However, it can be cleared 
		/// to ‘0’ for continuous conversion while locked
		/// </summary>
		public Mcp9808ShutdownMode ShutdownMode
		{
			get
			{
				return this.GetConfigurationBit(Mcp9808ConfigurationBit.ShutdownMode) ? Mcp9808ShutdownMode.LowPowerMode : Mcp9808ShutdownMode.ContinuousConversionMode;
			}
			set
			{
				this.SetConfigurationBit(Mcp9808ConfigurationBit.ShutdownMode, value == Mcp9808ShutdownMode.LowPowerMode ? true : false);
			}
		}

		/// <summary>
		/// Reads the value of a register.
		/// </summary>
		/// <param name="registerId">Specifies the ID of the register.</param>
		/// <returns>Returns the bytes stored in the specified register. The
		/// upper byte is at index 0 and the lower byte is at index 1;</returns>
		public byte[] ReadFromRegister(byte registerId)
		{
			byte[] returnValue = new byte[2];

			if (IsInitialized)
			{
				// ***
				// *** The register ID
				// ***
				byte[] writeBuffer = new byte[1] { registerId };

				// ***
				// *** Write the register value and read from the device
				// *** at the same time. The byte at index 0 is the high
				// *** byte and the byte at index 1 is the low byte.
				// ***
				this.Device.WriteRead(writeBuffer, returnValue);
			}
			else
			{
				throw new DeviceNotInitializedException();
			}

			return returnValue;
		}

		/// <summary>
		/// Writes two bytes to the specified register.
		/// </summary>
		/// <param name="registerId">Specifies the ID of the register.</param>
		/// <param name="upperByte">The value of the upper byte 
		/// that is written.</param>
		/// <param name="lowerByte">The value of the lower byte 
		/// that is written.</param>
		/// <returns>Returns true if successful; false otherwise.</returns>
		public bool WriteToRegister(byte registerId, byte upperByte, byte lowerByte)
		{
			bool returnValue = false;

			if (IsInitialized)
			{
				// ***
				// *** Write the register value and read from the device
				// *** at the same time
				// ***
				byte[] writeBuffer = new byte[3] { registerId, upperByte, lowerByte };
				this.Device.Write(writeBuffer);
			}
			else
			{
				throw new DeviceNotInitializedException();
			}

			return returnValue;
		}

		/// <summary>
		/// Gets the value of the configuration bit at 
		/// the given index.
		/// </summary>
		/// <param name="bitIndex">Specifies the configuration 
		/// bit index from 0 to 15.</param>
		/// <returns>Returns true if the bit is High and false 
		/// if the bit is low.</returns>
		public virtual bool GetConfigurationBit(int bitIndex)
		{
			byte[] buffer = this.ReadFromRegister(Mcp9808Register.Configuration);
			return RegisterConverter.BitIsHigh(buffer[bitIndex > 7 ? 0 : 1], bitIndex);
		}

		/// <summary>
		/// Sets the value of a configuration bit at the given
		/// index.
		/// </summary>
		/// <param name="bitIndex">Specifies the configuration 
		/// bit index from 0 to 15.</param>
		/// <param name="value">The value of the bit to set.</param>
		public virtual void SetConfigurationBit(int bitIndex, bool value)
		{
			// ***
			// *** Read the current configuration
			// ***
			byte[] config = this.ReadFromRegister(Mcp9808Register.Configuration);

			// ***
			// *** Set the selected bit
			// ***
			config[bitIndex > 7 ? 0 : 1] = RegisterConverter.SetBit(config[bitIndex > 7 ? 0 : 1], bitIndex % 8, value);

			// ***
			// *** Write the configuration back to the register
			// ***
			this.WriteToRegister(Mcp9808Register.Configuration, config[0], config[1]);
		}

		/// <summary>
		/// Dispose managed objects.
		/// </summary>
		public void Dispose()
		{
			// ***
			// *** Check to see if initialized
			// ***
			if (this.IsInitialized)
			{
				// ***
				// *** Dispose the device
				// ***
				if (this.Device != null)
				{
					this.Device.Dispose();
					this.Device = null;
				}

				// ***
				// *** Mark the Mcp9808 instance as NOT initialized
				// ***
				this.IsInitialized = false;
			}
		}
	}
}
