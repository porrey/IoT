// Copyright © 2015 Daniel Porrey
//
// This file is part of the Sensor Telemetry solution.
namespace System
{
	/// <summary>
	/// Helper methods for temperature conversion
	/// </summary>
	public class Temperature
	{
		/// <summary>
		/// Converts a temperature in Celsius to Fahrenheit.
		/// </summary>
		/// <param name="fahrenheit">The temperature in Celsius.</param>
		/// <returns>Returns the temperature in Fahrenheit.</returns>
		public static float ConvertToCelsius(float fahrenheit) => (fahrenheit - 32f) * 5f / 9f;

		/// <summary>
		/// Converts a temperature in Fahrenheit to Celsius.
		/// </summary>
		/// <param name="celsius">The temperature in Fahrenheit.</param>
		/// <returns>Returns the temperature in Celsius.</returns>
		public static float ConvertToFahrenheit(float celsius) => (celsius * 9f / 5f) + 32f;
	}
}
