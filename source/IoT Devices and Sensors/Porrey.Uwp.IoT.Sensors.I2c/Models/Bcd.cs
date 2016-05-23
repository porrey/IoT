// Copyright © 2015-2106 Daniel Porrey. All Rights Reserved.
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
