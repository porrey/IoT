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
namespace Porrey.Uwp.IoT.Calibration
{
    /// <summary>
    /// Represents an x- and y-coordinate pair in two-dimensional space.
    /// </summary>
    public struct CalibrationPoint
	{
		/// <summary>
		/// Gets or sets the X-coordinate value of 
		/// this Porrey.Uwp.IoT.CalibrationPoint structure.
		/// </summary>
		public float X { get; set; }

		/// <summary>
		/// Gets or sets the Y-coordinate value of 
		/// this Porrey.Uwp.IoT.CalibrationPoint structure.
		/// </summary>
		public float Y { get; set; }
	}
}
