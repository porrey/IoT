using System;

namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public class ArduinoException : Exception
	{
		public ArduinoException()
			: base()
		{
		}

		public ArduinoException(string message)
			: base(message)
		{
		}

		public ArduinoException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
