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
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

namespace Porrey.Uwp.IoT.Sensors
{
	/// <summary>
	/// Defines an intergration point to the SPI interface.
	/// </summary>
	public class Spi : ISpi
	{
		private SpiConnectionSettings _settings = null;
		private SpiDevice _device = null;

        /// <summary>
        /// Initializes a new instance of the Windows.Devices.Sensors.Spi with
        /// the specified Chip Select Line (0 or 1).
        /// </summary>
        /// <param name="chipSelectLine">The Chip Select Line the sensor
        /// is physically connected to. This value is either 0 or 1 on the
        /// Raspberry Pi 2.</param>
        public Spi(int chipSelectLine)
		{
			this.Settings = new SpiConnectionSettings(chipSelectLine);

			// ***
			// *** Set the defaults for the SPI interface
			// ***			
			this.Settings.ClockFrequency = 1000000;
			this.Settings.Mode = SpiMode.Mode0;
			this.Settings.SharingMode = SpiSharingMode.Exclusive;
		}

        /// <summary>
        /// Initializes a new instance of the Windows.Devices.Sensors.Spi with
        /// the specified SpiConnectionSettings.
        /// </summary>
        /// <param name="settings">An instance of SpiConnectionSettings that specifies
        /// the parameters of the SPI connection for the sensor.</param>
        public Spi(SpiConnectionSettings settings)
		{
			this.Settings = settings;
		}

		/// <summary>
		/// Gets the settings used on the SPI interface
		/// to communicate to the sensor.
		/// </summary>
		public SpiConnectionSettings Settings
		{
			get
			{
				return _settings;
			}
			protected set
			{
				this._settings = value;
			}
		}

		/// <summary>
		/// Initializes the sensor by establishing a connection on the SPI interface.
		/// </summary>
		public async Task Initialize()
		{
			if (_device == null)
			{
				// ***
				// *** Select and setup SPI
				// ***
				string selector = SpiDevice.GetDeviceSelector(string.Format("SPI{0}", this.Settings.ChipSelectLine));
				var deviceInfo = await DeviceInformation.FindAllAsync(selector);
				_device = await SpiDevice.FromIdAsync(deviceInfo[0].Id, this.Settings);
			}
			else
			{
				throw new AlreadyInitializedException();
			}
		}

		/// <summary>
		/// Gets the underlying SpiDevice instance used by this instance
		/// of Porrey.Uwp.IoT.Sensors.Spi.
		/// </summary>
		public SpiDevice Device
		{
			get
			{
				if (_device == null)
				{
					throw new NotInitializedException();
				}

				return _device;
			}
			protected set
			{
				this._device = value;
			}
		}

		/// <summary>
		/// Disposes manages objects used by this instance of Porrey.Uwp.IoT.Sensors.Spi. After
		/// being disposed this instance should not be used.
		/// </summary>
		public void Dispose()
		{
			try
			{
				if (_device != null)
				{
					_device.Dispose();
					_device = null;
				}
			}
			finally
			{
				this.OnDispose();
			}
		}

		protected virtual void OnDispose()
		{
		}
	}
}
