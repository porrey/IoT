// Copyright © 2015 Daniel Porrey. All Rights Reserved.
//
// This file is part of the IoT Devices and Sensors project.
// 
// IoT Devices and Sensors is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// IoT Devices and Sensors is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with IoT Devices and Sensors. If not, 
// see http://www.gnu.org/licenses/.
//
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
