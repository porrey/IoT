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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Porrey.Uwp.IoT.SensorStability
{
	/// <summary>
	/// Measures the stability of multiple sensor readings over a period of time to determine
	/// if the reading is stable. This is useful, for example, for a sensor such as a scale where
	/// the weight will vary until while an object is placed on the scale but will eventually
	/// stabilize. This class aids in determining when such a sensor has a stable value.
	/// </summary>
	public class StabilityMonitor
	{
		private int _sampleSize = 10;
		private readonly List<StabilityReading<double>> _readings = new List<StabilityReading<double>>();

		/// <summary>
		/// Creates an instance of Windows.Devices.Sensors.SensorStability.StabilityMonitor
		/// with the specified Maximum Consecutive readings required for a stable sensor 
		/// reading.
		/// </summary>
		/// <param name="sampleSize">Specifies the number of samples to collect.</param>
		public StabilityMonitor(int sampleSize = 10)
		{
			this.SampleSize = sampleSize;
		}

		/// <summary>
		/// Gets/sets the number of samples to collect to determine the
		/// stability of the sensor reading.
		/// </summary>
		public int SampleSize
		{
			get
			{
				return _sampleSize;
			}

			set
			{
				this._sampleSize = value;
			}
		}

		/// <summary>
		/// Gets a normalized value indicating the level of stability in the sensor reading.
		/// A value of zero indicates no variance in the reading samples (perfect stability).
		/// The larger the number the more unstable the readings are.
		/// </summary>
		public double Stability
		{
			get
			{
				double returnValue = 1d;

				// ***
				// *** Determine the Coefficient of Variance (normalized 
				// *** standard deviation)
				// ***
				if (this.Average > 0)
				{
					returnValue = this.StandardDeviation / this.Average;
				}

				return returnValue;
			}
		}

		/// <summary>
		/// Gets the last n consecutive readings where n is the
		/// MaximumConsecutiveReadings property.
		/// </summary>
		public List<StabilityReading<double>> Readings
		{
			get
			{
				return _readings;
			}
		}

		/// <summary>
		/// Adds a new reading to the meter and updates the stability measurement.
		/// </summary>
		/// <param name="sensorReading">The sensor reading of type T.</param>
		public Task AddReading(double sensorReading)
		{
			lock (this.Readings)
			{
				// ***
				// *** Add this value to the stack
				// ***
				this.Readings.Add(new StabilityReading<double>(DateTimeOffset.Now, sensorReading));

				// ***
				// *** Only keep the maximum number of readings in the list
				// ***
				if (this.Readings.Count > this.SampleSize)
				{
					this.Readings.Remove(Readings[0]);
				}
			}

			return Task.FromResult(0);
		}

		public IEnumerable<double> Values
		{
			get
			{
				IEnumerable<double> returnValue = new double[1] { 0d };

				if (this.Readings.Count() > 0)
				{
					returnValue = this.Readings.Select(t => t.Value);
				}

				return returnValue;
			}
		}

		public double Average
		{
			get
			{
				// ***
				// *** Calculate the average
				// ***
				return this.Values.Average();
			}
		}

		public double StandardDeviation
		{
			get
			{
				// ***
				// *** Calculate the standard deviation
				// ***
				double sum = this.Values.Sum(d => (d - this.Average) * (d - this.Average));
				return Math.Sqrt(sum / (double)this.Readings.Count());
			}
		}
	}
}
