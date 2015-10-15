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
	/// Gets the current date and time from one or more network servers.
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
		public async Task<DateTime?> GetAsync(string server)
		{
			DateTime? returnValue = null;

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

				returnValue = this.CreateDateTime(data);

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
		public async Task<DateTime?> GetAsync(params string[] servers)
		{
			DateTime? returnValue = null;

			// ***
			// *** Create the list of tasks
			// ***
			IEnumerable<Task<DateTime?>> taskQuery = from server in servers select this.GetAsync(server);

			// ***
			// *** Execute the tasks
			// ***
			List<Task<DateTime?>> taskList = taskQuery.ToList();

			while (taskList.Count() > 0)
			{
				// ***
				// *** Select one of the results
				// ***
				Task<DateTime?> completedTask = await Task.WhenAny(taskList);

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
					// *** return since we only need one result
					// ***
					break;
				}
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
		private DateTime? CreateDateTime(byte[] data)
		{
			DateTime? returnValue = null;

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
