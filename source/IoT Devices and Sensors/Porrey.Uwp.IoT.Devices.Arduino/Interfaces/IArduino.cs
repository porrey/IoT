using System;
using System.Threading.Tasks;

namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public interface IArduino
	{
		Task<ushort> AnalogReadAsync(byte pin);
		Task<bool> AnalogWriteAsync(byte pin, byte value);
		Task<ArduinoPinValue> DigitalReadAsync(byte pin);
		Task<bool> DigitalWriteAsync(byte pin, ArduinoPinValue value);
		Task<bool> NoToneAsync(byte pin);
		Task<bool> PinModeAsync(byte pin, ArduinoPinMode pinMode);
		Task<bool> ToneAsync(byte pin, ushort frequency);
		Task<bool> ToneAsync(byte pin, ushort frequency, TimeSpan duration);
	}
}