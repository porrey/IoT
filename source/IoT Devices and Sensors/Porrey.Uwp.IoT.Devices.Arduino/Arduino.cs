using System;
using System.Threading.Tasks;
using Porrey.Uwp.IoT.Sensors;

namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public enum ArduinoRegister
	{
		PinMode = 0,
		DigitalRead = 1,
		DigitalWrite = 2,
		AnalogRead = 3,
		AnalogWrite = 4,
		Tone = 5,
		NoTone = 6
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
			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.PinMode);
            await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, (byte)pinMode });
		}

		public async Task DigitalWrite(byte pin, ArduinoPinValue value)
		{
			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.DigitalWrite);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, (byte)value });
		}

		public async Task AnalogWrite(byte pin, byte value)
		{
			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.AnalogWrite);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, value });
		}

		public async Task<ArduinoPinValue> DigitalRead(byte pin)
		{
			ArduinoPinValue returnValue = ArduinoPinValue.Low;

			byte[] readBuffer = new byte[1];
			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.DigitalRead);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin });

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
			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.DigitalRead);

			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, frequencyBytes[0], frequencyBytes[1], durationBytes[0], durationBytes[1], durationBytes[2], durationBytes[3] });
		}

		public async Task Tone(byte pin, UInt16 frequency)
		{
			// ***
			// *** 2 bytes
			// ***
			byte[] frequencyBytes = BitConverter.GetBytes(frequency);
			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.DigitalRead);

			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, frequencyBytes[0], frequencyBytes[1]});
		}

		public async Task NoTone(byte pin)
		{
			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.DigitalRead);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin });
		}
	}
}
