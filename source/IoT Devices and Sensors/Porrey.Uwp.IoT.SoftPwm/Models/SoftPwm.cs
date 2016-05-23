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
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Porrey.Uwp.IoT
{
	/// <summary>
	/// Delegate for the PulseWidthChangedEvent.
	/// </summary>
	/// <param name="sender">The object where the event handler is attached.</param>
	/// <param name="e">The event data.</param>
	public delegate void PulseWidthChangedEventHandler(object sender, PulseWidthChangedEventArgs e);

	/// <summary>
	/// Provides a software based Pulse Width Modulation capability for any GPIO pin on
	/// the device. PWM is used in a variety of circuits as a way to control analog 
	/// circuits through digital interfaces.
	/// </summary>
	public class SoftPwm : ISoftPwm
	{
		/// <summary>
		/// This event is fired whenever the HighPulseWidth or LowPulseWidth
		/// values change. NOTE: This property is NOT fired when the PulseWidth
		/// property changes.
		/// </summary>
		public event PulseWidthChangedEventHandler PulseWidthChanged = null;

		/// <summary>
		/// This event is fired for every pulse (after the low pulse). Monitoring of this event
		/// can impact the performance of this Soft PWM and it's ability to keep accurate timing.
		/// </summary>
		public EventHandler PwmPulsed = null;

		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private Task _pulserTask;
		private double _value = 0;
		private double _previousLowPulseWidth = 0d;
		private double _previousHighPulseWidth = 0d;

		/// <summary>
		/// Creates an instance of SoftPwm given an instance
		/// of Windows.Devices.Gpio.GpioPin.
		/// </summary>
		/// <param name="pin">An instance of Windows.Devices.Gpio.GpioPin to create the SoftPwm on.</param>
		public SoftPwm(GpioPin pin)
		{
			if (pin == null) throw new ArgumentNullException(nameof(pin));

			// ***
			// *** Set the pin up
			// ***
			this.Pin = pin;
			this.Pin.SetDriveMode(GpioPinDriveMode.Output);
			this.Pin.Write(GpioPinValue.Low);
		}

		/// <summary>
		/// Gets the underlying Windows.Devices.Gpio.GpioPin instance that this SoftPwm instance
		/// is controlling.
		/// </summary>
		public GpioPin Pin { get; protected set; } = null;

		/// <summary>
		/// Gets the minimum value that can be set.
		/// </summary>
		public double MinimumValue { get; } = 0;

		/// <summary>
		/// Gets/sets the maximum value allowed.
		/// </summary>
		public double MaximumValue { get; set; } = 255;

		/// <summary>
		/// Gets/set the frequency of the pulse in Hz.
		/// </summary>
		public double PulseFrequency { get; set; } = 100d;

		/// <summary>
		/// Gets the total width/length in μs (micro-seconds) of the pulse.
		/// </summary>
		public double PulseWidth
		{
			get
			{
				// ***
				// *** Check if disposed and throw an exception 
				// *** if this instance has been disposed.
				// ***
				this.CheckDisposed();

				return 1d / this.PulseFrequency * 1000d * 1000d;
			}
		}

		/// <summary>
		/// Gets the width/length in μs (micro-seconds) of the high pulse.
		/// </summary>
		public double HighPulseWidth
		{
			get
			{
				// ***
				// *** Check if disposed and throw an exception 
				// *** if this instance has been disposed.
				// ***
				this.CheckDisposed();

				return ((double)this.Value / this.MaximumValue) * this.PulseWidth;
			}
		}

		/// <summary>
		/// Gets the width/length in μs (micro-seconds) of the low pulse.
		/// </summary>
		public double LowPulseWidth
		{
			get
			{
				// ***
				// *** Check if disposed and throw an exception 
				// *** if this instance has been disposed.
				// ***
				this.CheckDisposed();

				return ((double)(this.MaximumValue - this.Value) / this.MaximumValue) * this.PulseWidth;
			}
		}

		/// <summary>
		/// Start the SoftPwm in the GPIO pin.
		/// </summary>
		public void StartAsync()
		{
			// ***
			// *** Check if disposed and throw an exception 
			// *** if this instance has been disposed.
			// ***
			this.CheckDisposed();

			Task.Factory.StartNew(async () =>
			{
				while (!_cancellationTokenSource.IsCancellationRequested)
				{
					// ***
					// *** Pulse High (unless the value is 0 in which case
					// *** the output will stay low.
					// ***
					if (this.Value != this.MinimumValue)
					{
						this.Pin.Write(GpioPinValue.High);
					}

					// ***
					// *** Delay the for the time specified by HighPulseWidth
					// ***
					await this.DelayMicroSeconds(this.HighPulseWidth);

					// ***
					// *** Pulse Low unless the value is at maximum in which
					// *** case the pulse will remain high.
					// ***
					if ((this.MaximumValue - this.Value) != this.MinimumValue)
					{
						this.Pin.Write(GpioPinValue.Low);
					}

					// ***
					// *** Delay the for the time specified by LowPulseWidth
					// ***
					await this.DelayMicroSeconds(this.LowPulseWidth);

					// ***
					// *** Check if the PulseWidthChanged event needs to be fired.
					// ***
					if (this.LowPulseWidth != _previousLowPulseWidth || this.HighPulseWidth != _previousHighPulseWidth)
					{
						_previousLowPulseWidth = this.LowPulseWidth;
						_previousHighPulseWidth = this.HighPulseWidth;						
						this.OnPulseWidthChanged(this.HighPulseWidth, this.LowPulseWidth);
					}

					// ***
					// *** Fire the Pulsed event (monitoring of this event
					// *** can impact the performance of the application and 
					// *** the ability of this code to keep the timing
					// *** correct.
					// ***
					this.OnPwmPulsed();
				}
			});
		}

		/// <summary>
		/// Stop the SoftPwm on the GPIO pin.
		/// </summary>
		/// <returns></returns>
		public async Task StopAsync()
		{
			// ***
			// *** Check if disposed and throw an exception 
			// *** if this instance has been disposed.
			// ***
			this.CheckDisposed();

			// ***
			// *** Call cancel to stop the loop which will
			// *** allow it to drop out and stop.
			// ***
			_cancellationTokenSource.Cancel();

			// ***
			// *** Wait for task to complete.
			// ***
			await _pulserTask;
		}

		/// <summary>
		/// Gets/sets the current value.
		/// </summary>
		public double Value
		{
			get
			{
				// ***
				// *** Check if disposed and throw an exception 
				// *** if this instance has been disposed.
				// ***
				this.CheckDisposed();

				return _value;
			}
			set
			{
				// ***
				// *** Check if disposed and throw an exception 
				// *** if this instance has been disposed.
				// ***
				this.CheckDisposed();

				_value = value;
				if (_value < this.MinimumValue)
				{
					_value = this.MinimumValue;
				}

				if (_value > this.MaximumValue)
				{
					_value = this.MaximumValue;
				}
			}
		}

		/// <summary>
		/// Stops the SoftPwm if active and calls Dispose on the GPIO pin.
		/// </summary>
		public void Dispose()
		{
			// ***
			// *** Check if disposed and throw an exception 
			// *** if this instance has been disposed.
			// ***
			this.CheckDisposed();

			this.StopAsync().Wait();
			this.Pin.Dispose();
			this.Pin = null;
		}

		/// <summary>
		/// Checks if this instance has been disposed and 
		/// throws the ObjectDisposedException exception if it is.
		/// </summary>
		private void CheckDisposed()
		{
			if (this.Pin == null) { throw new ObjectDisposedException(nameof(SoftPwm)); }
		}

		/// <summary>
		/// Delays the current thread by the given number of μs.
		/// </summary>
		/// <param name="delayMicroseconds">The number of μs to delay the thread.</param>
		/// <returns>Returns an awaitable Task instance.</returns>
		private async Task DelayMicroSeconds(double delayMicroseconds)
		{
			TimeSpan delay = TimeSpan.FromTicks(10 * (int)delayMicroseconds);
			await Task.Delay(delay);
		}

		/// <summary>
		/// Called to raise the PulseWidthChanged event.
		/// </summary>
		protected virtual void OnPulseWidthChanged(double highPulseWidth, double lowPulseWidth)
		{
			if (this.PulseWidthChanged != null)
			{
				this.PulseWidthChanged(this, new PulseWidthChangedEventArgs(highPulseWidth, lowPulseWidth));
			}
		}

		/// <summary>
		/// Called to raise the PwmPulsed event
		/// </summary>
		protected virtual void OnPwmPulsed()
		{
			if (this.PwmPulsed != null)
			{
				this.PwmPulsed(this, new EventArgs());
			}
		}
    }
}
