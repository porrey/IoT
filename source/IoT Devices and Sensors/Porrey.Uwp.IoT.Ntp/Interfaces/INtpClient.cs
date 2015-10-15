using System;
using System.Threading.Tasks;

namespace Porrey.Uwp.Ntp
{
	public interface INtpClient
	{
		TimeSpan Timeout { get; set; }

		Task<DateTime?> GetAsync(string server);
		Task<DateTime?> GetAsync(params string[] servers);
	}
}