using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Porrey.Uwp.IoT.Devices.Lifx
{
	public class LifxIdentityV1 : ILifxIdentity
	{
		private string _token = string.Empty;
		private AuthenticationHeaderValue _authentication = null;
		private readonly Uri _baseUri = new Uri("https://api.lifx.com/v1/");

		public LifxIdentityV1(string token)
		{
			this.Token = token;
		}

		public string Token
		{
			get
			{
				return _token;
			}
			set
			{
				_token = value;
				this.Authentication = new AuthenticationHeaderValue("Bearer", this.Token);
			}
		}

		public AuthenticationHeaderValue Authentication
		{
			get
			{
				return _authentication;
			}
			set
			{
				_authentication = value;
			}
		}

		public Uri BaseUri
		{
			get
			{
				return _baseUri;
			}
		}
	}
}
