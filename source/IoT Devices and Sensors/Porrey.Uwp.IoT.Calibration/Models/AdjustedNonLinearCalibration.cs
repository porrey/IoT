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

namespace Porrey.Uwp.IoT.Calibration
{
	/// <summary>
	/// Allows calibration of a device using a 2nd Order Polynomial
	/// in the form y = ax² + bx + c and includes an adjustment for 0.
	/// </summary>
	public class AdjustedNonLinearCalibration : CalibratedMeasurement
	{
		/// <summary>
		/// The number of calibration points required by this calibration method.
		/// </summary>
		private const int _calibrationPointCount = 4;

		/// <summary>
		/// Represents the value a in the formula y = ax² + bx + c
		/// </summary>
		private float _a = 0f;

		/// <summary>
		/// Represents the value b in the formula y = ax² + bx + c
		/// </summary>
		private float _b = 0f;

		/// <summary>
		/// Represents the value c in the formula y = ax² + bx + c
		/// </summary>
		private float _c = 0f;

		/// <summary>
		/// Creates an instance of Windows.Devices.Sensors.AdjustedNonLinearCalibration
		/// with the specified point count and maximum reading value.
		/// </summary>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		public AdjustedNonLinearCalibration(float maximum) 
			: base(_calibrationPointCount, maximum)
		{
		}

		/// <summary>
		/// Creates an instance of Windows.Devices.Sensors.AdjustedNonLinearCalibration
		/// with the specified point count and minimum and maximum 
		/// reading values. 
		/// </summary>
		/// <param name="minimum">The minimum adjusted reading value allowed.</param>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		public AdjustedNonLinearCalibration(float minimum, float maximum) 
			: base(_calibrationPointCount, minimum, maximum)
		{
		}

		/// <summary>
		/// Creates an instance of Windows.Devices.Sensors.AdjustedNonLinearCalibration
		/// with the specified point count, maximum 
		/// reading values and calibration points.
		/// </summary>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		/// <param name="calibrationPoints">The calibrations points used to adjust the reading.</param>
		public AdjustedNonLinearCalibration(float maximum, CalibrationPoint[] calibrationPoints)
			: base(_calibrationPointCount, maximum, calibrationPoints)
        {
		}

		/// <summary>
		/// Creates an instance of Windows.Devices.Sensors.AdjustedNonLinearCalibration
		/// with the specified point count, minimum and maximum 
		/// reading values and calibration points.
		/// </summary>
		/// <param name="minimum">The minimum adjusted reading value allowed.</param>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		/// <param name="calibrationPoints">The calibrations points used to adjust the reading.</param>
		public AdjustedNonLinearCalibration(float minimum, float maximum, CalibrationPoint[] calibrationPoints)
			: base(_calibrationPointCount, minimum, maximum, calibrationPoints)
		{
		}

		protected override float OnAdjustedReading(float x)
		{
			float returnValue = 0f;

			if (x <= this.CalibrationPoints[3].X)
			{
				returnValue = (x * x * _a) + (_b * x) + _c;
			}
			else
			{
				returnValue = this.CalibrationPoints[3].Y;
			}

			return returnValue;
		}

		protected override void OnCalibrationPointsChanged(CalibrationPoint[] calibrationPoints)
		{
			this.CalculateFormulaVariables(calibrationPoints, out _a, out _b, out _c);
		}

		/// <summary>
		/// Calculates the values a, b and c in the formula y = ax² + bx + c
		/// </summary>
		public virtual void CalculateFormulaVariables(CalibrationPoint[] calibrationPoints, out float a, out float b, out float c)
		{
			if (calibrationPoints.Length == this.CalibrationPointCount)
			{
				// ***
				// *** These value are cast into variables here to hopefully
				// *** makes this more readable
				// ***
				float x1 = calibrationPoints[0].X;
				float x2 = calibrationPoints[1].X;
				float x3 = calibrationPoints[2].X;

				float y1 = calibrationPoints[0].Y;
				float y2 = calibrationPoints[1].Y;
				float y3 = calibrationPoints[2].Y;

				float denom = (x1 - x2) * (x1 - x3) * (x2 - x3);
				a = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / denom;
				b = (x3 * x3 * (y1 - y2) + x2 * x2 * (y3 - y1) + x1 * x1 * (y2 - y3)) / denom;
				c = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / denom;
			}
			else if (calibrationPoints.Length < this.CalibrationPointCount)
			{
				// ***
				// *** Throw a specific exception letting the caller 
				// *** know there are less than the three required points.
				// ***
				throw new ArgumentOutOfRangeException(string.Format("There are too few points defined. {0} must be an array of three points.", nameof(calibrationPoints)));
			}
			else
			{
				// ***
				// *** Throw a specific exception letting the caller 
				// *** know there are more than the three required points.
				// ***
				throw new ArgumentOutOfRangeException(string.Format("There are too many points defined. {0} must be an array of three points.", nameof(calibrationPoints)));
			}
		}
	}
}
