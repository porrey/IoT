using System;
using System.Linq;

namespace Porrey.Uwp.IoT.Sensors.I2C
{
	/// <summary>
	/// Helpers methods for reading and writing from
	/// the device registers.
	/// </summary>
	public static class RegisterConverter
	{
		/// <summary>
		/// Converts a 2-byte register value
		/// to a float.
		/// </summary>
		/// <param name="registerValue">A 2-byte register value
		/// read from the device to be converted.</param>
		/// <returns>Returns the register value as a float.</returns>
		public static float ToFloat(byte[] registerValue)
		{
			float returnValue = 0f;

			if (registerValue.Length == 2)
			{
				// ***
				// *** Reverse the byte order
				// ***
				byte[] buffer = registerValue.Reverse().ToArray();

				// ***
				// *** Use BitConverter to convert the value to
				// *** a 16-bit int
				// ***
				UInt16 bufferValue = BitConverter.ToUInt16(buffer, 0);

				// ***
				// *** Mask out the first 4 bits of the upper byte
				// ***
				float maskedBufferValue = bufferValue & 0x0FFF;

				// ***
				// *** Right shift the bits by 4
				// ***
				maskedBufferValue /= 16.0f;

				// ***
				// *** Check if bit 12 is 1 or 0 and compensate (if
				// *** 1 then the value is negative)
				// ***
				if ((bufferValue & 0x1000) == 0x1000)
				{
					returnValue = maskedBufferValue - 256;
				}
				else
				{
					returnValue = maskedBufferValue;
				}
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(registerValue));
			}

			return returnValue;
		}

		/// <summary>
		/// Converts a float to a 2-byte value to be written
		/// to the device register.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>Returns a 2-byte value that can be written to
		/// the device register.</returns>
		public static byte[] ToByteArray(float value)
		{
			byte[] returnValue = new byte[0];

			int signBit = 0;
			if (value < 0)
			{
				value += 256;

				// ***
				// *** Set bit 12 to 1 to indicate a negative number
				// ***
				signBit = 0x1000;
            }

			// ***
			// *** Left shift the bits by 4 and add the sign bit
			// ***
			UInt16 bufferValue = (UInt16)((value * 16f) + signBit);

			// ***
			// *** Convert the Int16 to a byte array
			// ***
			byte[] buffer = BitConverter.GetBytes(bufferValue);

			// ***
			// *** Reverse the byte order
			// ***
			returnValue = buffer.Reverse().ToArray();

			return returnValue;
		}

		/// <summary>
		/// Determines if a bit in the given position of a byte
		/// is High (1) or Low (0).
		/// </summary>
		/// <param name="value">The byte value to be checked.</param>
		/// <param name="bitIndex">The bit position from 0 to 7.</param>
		/// <returns>Returns true if the bit is High and false if the bit is Low.</returns>
		public static bool BitIsHigh(byte value, int bitIndex)
		{
			if (bitIndex < 0 || bitIndex > 7) throw new ArgumentOutOfRangeException(nameof(bitIndex));
			byte mask = (byte)Math.Pow(2, bitIndex);
			return ((value & mask) == mask);
		}

		/// <summary>
		/// Determines if a bit in the given position of a byte
		/// is High (1) or Low (0).
		/// </summary>
		/// <param name="value">The byte value to be checked.</param>
		/// <param name="bitIndex">The bit position from 0 to 7.</param>
		/// <returns>Returns true if the bit is Low and false if the bit is High.</returns>
		public static bool BitIsLow(byte value, int bitIndex)
		{
			if (bitIndex < 0 || bitIndex > 7) throw new ArgumentOutOfRangeException(nameof(bitIndex));
            byte mask = (byte)Math.Pow(2, bitIndex);
			return ((value & mask) == 0x0);
		}

		/// <summary>
		/// Gets a bit in the given position of a byte to the give
		/// value.
		/// </summary>
		/// <param name="value">The byte value to be modified.</param>
		/// <param name="bitIndex">The index of the bit to modify. This
		/// value can be 0 to 7.</param>
		/// <returns>Returns the value of the bit at the specified position as a boolean.</returns>
		public static bool GetBit(byte value, int bitIndex)
		{
			bool returnValue = false;

			if (bitIndex < 0 || bitIndex > 7) throw new ArgumentOutOfRangeException(nameof(bitIndex));
			byte mask = (byte)Math.Pow(2, bitIndex);
			returnValue = (value & mask) == mask;

			return returnValue;
		}

		/// <summary>
		/// Sets a bit in the given position of a byte to the give
		/// value.
		/// </summary>
		/// <param name="value">The byte value to be modified.</param>
		/// <param name="bitIndex">The index of the bit to modify. This
		/// value can be 0 to 7.</param>
		/// <param name="bit">The value to be set in the byte.</param>
		/// <returns>Returns the modified byte.</returns>
		public static byte SetBit(byte value, int bitIndex, bool bit)
		{
			if (bitIndex < 0 || bitIndex > 7) throw new ArgumentOutOfRangeException(nameof(bitIndex));
			byte mask = (byte)Math.Pow(2, bitIndex);

			if (bit && RegisterConverter.BitIsLow(value, bitIndex))
			{
				value += mask;
			}
			else if (!bit && RegisterConverter.BitIsHigh(value, bitIndex))
			{
				value -= mask;
			}

			return value;
		}
	}
}
