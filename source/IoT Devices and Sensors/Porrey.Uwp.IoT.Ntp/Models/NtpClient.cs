// Copyright © 2015-2017 Daniel Porrey. All Rights Reserved.
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
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Porrey.Uwp.Ntp
{
	/// <summary>
	/// Gets the current date and time from one or more network time servers.
	/// </summary>
	public class NtpClient : INtpClient
	{
		/// <summary>
		/// The port on which to communicate 
		/// </summary>
		private const string PORT = "123";

		/// <summary>
		/// Specifies the amount of time to wait for a response from
		/// the server. The default is 3 seconds.
		/// </summary>
		private TimeSpan _timeout = TimeSpan.FromSeconds(3);

		/// <summary>
		/// Gets the current date and time from the specified
		/// NTP server.
		/// </summary>
		/// <param name="server">Specifies the host name or IP address 
		/// of the NTP server to call.</param>
		/// <returns>Returns a DateTime instance containing the date and time
		/// obtained from the NTP server.</returns>
		private async Task<DateTimeOffset?> GetAsync(string server)
		{
			DateTimeOffset? returnValue = null;

			// ***
			// *** Create a DatagramSocket for the UDP message
			// ***
			DatagramSocket socket = new DatagramSocket();

			// ***
			// ***
			// ***
			ManualResetEvent _clientDone = new ManualResetEvent(false);

			// ***
			// *** Connect the callback for the response data
			// ***
			socket.MessageReceived += (s, a) =>
			{
				DataReader reader = a.GetDataReader();
				byte[] data = new byte[48];
				reader.ReadBytes(data);

				returnValue = this.ConvertDateTime(data);

				// ***
				// *** Signal that the callback is complete
				// ***
				_clientDone.Set();
			};

			// ***
			// *** Get the host name instance and connect
			// *** the socket.
			// ***
			HostName serverHost = new HostName(server);
			await socket.ConnectAsync(serverHost, PORT);

			// ***
			// *** Get a data writer for the network stream
			// ***
			DataWriter dataWriter = new DataWriter(socket.OutputStream);

			// ***
			// *** RFC 2030 
			// ***
			byte[] ntpData = new byte[48];
			ntpData[0] = 0x1B;
			dataWriter.WriteBytes(ntpData);

			// ***
			// *** Send the data
			// ***
			uint result = await dataWriter.StoreAsync();

			// ***
			// *** Wait for the callback to complete
			// ***
			if (!_clientDone.WaitOne(this.Timeout))
			{
				throw new TimeoutException();
			}

			return returnValue;
		}

		/// <summary>
		/// Gets the current date and time from the specified
		/// NTP servers.
		/// </summary>
		/// <param name="servers">Specifies the host name or IP address 
		/// of one or more NTP server to call. The result will be taken
		/// from the first server to respond.</param>
		/// <returns>Returns a DateTime instance containing the date and time
		/// obtained from the NTP server.</returns>
		public async Task<DateTimeOffset?> GetAsync(params string[] servers)
		{
			DateTimeOffset? returnValue = null;

			// ***
			// *** Collect and throw exceptions if no result is found.
			// ***
			List<Exception> exceptions = new List<Exception>();

			// ***
			// *** Create the list of tasks
			// ***
			IEnumerable<Task<DateTimeOffset?>> taskQuery = servers.Select(t => this.GetAsync(t)t);

			// ***
			// *** Execute the tasks
			// ***
			List<Task<DateTimeOffset?>> taskList = taskQuery.ToList();

			while (taskList.Count() > 0)
			{
				// ***
				// *** Select one of the results.
				// ***
				Task<DateTimeOffset?> completedTask = await Task.WhenAny(taskList);

				// ***
				// *** Remove this from the list
				// ***
				taskList.Remove(completedTask);

				// ***
				// *** Check for an exception
				// ***
				if (completedTask.Exception == null)
				{
					// ***
					// *** Get the result
					// ***
					returnValue = completedTask.Result;

					// ***
					// *** Return since we only need one result
					// ***
					break;
				}
				else
				{
					exceptions.Add(completedTask.Exception);
				}
			}

			// ***
			// *** Determine if an exception should be thrown.
			// ***
			if (exceptions.Count() > 0 && returnValue == null)
			{
				throw new AggregateException(exceptions);
			}

			return returnValue;
		}

		/// <summary>
		/// Gets/sets the amount of time to wait for a response 
		/// from the NTP server.
		/// </summary>
		public TimeSpan Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				this._timeout = value;
			}
		}

		/// <summary>
		/// Converts the byte array returned from the NTP
		/// server into a DateTime instance.
		/// </summary>
		/// <param name="data">The data returned from the NTP server.</param>
		/// <returns>Returns a DateTime instance containing the date and time
		/// obtained from the NTP server.</returns>
		private DateTimeOffset? ConvertDateTime(byte[] data)
		{
			DateTimeOffset? returnValue = null;

			byte offsetTransmitTime = 40;
			ulong intpart = 0;
			ulong fractpart = 0;

			for (int i = 0; i <= 3; i++)
			{
				intpart = 256 * intpart + data[offsetTransmitTime + i];
			}

			for (int i = 4; i <= 7; i++)
			{
				fractpart = 256 * fractpart + data[offsetTransmitTime + i];
			}

			ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);

			TimeSpan timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);

			DateTime dateTime = new DateTime(1900, 1, 1);
			dateTime += timeSpan;

			TimeSpan offsetAmount = TimeZoneInfo.Local.GetUtcOffset(dateTime);
			returnValue = (dateTime + offsetAmount);

			return returnValue;
		}
	}
}
