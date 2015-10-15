using System;

namespace Porrey.Uwp.IoT.Sensors
{
	public class DeviceNotInitializedException : I2cException
	{
		public DeviceNotInitializedException() 
			: base("The I2C device has not been initialized.")
		{
		}
	}
}
