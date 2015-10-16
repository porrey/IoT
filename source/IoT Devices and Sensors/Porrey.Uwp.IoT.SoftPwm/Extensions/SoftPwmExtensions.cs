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
using Windows.Devices.Gpio;

namespace Porrey.Uwp.IoT
{
	/// <summary>
	/// Fluent API Extension for SoftPwm.
	/// </summary>
	public static class SoftPwmExtensions
	{
		/// <summary>
		/// Creates an instance of a SoftPwm object from the given 
		/// Windows.Devices.Gpio.GpioPin instance.
		/// </summary>
		/// <param name="pin">An instance of Windows.Devices.Gpio.GpioPin to 
		/// create the SoftPwm on.</param>
		/// <returns>Returns a new SOftPwm instance.</returns>
		public static ISoftPwm AssignSoftPwm(this GpioPin pin)
		{
			return new SoftPwm(pin);
		}

		/// <summary>
		/// Starts the given SoftPwm instance.
		/// </summary>
		/// <param name="pwm">The instance of SoftPwm to start.</param>
		/// <returns></returns>
		public static ISoftPwm Start(this ISoftPwm pwm)
		{
			pwm.StartAsync();
			return pwm;
		}

		/// <summary>
		/// Sets the value of a SoftPwm instance with the given value.
		/// </summary>
		/// <param name="pwm">The instance of SoftPwm to start.</param>
		/// <param name="value">The value to set the SoftPwm instance to.</param>
		/// <returns></returns>
		public static ISoftPwm WithValue(this ISoftPwm pwm, double value)
		{
			pwm.Value = value;
			return pwm;
		}

		/// <summary>
		/// Sets the pulse frequency (in Hz) of the SoftPwm instance.
		/// </summary>
		/// <param name="pwm">The instance of SoftPwm to start.</param>
		/// <param name="pulseFrequency">The pulse frequency to use given in Hz.</param>
		/// <returns></returns>
		public static ISoftPwm WithPulseFrequency(this ISoftPwm pwm, double pulseFrequency)
		{
			pwm.PulseFrequency = pulseFrequency;
			return pwm;
		}

		/// <summary>
		/// Attaches a handler to the PulseWidthChanged event to watch for
		/// changes to the HighPulseWidth and LowPulseWidth properties.
		/// </summary>
		/// <param name="pwm">The instance of SoftPwm to start.</param>
		/// <param name="eventHandler">A PulseWidthChangedEventHandler method or lambda expression..</param>
		/// <returns>The ISoftPwm reference to allow chaining of methods.</returns>
		public static ISoftPwm WatchPulseWidthChanges(this ISoftPwm pwm, PulseWidthChangedEventHandler eventHandler)
		{
			if (pwm is SoftPwm)
			{
				((SoftPwm)pwm).PulseWidthChanged += eventHandler;
			}

			return pwm;
        }

		/// <summary>
		/// Attaches a handler to the PwmPulsed event to watch
		/// for changes in the pulse.
		/// </summary>
		/// <param name="pwm">The instance of SoftPwm to start.</param>
		/// <param name="eventHandler">A EventHandler method or lambda expression.</param>
		/// <returns>The ISoftPwm reference to allow chaining of methods.</returns>
		public static ISoftPwm WatchPulse(this ISoftPwm pwm, EventHandler eventHandler)
		{
			if (pwm is SoftPwm)
			{
				((SoftPwm)pwm).PwmPulsed += eventHandler;
			}

			return pwm;
		}
	}
}
