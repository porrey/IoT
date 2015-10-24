namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public class BufferErrorException : ArduinoException
	{
		public BufferErrorException() 
			: base("An error occurred while reading the bytes from the I2C bus.")
		{
		}
	}
}
