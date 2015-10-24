using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Porrey.Uwp.IoT.System
{
	[StructLayout(LayoutKind.Sequential)]
	public struct SYSTEMTIME
	{
		[MarshalAs(UnmanagedType.U2)]
		public ushort wYear;
		[MarshalAs(UnmanagedType.U2)]
		public ushort wMonth;
		[MarshalAs(UnmanagedType.U2)]
		public ushort wDayOfWeek;
		[MarshalAs(UnmanagedType.U2)]
		public ushort wDay;
		[MarshalAs(UnmanagedType.U2)]
		public ushort wHour;
		[MarshalAs(UnmanagedType.U2)]
		public ushort wMinute;
		[MarshalAs(UnmanagedType.U2)]
		public ushort wSecond;
		[MarshalAs(UnmanagedType.U2)]
		public ushort wMilliseconds;
	}

	public static class SystemDateTime
	{
		[DllImport("kernelbase.dll")]
		private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

		[DllImport("kernelbase.dll")]
		private extern static int SetSystemTime(ref SYSTEMTIME lpSystemTime);

		[DllImport("kernelbase.dll")]
		private extern static int GetLastError();

		public static Task<int> Set(DateTimeOffset datetime)
		{
			SYSTEMTIME systime = new SYSTEMTIME();

			systime.wYear = (ushort)datetime.Year;
			systime.wMonth = (ushort)datetime.Month;
			systime.wDay = (ushort)datetime.Day;
			systime.wDayOfWeek = (ushort)datetime.DayOfWeek;
			systime.wHour = (ushort)datetime.Hour;
			systime.wMinute = (ushort)datetime.Minute;
			systime.wSecond = (ushort)datetime.Second;
			systime.wMilliseconds = (ushort)datetime.Millisecond;

			int result = SetSystemTime(ref systime);

			if (result == 0)
			{
				result = SystemDateTime.GetLastError();
				string errorMessage = new Win32Exception(result).Message;
				throw new Exception(errorMessage);
			}

			return Task.FromResult(result);
		}
	}
}
