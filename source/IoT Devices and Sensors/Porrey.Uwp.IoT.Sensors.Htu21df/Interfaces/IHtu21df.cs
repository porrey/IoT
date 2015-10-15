using System.Threading.Tasks;

namespace Porrey.Uwp.IoT.Sensors
{
	public interface IHtu21df
	{
		Task<Htu21df.BatteryStatus> GetBatteryStatus();
		Task<Htu21df.HeaterStatus> GetHeaterStatus();
		Task<Htu21df.Resolution> GetResolution();
		Task<float> ReadHumidityAsync();
		Task<float> ReadTemperatureAsync();
		Task<bool> SetHeaterStatus(Htu21df.HeaterStatus heaterStatus);
		Task<bool> SetResolution(Htu21df.Resolution resolution);
	}
}