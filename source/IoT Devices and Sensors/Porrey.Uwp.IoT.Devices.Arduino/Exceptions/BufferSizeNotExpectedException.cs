namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public class BufferSizeNotExpectedException : ArduinoException
	{
		public BufferSizeNotExpectedException() 
			: base("The buffer size for the command was not expected.")
		{
		}
	}
}
