using System;

namespace Porrey.Uwp.IoT.Sensors
{
	public class InvalidAddressException : I2cException
	{
		public InvalidAddressException() 
			: base("Slave address was not acknowledged.")
		{
		}
	}
}
