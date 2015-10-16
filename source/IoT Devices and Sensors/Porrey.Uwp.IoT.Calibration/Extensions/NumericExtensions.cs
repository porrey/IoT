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
namespace Porrey.Uwp.IoT.Calibration
{
	public static class NumericExtensions
	{
		/// <summary>
		/// Normalzies value to a number between 0 and 1.
		/// </summary>
		/// <param name="value">The value to be normalized.</param>
		/// <param name="maximumValue">The maximum of value.</param>
		/// <returns>Returns value normalized between 0 and 1.</returns>
		public static float Normalize(this float value, float maximumValue)
		{
			return value / maximumValue;
		}

		/// <summary>
		/// Normalzies value to a number between 0 and 1.
		/// </summary>
		/// <param name="value">The value to be normalized.</param>
		/// <param name="maximumValue">The maximum of value.</param>
		/// <returns>Returns value normalized between 0 and 1.</returns>
		public static float Normalize(this int value, float maximumValue)
		{
			return (float)value / maximumValue;
		}

		/// <summary>
		/// Returns an adjusted value based on a calibration done
		/// to determine the maximum value of the device.
		/// </summary>
		/// <param name="value">The sensor reading to be calibrated.</param>
		/// <param name="calibratedMaximum">The maximum value based on calibration of the sensor.</param>
		/// <returns>Returns a float value adjusted based on the calibration value.</returns>
		public static float WithCalibration(this float value, float calibratedMaximum) => value / calibratedMaximum;

		/// <summary>
		/// Returns the value within the range specified.
		/// </summary>
		/// <param name="minimumValue">The lower end of the range and the minimum value returned.</param>
		/// <param name="maximumValue">The upper end of the range and the maximum value returned.</param>
		/// <returns>Returns a float value scaled value between the minimum and maximum value.</returns>
		public static float AsRange(this float value, float minimumValue, float maximumValue)
		{
			float returnValue = 0f;

			returnValue = ((value * (maximumValue - minimumValue)) + minimumValue).Maximum(maximumValue).Minimum(minimumValue);

			return returnValue;
		}

		/// <summary>
		/// Returns the value within the range specified.
		/// </summary>
		/// <param name="minimumValue">The lower end of the range and the minimum value returned.</param>
		/// <param name="maximumValue">The upper end of the range and the maximum value returned.</param>
		/// <returns>Returns an integer value scaled value between the minimum and maximum value.</returns>
		public static int AsRange(this float value, int minimumValue, int maximumValue)
		{
			int returnValue = 0;

			returnValue = ((int)((value * ((float)maximumValue - (float)minimumValue)) + (float)minimumValue)).Maximum(maximumValue).Minimum(minimumValue);

			return returnValue;
		}

		/// <summary>
		/// Returns the value read from the channel as a scaled against
		/// the specified maximum value.
		/// </summary>
		/// <param name="maximumValue">The largest allowed value.</param>
		/// <returns>Returns a float value scaled value calculated as a percentage of the 
		/// maximum value using the value.</returns>
		public static float AsScaledValue(this float value, float maximumValue) => value * maximumValue;

		/// <summary>
		/// Returns the value read from the channel as a scaled against
		/// the specified maximum value.
		/// </summary>
		/// <param name="maximumValue">The largest allowed value.</param>
		/// <returns>Returns an integer value scaled value calculated as a percentage of the 
		/// maximum value using the value.</returns>
		public static int AsScaledValue(this float value, int maximumValue) => (int)(value * maximumValue);

		/// <summary>
		/// Returns a value that is never larger than maximum.
		/// </summary>
		/// <param name="value">The value that is limited.</param>
		/// <param name="maximumValue">The maximum value.</param>
		/// <returns>If value is less than maximum than value is returned; otherwise maximumValue is returned.</returns>
		public static int Maximum(this int value, int maximumValue)
		{
			return value > maximumValue ? maximumValue : value;
		}

		/// <summary>
		/// Returns a value that is never larger than maximumValue.
		/// </summary>
		/// <param name="value">The value that is limited.</param>
		/// <param name="maximumValue">The maximum value.</param>
		/// <returns>If value is less than maximumValue than value is returned; otherwise maximumValue is returned.</returns>
		public static float Maximum(this float value, float maximumValue)
		{
			return value > maximumValue ? maximumValue : value;
		}

		/// <summary>
		/// Returns a value that is never smaller than minimumValue.
		/// </summary>
		/// <param name="value">The value that is limited.</param>
		/// <param name="maximumValue">The minimum value.</param>
		/// <returns>If value is more than minimum than value is returned; otherwise minimumValue is returned.</returns>
		public static int Minimum(this int value, int minimumValue)
		{
			return value < minimumValue ? minimumValue : value;
		}

		/// <summary>
		/// Returns a value that is never smaller than minimumValue.
		/// </summary>
		/// <param name="value">The value that is limited.</param>
		/// <param name="maximumValue">The minimum value.</param>
		/// <returns>If value is more than minimum than value is returned; otherwise minimumValue is returned.</returns>
		public static float Minimum(this float value, float minimumValue)
		{
			return value < minimumValue ? minimumValue : value;
		}
	}
}
