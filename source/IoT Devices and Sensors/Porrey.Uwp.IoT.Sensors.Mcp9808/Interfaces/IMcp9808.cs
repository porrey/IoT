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
using Windows.Devices.I2c;

namespace Porrey.Uwp.IoT.Sensors
{
	/// <summary>
	/// Defines the output value of the alert pin.
	/// </summary>
	public enum Mcp9808AlertOutputPolarity
	{
		/// <summary>
		/// When the alert is active, the pin will be Low. When
		/// this mode is selected, the pin is normally High and 
		/// requires a pull-up resistor. This is the power-up
		/// default value.
		/// </summary>
		ActiveLow = 0,
		/// <summary>
		/// When the alert is active, the pin will be High. When
		/// this mode is selected the pin is normally Low.
		/// </summary>
		ActiveHigh = 1
	}

	/// <summary>
	/// 
	/// </summary>
	public enum Mcp9808AlertOutputMode
	{
		/// <summary>
		/// Comparator output (power-up default). In this mode the 
		/// alert will auto-reset when the temperature is back within
		/// the defined normal zone.
		/// </summary>
		ComparatorMode = 0,
		/// <summary>
		/// Interrupt output. In this mode the alert must be manually
		/// reset.
		/// </summary>
		InterruptMode = 1
	}

	/// <summary>
	/// Determines the parameters that will trigger (assert)
	/// the alert.
	/// </summary>
	public enum Mcp9808AlertOutputSelect
	{
		/// <summary>
		/// Alert output for TUPPER, TLOWER and TCRIT (power-up default)
		/// </summary>
		All = 0,
		/// <summary>
		/// TA > TCRIT only (TUPPER and TLOWER temperature boundaries are disabled)
		/// </summary>
		CriticalTemperatureOnly = 1
	}

	/// <summary>
	/// Alert Output Status 
	/// </summary>
	public enum Mcp9808AlertOutputStatus
	{
		/// <summary>
		/// Alert output is not asserted by the 
		/// device (power-up default)
		/// </summary>
		NotAsserted = 0,
		/// <summary>
		/// Alert output is asserted as a Comparator/Interrupt 
		/// or critical temperature output
		/// </summary>
		Asserted = 1
	}

	/// <summary>
	/// In shutdown, all power-consuming activities are disabled, though all 
	/// registers can be written to or read. This bit cannot be set to ‘1’ 
	/// when either of the Lock bits is set(bit 6 and bit 7). However, it can be
	/// cleared to ‘0’ for continuous conversion while locked
	/// </summary>
	public enum Mcp9808ShutdownMode
	{
		/// <summary>
		/// Continuous conversion (power-up default)
		/// </summary>
		ContinuousConversionMode = 0,
		/// <summary>
		/// Shutdown (Low-Power mode)
		/// </summary>
		LowPowerMode = 1
	}

	/// <summary>
	/// A hysteresis of 0°C, +1.5°C, +3°C or +6°C can be selected 
	/// for the TUPPER, TLOWER and TCRIT temperate boundaries, using 
	/// bits 10 and 9 of CONFIG. The hysteresis applies for decreasing 
	/// temperature only (hot to cold) or as temperature drifts below 
	/// the specified limit. 
	/// /// </summary>
	public enum Mcp9808Hysteresis
	{
		/// <summary>
		/// 0°C
		/// </summary>
		Zero = 0,
		/// <summary>
		/// +1.5°C
		/// </summary>
		PositiveOnePointFive = 1,
		/// <summary>
		/// +3°C
		/// </summary>
		PositiveThree = 2,
		/// <summary>
		/// +6°C
		/// </summary>
		PositiveSix = 3
	}

	/// <summary>
	/// Defines an interface for interacting with the MC9808.
	/// </summary>
	public interface IMcp9808 : II2CSensor, IDisposable
	{
		/// <summary>
		/// Initializes the MC9808.
		/// </summary>
		/// <returns>Returns a InitializationResult value indicating if the
		/// initialization was success or not.</returns>
		Task<InitializationResult> Initialize();

		/// <summary>
		/// Gets a value that indicates if the MCP9808 was initialized 
		/// successfully or not. True indicates that Initialize() was
		/// successfully called.
		/// </summary>
		bool IsInitialized { get; }

		/// <summary>
		/// Gets the device address used when this instance 
		/// of the IMcp9808 was created.
		/// </summary>
		byte DeviceAddress { get; }

		/// <summary>
		/// Gets the I2C bus speed set when this instance 
		/// of the IMcp9808 was created.
		/// </summary>
		I2cBusSpeed BusSpeed { get; }

		/// <summary>
		/// Gets the Manufacturing ID from the device.
		/// </summary>
		int ManufacturingId { get; }

		/// <summary>
		/// Gets the Device ID and Revision from the device.
		/// </summary>
		IDeviceId DeviceId { get; }

		/// <summary>
		/// Gets the current sensor reading from the device.
		/// </summary>
		/// <returns>Returns an ISensorReading instance that 
		/// indicates current temperature and whether or not
		/// the thresholds have been exceeded.</returns>
		Task<IDeviceSensorReading> ReadSensor();

		/// <summary>
		/// Gets/sets a value that indicates if the alert mode
		/// is enabled or disabled on the device.
		/// </summary>
		bool AlertEnabled { get; set; }

		/// <summary>
		/// Gets/sets the output level of the alert pin.
		/// This bit cannot be altered when either of the Lock bits 
		/// are set (bit 6 and bit 7). This bit can be programmed 
		/// in Shutdown mode, but the Alert output will not assert or deassert.
		/// </summary>
		Mcp9808AlertOutputPolarity AlertOutputPolarity { get; set; }

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
		Mcp9808AlertOutputMode AlertOutputMode { get; set; }

		/// <summary>
		/// Gets/sets a value that indicates if the alert output
		/// is based on critical temperature only or on critical
		/// temperature and the upper and lower threshold temperatures.
		/// When the Alarm Window Lock bit is set, this bit cannot be 
		/// altered until unlocked (bit 6). This bit can be programmed 
		/// in Shutdown mode, but the Alert output will not assert or 
		/// deassert.
		/// </summary>
		Mcp9808AlertOutputSelect AlertOutputSelect { get; set; }

		/// <summary>
		/// Gets or clears the output status of the alert pin. This bit 
		/// can not be set to ‘1’ or cleared to ‘0’ in Shutdown 
		/// mode. However, if the Alert output is configured as Interrupt 
		/// mode, and if the host controller clears to ‘0’, the interrupt, 
		/// using bit 5 while the device is in Shutdown mode, then this 
		/// bit will also be cleared ‘0’.
		/// </summary>
		Mcp9808AlertOutputStatus AlertOutputStatus { get; set; }

		/// <summary>
		/// When the alert is asserted in Interrupt mode, this method
		/// should be called to deassert. This bit can not be set to 
		/// ‘1’ in Shutdown mode, but it can be cleared after the 
		/// device enters Shutdown mode.
		/// </summary>
		void ClearInterrupt();

		/// <summary>
		/// Gets/sets the operation mode of the device. In shutdown, 
		/// all power-consuming activities are disabled, though 
		/// all registers can be written to or read. This bit 
		/// cannot be set to ‘1’ when either of the Lock bits 
		/// is set(bit 6 and bit 7). However, it can be cleared 
		/// to ‘0’ for continuous conversion while locked
		/// </summary>
		Mcp9808ShutdownMode ShutdownMode { get; set; }

		/// <summary>
		/// Gets/sets the critical temperature used when 
		/// alert mode is enabled.
		/// </summary>
		float CriticalTemperatureThreshold { get; set; }

		/// <summary>
		/// Gets/sets the lower temperature threshold used 
		/// when alert mode is enabled.
		/// </summary>
		float LowerTemperatureThreshold { get; set; }

		/// <summary>
		/// Gets/sets the upper temperature threshold used 
		/// when alert mode is enabled. 
		/// </summary>
		float UpperTemperatureThreshold { get; set; }

		/// <summary>
		/// Gets/sets the Hysteresis for the device.
		/// </summary>
		Mcp9808Hysteresis Hysteresis { get; set; }

		/// <summary>
		/// Gets the value of the configuration bit at 
		/// the given index.
		/// </summary>
		/// <param name="bitIndex">Specifies the configuration 
		/// bit index from 0 to 15.</param>
		/// <returns>Returns true if the bit is High and false 
		/// if the bit is low.</returns>
		bool GetConfigurationBit(int bitIndex);

		/// <summary>
		/// Sets the value of a configuration bit at the given
		/// index.
		/// </summary>
		/// <param name="bitIndex">Specifies the configuration 
		/// bit index from 0 to 15.</param>
		/// <param name="value">The value of the bit to set.</param>
		void SetConfigurationBit(int bitIndex, bool value);

		/// <summary>
		/// Writes two bytes to the specified register.
		/// </summary>
		/// <param name="registerId">Specifies the ID of the register.</param>
		/// <param name="upperByte">The value of the upper byte 
		/// that is written.</param>
		/// <param name="lowerByte">The value of the lower byte 
		/// that is written.</param>
		/// <returns>Returns true if successful; false otherwise.</returns>
		bool WriteToRegister(byte registerId, byte upperByte, byte lowerByte);

		/// <summary>
		/// Reads the value of a register.
		/// </summary>
		/// <param name="registerId">Specifies the ID of the register.</param>
		/// <returns>Returns the bytes stored in the specified register. The
		/// upper byte is at index 0 and the lower byte is at index 1;</returns>
		byte[] ReadFromRegister(byte registerId);

	}
}
