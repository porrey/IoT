using System;
using System.Threading.Tasks;
using Porrey.Uwp.IoT.Sensors;

namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public enum ArduinoRegister
	{
		PinMode = 0x00,
		DigitalRead = 0x01,
		DigitalWrite = 0x02,
		AnalogRead = 0x03,
		AnalogWrite = 0x04,
		Tone = 0x05,
		NoTone = 0x06
	}

	public enum ArduinoPinValue
	{
		High = 1,
		Low = 0
	}

	public enum ArduinoPinMode
	{
		Input = 0,
		Output = 1,
		InputPullup = 2
	}

	public class Arduino : I2c
	{
		public Arduino(byte address)
			: base(address)
		{
		}

		public async Task PinMode(byte pin, ArduinoPinMode pinMode)
		{
			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.PinMode, pin, (byte)pinMode });
		}

		public async Task DigitalWrite(byte pin, ArduinoPinValue value)
		{
			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.DigitalWrite, pin, (byte)value });
		}

		public async Task AnalogWrite(byte pin, byte value)
		{
			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.AnalogWrite, pin, value });
		}

		public async Task<ArduinoPinValue> DigitalRead(byte pin)
		{
			ArduinoPinValue returnValue = ArduinoPinValue.Low;

			byte[] readBuffer = new byte[1];
			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.DigitalRead, pin });

			// ***
			// *** Need a very short delay here for this to work
			// ***
			await Task.Delay(1);
			await this.ReadAsync(readBuffer);

			if (readBuffer[0] == 1)
			{
				returnValue = ArduinoPinValue.High;
			}

			return returnValue;
		}

		public async Task Tone(byte pin, UInt16 frequency, TimeSpan duration)
		{
			// ***
			// *** 2 bytes
			// ***
			byte[] frequencyBytes = BitConverter.GetBytes(frequency);

			// ***
			// *** 4 bytes
			// ***
			UInt32 durationInMilliseconds = (UInt32)duration.TotalMilliseconds;
			byte[] durationBytes = BitConverter.GetBytes(durationInMilliseconds);

			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.Tone, pin, frequencyBytes[0], frequencyBytes[1], durationBytes[0], durationBytes[1], durationBytes[2], durationBytes[3] });
		}

		public async Task Tone(byte pin, UInt16 frequency)
		{
			// ***
			// *** 2 bytes
			// ***
			byte[] frequencyBytes = BitConverter.GetBytes(frequency);

			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.Tone, pin, frequencyBytes[0], frequencyBytes[1]});
		}

		public async Task NoTone(byte pin)
		{
			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.NoTone, pin });
		}
	}
}
