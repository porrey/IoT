using System;
using Windows.Devices.Gpio;

namespace Porrey.Uwp.IoT.Devices.KeyPad
{
	public enum PinChangedStatus
	{
		None,
		WentHigh,
		WentLow
	}

	public enum GpioMappingType
	{
		Row,
		Column
	}

	public interface IGpioPinMapping : IDisposable
	{
		GpioMappingType MappingType { get; set; }
		int PinNumber { get; set; }
		int MatrixValue { get; set; }
		GpioPin GpioPin { get; set; }
		GpioPinValue PreviousValue { get; set; }

		void InitializeColumn(TimeSpan debounceTime);
		void InitializeRow();
        PinChangedStatus GetChangedStatus(GpioPinEdge edge);
    }
}