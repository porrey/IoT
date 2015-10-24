namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public class CommandNotSupportedException : ArduinoException
	{
		public CommandNotSupportedException() 
			: base("The command is not supported or not mapped on the Arduino.")
		{
		}
	}
}
