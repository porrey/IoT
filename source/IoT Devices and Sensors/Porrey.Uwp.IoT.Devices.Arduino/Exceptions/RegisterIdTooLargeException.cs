namespace Porrey.Uwp.IoT.Devices.Arduino
{
	public class RegisterIdTooLargeException : ArduinoException
	{
		public RegisterIdTooLargeException() 
			: base("The given Register ID is larger than the command buffer.")
		{
		}
	}
}
