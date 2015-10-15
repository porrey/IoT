using System;

namespace Porrey.Uwp.IoT.Sensors
{
	public class DeviceNotFoundException : I2cException
	{
		public DeviceNotFoundException(string deviceName)
			: base(string.Format("The device {0} was not found.", deviceName))
		{
		}
	}
}
