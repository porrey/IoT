namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public class MissingMappingException : ArduinoException
	{
		public MissingMappingException() 
			: base("The command mapping does not have a callback defined.")
		{
		}
	}
}
