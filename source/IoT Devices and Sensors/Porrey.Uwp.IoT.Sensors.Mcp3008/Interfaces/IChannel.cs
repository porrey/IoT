namespace Porrey.Uwp.IoT.Sensors
{
	public interface IChannel
	{
		int Id { get; set; }
		InputConfiguration InputConfiguration { get; set; }
	}
}