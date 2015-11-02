namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public class BufferNotReadyException : ArduinoException
	{
		public BufferNotReadyException() 
			: base("An attempt to read from the device was made before the buffer was ready.")
		{
		}
	}
}
