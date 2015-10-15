using System.Threading.Tasks;

namespace Porrey.Uwp.IoT.Sensors
{
	/// <summary>
	/// Defines an I2C interface for devices that can be created
	/// by the sensor factory.
	/// </summary>
	public interface II2c
	{
		Task ResetAsync();
	}
}
