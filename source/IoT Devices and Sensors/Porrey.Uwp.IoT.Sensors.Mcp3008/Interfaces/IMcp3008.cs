namespace Porrey.Uwp.IoT.Sensors
{
	public interface IMcp3008 : ISpi
	{
		Mcp3008Reading Read(IChannel channel);
	}
}