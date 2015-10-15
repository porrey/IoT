namespace Porrey.Uwp.IoT.Sensors.Models
{
	public class Bcd
	{
		private byte _value = new byte();

		public Bcd(int value)
		{
			_value = Bcd.FromInt(value);
		}

		public Bcd(byte value)
		{
			_value = value;
		}

		public static implicit operator int(Bcd item)
		{
			return Bcd.ToInt(item._value);
		}

		public static implicit operator Bcd(int value)
		{
			return new Bcd(value);
		}

		public static implicit operator Bcd(byte value)
		{
			return new Bcd(value);
		}

		public static byte FromInt(int value)
		{
			return (byte)((value / 10 * 16) + (value % 10));
		}

		public static int ToInt(byte value)
		{
			return ((value / 16 * 10) + (value % 16));
		}
	}
}
