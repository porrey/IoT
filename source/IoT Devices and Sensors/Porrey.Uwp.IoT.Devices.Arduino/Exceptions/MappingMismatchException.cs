namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public class MappingMismatchException : ArduinoException
	{
		public MappingMismatchException() 
			: base("The Register ID of the command and the command mapping are out of sync.")
		{
		}
	}
}
