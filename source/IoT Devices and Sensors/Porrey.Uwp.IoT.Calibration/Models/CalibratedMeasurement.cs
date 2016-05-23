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
using System;

namespace Porrey.Uwp.IoT.Calibration
{
	public class CalibratedMeasurement : ICalibratedMeasurement
	{
		/// <summary>
		/// The number of calibration poinmt expected for this implementation of ICalibratedMeasurement.
		/// </summary>
		private int _calibrationPointCount = 0;

		/// <summary>
		/// Contains the points used for calibration.
		/// </summary>
		private CalibrationPoint[] _calibrationPoints = new CalibrationPoint[0];

		/// <summary>
		/// Creates an instance of Porrey.Uwp.IoT.Calibration
		/// with the specified point count and maximum reading value.
		/// </summary>
		/// <param name="pointCount">The required number of calibration points.</param>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		public CalibratedMeasurement(int pointCount, float maximum)
		{
			this.CalibrationPointCount = pointCount;
			this.CalibrationPoints = new CalibrationPoint[pointCount];
			this.Minimum = 0f;
			this.Maximum = maximum;
		}

		/// <summary>
		/// Creates an instance of Porrey.Uwp.IoT.Calibration
		/// with the specified point count and minimum and maximum 
		/// reading values. 
		/// </summary>
		/// <param name="pointCount">The required number of calibration points.</param>
		/// <param name="minimum">The minimum adjusted reading value allowed.</param>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		public CalibratedMeasurement(int pointCount, float minimum, float maximum)
		{
			this.CalibrationPointCount = pointCount;
			this.CalibrationPoints = new CalibrationPoint[pointCount];
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		/// <summary>
		/// Creates an instance of Porrey.Uwp.IoT.Calibration
		/// with the specified point count, maximum 
		/// reading values and calibration points.
		/// </summary>
		/// <param name="pointCount">The required number of calibration points.</param>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		/// <param name="calibrationPoints">The calibrations points used to adjust the reading.</param>
		public CalibratedMeasurement(int pointCount, float maximum, CalibrationPoint[] calibrationPoints)
		{
			this.CalibrationPointCount = pointCount;
			this.Maximum = maximum;
			this.CalibrationPoints = calibrationPoints;
		}

		/// <summary>
		/// Creates an instance of Porrey.Uwp.IoT.Calibration
		/// with the specified point count, minimum and maximum 
		/// reading values and calibration points.
		/// </summary>
		/// <param name="pointCount">The required number of calibration points.</param>
		/// <param name="minimum">The minimum adjusted reading value allowed.</param>
		/// <param name="maximum">The maximum adjusted reading value allowed.</param>
		/// <param name="calibrationPoints">The calibrations points used to adjust the reading.</param>
		public CalibratedMeasurement(int pointCount, float minimum, float maximum, CalibrationPoint[] calibrationPoints)
		{
			this.CalibrationPointCount = pointCount;
			this.Minimum = minimum;
			this.Maximum = maximum;
			this.CalibrationPoints = calibrationPoints;
		}

		/// <summary>
		/// Gets the number of calibration points expected for this 
		/// implementation of ICalibratedMeasurement.
		/// </summary>
		public int CalibrationPointCount
		{
			get
			{
				return _calibrationPointCount;
            }
			protected set
			{
				_calibrationPointCount = value;
            }
		}

		/// <summary>
		/// Gets sets the minimum adjusted reading.
		/// </summary>
		public virtual float Minimum { get; set; }

		/// <summary>
		/// Gets sets the maximum adjusted reading.
		/// </summary>
		public virtual float Maximum { get; set; }

		/// <summary>
		/// Gets the three calibration point used to calibrate
		/// the device measurement.
		/// </summary>
		public virtual CalibrationPoint[] CalibrationPoints
		{
			get
			{
				// ***
				// *** Return the current value.
				// ***
				return _calibrationPoints;
            }
			set
			{
				if (value.Length == this.CalibrationPointCount)
				{
					// ***
					// *** Calculate and store the values needed
					// *** for the equation.
					// ***
					this.OnCalibrationPointsChanged(value);

					// ***
					// *** Store the value
					// ***
					_calibrationPoints = value;
				}
				else if (value.Length < this.CalibrationPointCount)
				{
					// ***
					// *** Throw a specific exception letting the caller 
					// *** know there are less than the three required points.
					// ***
					throw new ArgumentOutOfRangeException(string.Format("There are too few points defined. {0} must be an array of {1} points.", nameof(CalibrationPoints), this.CalibrationPointCount));
				}
				else
				{
					// ***
					// *** Throw a specific exception letting the caller 
					// *** know there are more than the three required points.
					// ***
					throw new ArgumentOutOfRangeException(string.Format("There are too many points defined. {0} must be an array of {1} points.", nameof(CalibrationPoints), this.CalibrationPointCount));
				}
			}
		}

		/// <summary>
		/// Returns the current adjusted reading the device. The 
		/// reading is a value between 0 and 1.
		/// </summary>
		/// <returns>Returns the adjusted sensor reading.</returns>
		public virtual float AdjustedReading(float x)
		{
			float returnValue = 0f;

			returnValue = this.OnAdjustedReading(x).Maximum(this.Maximum).Minimum(this.Minimum);

			return returnValue;
		}

		/// <summary>
		/// Called to get the adjusted value from the concrete class.
		/// </summary>
		/// <param name="x">The sensor reading.</param>
		/// <returns></returns>
		protected virtual float OnAdjustedReading(float x)
		{
			return 0f;
		}

		/// <summary>
		/// Called when the calibration points are changed.
		/// </summary>
		/// <param name="calibrationPoints">The new calibration points.</param>
		protected virtual void OnCalibrationPointsChanged(CalibrationPoint[] calibrationPoints)
		{
		}
    }
}
