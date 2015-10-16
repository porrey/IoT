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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Porrey.Uwp.IoT.Sensors
{
	/// <summary>
	/// Defines the result of the initialization process for the MCP9808.
	/// </summary>
	public enum InitializationResult
	{
		/// <summary>
		/// Initialization has not been performed.
		/// </summary>
		None,
		/// <summary>
		/// Initialization was successful.
		/// </summary>
		Successful,
		/// <summary>
		/// Initialization failed due to lack of an i2C controller.
		/// </summary>
		NoI2cController,
		/// <summary>
		/// Initialization failed because an MCP9808 was not 
		/// found on the I2C bus.
		/// </summary>
		DeviceNotFound,
		/// <summary>
		/// Initialization failed due to device already in use.
		/// </summary>
		DeviceInUse
	}

	public class I2c : II2c, IDisposable
	{
		private bool _initialized = false;
		private byte _deviceAddress = 0;
		private I2cBusSpeed _busSpeed = I2cBusSpeed.FastMode;
		private I2cDevice _device = null;

		public I2c(byte deviceAddress)
		{
			this.DeviceAddress = deviceAddress;
			this.BusSpeed = I2cBusSpeed.StandardMode;
		}

		public I2c(byte deviceAddress, I2cBusSpeed busSpeed)
		{
			this.DeviceAddress = deviceAddress;
			this.BusSpeed = busSpeed;
		}

		/// <summary>
		/// Gets the device address used when this instance 
		/// of the was created.
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
		/// of the device was created.
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
		/// Gets a value that indicates if the device was initialized 
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
		/// Initializes the I2C device.
		/// </summary>
		/// <returns>Returns an InitializationResult value indicating if the
		/// initialization was success or not.</returns>
		public async Task<InitializationResult> InitializeAsync()
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

					try
					{
						await this.OnInitializeAsync();
					}
					catch (Exception ex)
					{
						// ***
						// *** Looking for: "The system cannot find the file specified. Slave address was not acknowledged."
						// ***
						if (ex.Message.Contains("Slave address was not acknowledged"))
						{
							throw new InvalidAddressException();
						}
						else
						{
							throw;
						}
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

		public Task<bool> WriteReadAsync(byte[] writeBuffer, byte[] readBuffer)
		{
			bool returnValue = false;

			if (IsInitialized)
			{
				// ***
				// *** 
				// ***
				this.Device.WriteRead(writeBuffer, readBuffer);
			}
			else
			{
				throw new DeviceNotInitializedException();
			}

			return Task<bool>.FromResult(returnValue);
		}

		public Task<bool> WriteAsync(byte[] writeBuffer)
		{
			bool returnValue = false;

			if (IsInitialized)
			{
				// ***
				// *** 
				// ***
				this.Device.Write(writeBuffer);
			}
			else
			{
				throw new DeviceNotInitializedException();
			}

			return Task<bool>.FromResult(returnValue);
		}

		public Task<bool> ReadAsync(byte[] readBuffer)
		{
			bool returnValue = false;

			if (IsInitialized)
			{
				// ***
				// *** 
				// ***
				this.Device.Read(readBuffer);
			}
			else
			{
				throw new DeviceNotInitializedException();
			}

			return Task<bool>.FromResult(returnValue);
		}

		public async Task<byte[]> ReadBytesAsync(int bufferSize)
		{
			byte[] readBuffer = new byte[bufferSize];
			await this.ReadAsync(readBuffer);
			return readBuffer;
		}

		public async Task<byte[]> ReadRegisterBytesAsync(byte registerId, int bufferSize)
		{
			byte[] readBuffer = new byte[bufferSize];
			await this.WriteReadAsync(new byte[] { registerId }, readBuffer);
			return readBuffer;
		}

		public async Task ResetAsync()
		{
			await this.OnResetAsync();
		}

		public void Dispose()
		{
			try
			{
				this.OnDispose();
			}
			finally
			{
				if (_device != null)
				{
					_device.Dispose();
				}
			}
		}

		protected virtual void OnDispose()
		{
		}

		protected async virtual Task OnInitializeAsync()
		{
			// ***
			// *** The default behavior is to attempt a read
			// ***
			byte[] readBuffer = new byte[2];
			await this.ReadAsync(readBuffer);
		}

		protected virtual Task OnResetAsync()
		{
			return Task.FromResult(0);
		}
    }
}
