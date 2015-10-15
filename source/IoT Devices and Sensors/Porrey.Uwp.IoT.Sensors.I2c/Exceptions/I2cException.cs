using System;

namespace Porrey.Uwp.IoT.Sensors
{
	public class I2cException : Exception
	{
		public I2cException(string message) : base(message)
		{
		}

		public I2cException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
