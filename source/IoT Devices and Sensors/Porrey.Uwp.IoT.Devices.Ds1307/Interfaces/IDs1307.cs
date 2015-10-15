using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Porrey.Uwp.IoT.Devices
{
	public interface IDs1307
	{
		Ds1307.Frequency OscillatorFrequency { get; set; }
		Ds1307.OscillatorOutputMode OscillatorMode { get; set; }
		GpioPinValue OscillatorValue { get; set; }

		Task<DateTime> GetAsync();
		Task Halt();
		Task<byte[]> ReadMemory();
		Task Resume();
		Task SetAsync(DateTime value);
		Task WriteMemory(byte[] data);
	}
}