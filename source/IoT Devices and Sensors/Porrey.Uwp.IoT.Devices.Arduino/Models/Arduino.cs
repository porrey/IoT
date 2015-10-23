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
		Unknwown = -1,
		Success = 0,
		BufferSizeNotExpected = 1
	}

	public class Arduino : I2c, IArduino
	{
		public Arduino(byte address)
			: base(address)
		{
		}

		public async Task<bool> PinModeAsync(byte pin, ArduinoPinMode pinMode)
		{
			bool returnValue = false;

			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.PinMode, pin, (byte)pinMode });

			// ***
			// *** Check if the call was successful
			// ***
			returnValue = await this.LastCallWasSuccessful();

			return returnValue;
		}

		public async Task<ArduinoPinValue> DigitalReadAsync(byte pin)
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

		public async Task<bool> DigitalWriteAsync(byte pin, ArduinoPinValue value)
		{
			bool returnValue = false;

			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.DigitalWrite, pin, (byte)value });

			// ***
			// *** Check if the call was successful
			// ***
			returnValue = await this.LastCallWasSuccessful();

			return returnValue;
		}

		public async Task<ushort> AnalogReadAsync(byte pin)
		{
			UInt16 returnValue = 0;

			byte[] readBuffer = new byte[1];
			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.AnalogRead, pin });

			// ***
			// *** Need a very short delay here for this to work
			// ***
			await Task.Delay(1);
			await this.ReadAsync(readBuffer);

			returnValue = Convert.ToUInt16(readBuffer);

			return returnValue;
		}

		public async Task<bool> AnalogWriteAsync(byte pin, byte value)
		{
			bool returnValue = false;

			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.AnalogWrite, pin, value });

			// ***
			// *** Check if the call was successful
			// ***
			returnValue = await this.LastCallWasSuccessful();

			return returnValue;
		}

		public async Task<bool> ToneAsync(byte pin, ushort frequency, TimeSpan duration)
		{
			bool returnValue = false;

			// ***
			// *** 2 bytes
			// ***
			byte[] frequencyBytes = BitConverter.GetBytes(frequency);

			// ***
			// *** 4 bytes
			// ***
			ushort durationInMilliseconds = (ushort)duration.TotalMilliseconds;
			byte[] durationBytes = BitConverter.GetBytes(durationInMilliseconds);
			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.Tone, pin, frequencyBytes[0], frequencyBytes[1], durationBytes[0], durationBytes[1], durationBytes[2], durationBytes[3] });

			// ***
			// *** Check if the call was successful
			// ***
			returnValue = await this.LastCallWasSuccessful();

			return returnValue;
		}

		public async Task<bool> ToneAsync(byte pin, ushort frequency)
		{
			bool returnValue = false;

			// ***
			// *** 2 bytes
			// ***
			byte[] frequencyBytes = BitConverter.GetBytes(frequency);
			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.Tone, pin, frequencyBytes[0], frequencyBytes[1] });

			// ***
			// *** Check if the call was successful
			// ***
			returnValue = await this.LastCallWasSuccessful();

			return returnValue;
		}

		public async Task<bool> NoToneAsync(byte pin)
		{
			bool returnValue = false;

			await this.WriteAsync(new byte[] { (byte)ArduinoRegister.NoTone, pin });

			// ***
			// *** Check if the call was successful
			// ***
			returnValue = await this.LastCallWasSuccessful();

			return returnValue;
		}

		protected async Task<ArduinoResult> GetlastResultAsync()
		{
			ArduinoResult returnValue = ArduinoResult.Unknwown;

			byte[] readBuffer = new byte[1];
			await this.ReadAsync(readBuffer);
			returnValue = (ArduinoResult)readBuffer[0];

			return returnValue;
		}

		protected async Task<bool> LastCallWasSuccessful()
		{
			// ***
			// *** Check if the call was successful
			// ***
			await Task.Delay(1);
			return (await this.GetlastResultAsync()) == ArduinoResult.Success;
		}
	}
}
