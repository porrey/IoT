using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Porrey.Uwp.IoT.System
{
	public struct SYSTEMTIME
	{
		public ushort wYear;
		public ushort wMonth;
		public ushort wDayOfWeek;
		public ushort wDay;
		public ushort wHour;
		public ushort wMinute;
		public ushort wSecond;
		public ushort wMilliseconds;
	}

	public static class SystemDateTime
	{
		[DllImport("coredll.dll")]
		public extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

		[DllImport("kernel32.dll")]
		public extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

		public static Task Set(DateTimeOffset datetime)
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

			uint result = SetSystemTime(ref systime);

			return Task.FromResult(0);
		}
	}
}
