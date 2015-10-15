using System.Threading.Tasks;

namespace Porrey.Uwp.IoT.Devices
{
	public interface ISI1145
	{
		Task<int> GetIrAsync();
		Task<int> GetProximityAsync();
		Task<int> GetUvAsync();
		Task<int> GetVisibleAsync();
		Task<byte> ReadParam(byte parameter);
		Task WriteParameter(byte parameter, byte value);
	}
}