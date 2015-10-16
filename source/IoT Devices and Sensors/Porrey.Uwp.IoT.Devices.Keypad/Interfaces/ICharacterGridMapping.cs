namespace Porrey.Uwp.IoT.Devices.KeyPad
{
	public interface ICharacterGridMapping
	{
		int Column { get; set; }
		int Row { get; set; }
		char Value { get; set; }
	}
}