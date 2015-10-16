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

namespace Porrey.Uwp.IoT.Devices.KeyPad
{
	public abstract class GpioPinMapping : IGpioPinMapping
	{
		public GpioPinMapping(GpioMappingType mappingType, int matrixValue, int pinNumber)
		{
			this.MappingType = mappingType;
			this.MatrixValue = matrixValue;
			this.PinNumber = pinNumber;
		}

		public GpioMappingType MappingType { get; set; }
		public int MatrixValue { get; set; }
		public int PinNumber { get; set; }
		public GpioPin GpioPin { get; set; }
		public GpioPinValue PreviousValue { get; set; }

		public void InitializeColumn(TimeSpan debounceTime)
		{
			this.GpioPin = GpioController.GetDefault().OpenPin(this.PinNumber);
            this.GpioPin.SetDriveMode(GpioPinDriveMode.InputPullDown);
			this.PreviousValue = this.GpioPin.Read();
			this.GpioPin.DebounceTimeout = debounceTime;
		}

		public void InitializeRow()
		{
			this.GpioPin = GpioController.GetDefault().OpenPin(this.PinNumber);
			this.GpioPin.SetDriveMode(GpioPinDriveMode.Output);
			this.GpioPin.Write(GpioPinValue.High);
		}

		public PinChangedStatus GetChangedStatus(GpioPinEdge edge)
		{
			PinChangedStatus returnValue = PinChangedStatus.None;
			GpioPinValue currentValue = this.GpioPin.Read();

			try
			{
				if (edge == GpioPinEdge.RisingEdge && this.PreviousValue == GpioPinValue.Low && currentValue != this.PreviousValue)
				{
					returnValue = PinChangedStatus.WentHigh;
				}
				else if (edge == GpioPinEdge.FallingEdge && this.PreviousValue == GpioPinValue.High && currentValue != this.PreviousValue)
				{
					returnValue = PinChangedStatus.WentLow;
				}
			}
			finally
			{
				this.PreviousValue = currentValue;
			}

			return returnValue;
		}

		public void Dispose()
		{
		}
	}
}
