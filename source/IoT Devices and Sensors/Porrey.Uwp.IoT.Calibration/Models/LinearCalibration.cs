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

namespace Porrey.Uwp.IoT.Calibration
{
	/// <summary>
	/// Allows calibration of a device using a linear equation
	/// in the form y = mx + b.
	/// </summary>
	public class LinearCalibration : CalibratedMeasurement
	{
		/// <summary>
		/// The number of calibration points required by this calibration method.
		/// </summary>
		private const int _calibrationPointCount = 2;

		/// <summary>
		/// Represents the value m in the formula y = mx + b
		/// </summary>
		private float _m = 0f;

		/// <summary>
		/// Represents the value b in the formula y = mx + b
		/// </summary>
		private float _b = 0f;

		/// <summary>
		/// Creates an instance of Windows.Devices.Sensors.NonLinearCalibration
		/// with the specified point count and maximum reading value.
		/// </summary>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		public LinearCalibration(float maximum)
			: base(_calibrationPointCount, maximum)
		{
		}

		/// <summary>
		/// Creates an instance of Windows.Devices.Sensors.NonLinearCalibration
		/// with the specified point count and minimum and maximum 
		/// reading values. 
		/// </summary>
		/// <param name="minimum">The minimum adjusted reading value allowed.</param>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		public LinearCalibration(float minimum, float maximum)
			: base(_calibrationPointCount, minimum, maximum)
		{
		}

		/// <summary>
		/// Creates an instance of Windows.Devices.Sensors.NonLinearCalibration
		/// with the specified point count, maximum 
		/// reading values and calibration points.
		/// </summary>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		/// <param name="calibrationPoints">The calibrations points used to adjust the reading.</param>
		public LinearCalibration(float maximum, CalibrationPoint[] calibrationPoints)
			: base(_calibrationPointCount, maximum, calibrationPoints)
		{
		}

		/// <summary>
		/// Creates an instance of Windows.Devices.Sensors.NonLinearCalibration
		/// with the specified point count, minimum and maximum 
		/// reading values and calibration points.
		/// </summary>
		/// <param name="minimum">The minimum adjusted reading value allowed.</param>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		/// <param name="calibrationPoints">The calibrations points used to adjust the reading.</param>
		public LinearCalibration(float minimum, float maximum, CalibrationPoint[] calibrationPoints)
			: base(_calibrationPointCount, minimum, maximum, calibrationPoints)
		{
		}

		protected override float OnAdjustedReading(float x)
		{
			return (_m * x) + _b;
		}

		protected override void OnCalibrationPointsChanged(CalibrationPoint[] calibrationPoints)
		{
			this.CalculateFormulaVariables(calibrationPoints, out _m, out _b);
		}

		/// <summary>
		/// Calculates the values a, b and c in the formula y = mx + b
		/// </summary>
		public virtual void CalculateFormulaVariables(CalibrationPoint[] calibrationPoints, out float m, out float b)
		{
			if (calibrationPoints.Length == this.CalibrationPointCount)
			{
				// ***
				// *** These value are cast into variables here to hopefully
				// *** makes this more readable
				// ***
				float x1 = calibrationPoints[0].X;
				float x2 = calibrationPoints[1].X;

				float y1 = calibrationPoints[0].Y;
				float y2 = calibrationPoints[1].Y;

				m = (y2 - y1) / (x2 - x1);

				float b1 = m * x1;
				float b2 = m * x2;

				b = (b1 + b2) / 2;
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
