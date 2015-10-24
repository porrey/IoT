namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public class ResultUnknownException : ArduinoException
	{
		public ResultUnknownException() 
			: base("The result of the command is unknown.")
		{
		}
	}
}
