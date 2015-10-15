using System;
using System.Net.Http.Headers;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public interface ILifxIdentity
	{
		AuthenticationHeaderValue Authentication { get; set; }
		Uri BaseUri { get; }
		string Token { get; set; }
	}
}