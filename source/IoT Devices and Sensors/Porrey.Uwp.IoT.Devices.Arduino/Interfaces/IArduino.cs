using System;
using System.Threading.Tasks;

namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public enum ArduinoRegister
	{
		PinMode = 0,
		DigitalRead = 1,
		DigitalWrite = 2,
		AnalogRead = 3,
		AnalogWrite = 4,
		AnalogReference = 5,
        Tone1 = 6,
		Tone2 = 7,
		NoTone = 8,
		ShiftOut = 9,
		Interrupts = 10,
		NoInterrupts = 11,
		BreatheLed = 12,
		NoBreatheLed = 13,
		PulsePin = 14,
		NoPulsePin = 15
	}

	public enum ArduinoPinValue
	{
		Unknwown = -1,
		High = 1,
		Low = 0
	}

	public enum ArduinoPinMode
	{
		Input = 0,
		Output = 1,
		InputPullup = 2
	}

	public enum ArduinoResult
	{
		Unknown = -1,
		Success = 0,
		BufferSizeNotExpected = 1,
		BufferError = 2,
		CommandNotSupported = 3,
		MappingMismatch = 4,
		MissingCallback = 5,
		IdTooLarge = 6
	}

	public enum ArduinoBitOrder
	{
		LsbFirst = 0,
		MsbFirst = 1
	}

	public enum ArduinoAnalogReferenceType
	{
		Default,
		Internal,
		Internal1v1,
		Internal2v56,
		External
	}

	public interface IArduino
	{
		Task<ushort> AnalogReadAsync(byte pin);
		Task<bool> AnalogWriteAsync(byte pin, byte value);
		Task<bool> AnalogReferenceAsync(ArduinoAnalogReferenceType type);
        Task<ArduinoPinValue> DigitalReadAsync(byte pin);
		Task<bool> DigitalWriteAsync(byte pin, ArduinoPinValue value);
		Task<bool> NoToneAsync(byte pin);
		Task<bool> PinModeAsync(byte pin, ArduinoPinMode pinMode);
		Task<bool> ToneAsync(byte pin, ushort frequency);
		Task<bool> ToneAsync(byte pin, ushort frequency, TimeSpan duration);
		Task<bool> ShiftOutAsync(byte dataPin, byte clockPin, ArduinoBitOrder bitOrder, byte value);
		Task<bool> EnableInterruptsAsync();
		Task<bool> DisableInterruptsAsync();
		Task<byte[]> CustomCommandAsync(int registerId, byte[] data, int resultBufferSize);
		Task<bool> BreatheLedAsync(byte pin, byte rate, byte step, byte offValue);
		Task<bool> NoBreatheLedAsync(byte pin);
		Task<bool> PulsePinAsync(byte pin, ArduinoPinValue offValue, TimeSpan onDuration, TimeSpan offDuration);
		Task<bool> NoPulsePinAsync(byte pin);
    }
}