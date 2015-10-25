using System;
using System.Threading.Tasks;
using Porrey.Uwp.IoT.Sensors;

namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public class Arduino : I2c, IArduino
	{
		private const int DELAY = 100;

		public Arduino(byte address)
			: base(address)
		{
		}

		public async Task<bool> PinModeAsync(byte pin, ArduinoPinMode pinMode)
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.PinMode);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, (byte)pinMode });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<ArduinoPinValue> DigitalReadAsync(byte pin)
		{
			ArduinoPinValue returnValue = ArduinoPinValue.Unknwown;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.DigitalRead);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin });

			// ***
			// *** Read the result
			// ***
			byte[] readBuffer = await this.GetReadBuffer(2);

			if (await this.CheckResult(readBuffer))
			{
				returnValue = readBuffer[1] == 1 ? ArduinoPinValue.High : ArduinoPinValue.Low;
			}

			return returnValue;
		}

		public async Task<bool> DigitalWriteAsync(byte pin, ArduinoPinValue value)
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.DigitalWrite);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, (byte)value });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<ushort> AnalogReadAsync(byte pin)
		{
			UInt16 returnValue = ushort.MaxValue;
			
			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.AnalogRead);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin });

			// ***
			// *** Get the result by reading form the bus
			// ***
			byte[] readBuffer = await this.GetReadBuffer(3);

			if (await this.CheckResult(readBuffer))
			{
				returnValue = BitConverter.ToUInt16(readBuffer, 1);
			}

			return returnValue;
		}

		public async Task<bool> AnalogWriteAsync(byte pin, byte value)
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.AnalogWrite);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, value });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<bool> AnalogReferenceAsync(ArduinoAnalogReferenceType type)
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.AnalogReference);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], (byte)type });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

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
			uint durationInMilliseconds = (ushort)duration.TotalMilliseconds;
			byte[] durationBytes = BitConverter.GetBytes(durationInMilliseconds);
			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.Tone1);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, frequencyBytes[0], frequencyBytes[1], durationBytes[0], durationBytes[1], durationBytes[2], durationBytes[3] });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<bool> ToneAsync(byte pin, ushort frequency)
		{
			bool returnValue = false;

			// ***
			// *** 2 bytes
			// ***
			byte[] frequencyBytes = BitConverter.GetBytes(frequency);
			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.Tone2);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, frequencyBytes[0], frequencyBytes[1] });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<bool> NoToneAsync(byte pin)
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.NoTone);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<bool> ShiftOutAsync(byte dataPin, byte clockPin, ArduinoBitOrder bitOrder, byte value)
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.ShiftOut);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], dataPin, clockPin, (byte)bitOrder, value });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<bool> EnableInterruptsAsync()
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.Interrupts);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1] });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<bool> DisableInterruptsAsync()
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.NoInterrupts);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1] });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<byte[]> CustomCommandAsync(int registerId, byte[] data, int resultBufferSize)
		{
			byte[] returnValue = new byte[resultBufferSize];

			// ***
			// *** Write the data
			// ***
			await this.WriteAsync(await this.BuildWriteBuffer(registerId, data));

			// ***
			// *** Delay and read
			// ***
			await Task.Delay(DELAY);
			await this.ReadAsync(returnValue);

			return returnValue;
		}

		public async Task<bool> BreatheLedAsync(byte pin, byte rate, byte step, byte offValue)
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.BreatheLed);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin, rate, step, offValue });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<bool> NoBreatheLedAsync(byte pin)
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.NoBreatheLed);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<bool> PulsePinAsync(byte pin, ArduinoPinValue offValue, TimeSpan onDuration, TimeSpan offDuration)
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.PulsePin);

			uint highDurationInMilliseconds = (ushort)onDuration.TotalMilliseconds;
			byte[] highDurationBytes = BitConverter.GetBytes(highDurationInMilliseconds);

			uint lowDurationInMilliseconds = (ushort)offDuration.TotalMilliseconds;
			byte[] lowDurationBytes = BitConverter.GetBytes(lowDurationInMilliseconds);

			await this.WriteAsync(new byte[]
			{
				registerId[0], registerId[1],
				pin,
				(byte)offValue,
                highDurationBytes[0], highDurationBytes[1], highDurationBytes[2], highDurationBytes[3],
				lowDurationBytes[0], lowDurationBytes[1], lowDurationBytes[2], lowDurationBytes[3]
			});

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		public async Task<bool> NoPulsePinAsync(byte pin)
		{
			bool returnValue = false;

			byte[] registerId = BitConverter.GetBytes((uint)ArduinoRegister.NoPulsePin);
			await this.WriteAsync(new byte[] { registerId[0], registerId[1], pin });

			// ***
			// *** Check if the call was successful
			// ***
			if (await this.CheckResult())
			{
				returnValue = true;
			}

			return returnValue;
		}

		protected Task<byte[]> BuildWriteBuffer(int registerId, byte[] data)
		{
			byte[] returnValue = new byte[2 + data.Length];

			// ***
			// *** Build the write buffer
			// ***
			byte[] registerIdBytes = BitConverter.GetBytes((uint)registerId);
			returnValue[0] = registerIdBytes[0];
			returnValue[1] = registerIdBytes[1];
			Array.Copy(data, 0, returnValue, 2, data.Length);

			return Task<byte[]>.FromResult(returnValue);
		}

		protected async Task<byte[]> GetReadBuffer(int length = 1)
		{
			byte[] readBuffer = new byte[length];

			await Task.Delay(DELAY);			
			await this.ReadAsync(readBuffer);

			return readBuffer;
		}

		protected async Task<bool> CheckResult(bool throwException = true)
		{
			byte[] readBuffer = await this.GetReadBuffer();
            return await this.CheckResult(readBuffer, throwException);
		}

		protected Task<bool> CheckResult(byte[] readBuffer, bool throwException = true)
		{
			bool returnValue = false;

			ArduinoResult result = (ArduinoResult)readBuffer[0];

			if (result == ArduinoResult.Success)
			{
				returnValue = true;
			}
			else
			{
				if (throwException)
				{
					this.ThrowException(result);
				}
				else
				{
					returnValue = false;
				}
			}

			return Task<bool>.FromResult(returnValue);
		}

		protected void ThrowException(ArduinoResult result)
		{
			switch (result)
			{
				case ArduinoResult.Unknown:
					throw new ResultUnknownException();
				case ArduinoResult.Success:
					// ***
					// *** Do nothing
					// ***
					break;
				case ArduinoResult.BufferSizeNotExpected:
					throw new BufferSizeNotExpectedException();
				case ArduinoResult.BufferError:
					throw new BufferErrorException();
				case ArduinoResult.CommandNotSupported:
					throw new CommandNotSupportedException();
				case ArduinoResult.MappingMismatch:
					throw new MappingMismatchException();
				case ArduinoResult.MissingCallback:
					throw new MissingMappingException();
				case ArduinoResult.IdTooLarge:
					throw new RegisterIdTooLargeException();
			}
		}
	}
}
