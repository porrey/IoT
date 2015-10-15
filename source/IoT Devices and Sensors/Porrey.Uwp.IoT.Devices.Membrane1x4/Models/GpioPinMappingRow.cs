namespace Porrey.Uwp.IoT.Devices.KeyPad
{
	public class GpioPinMappingRow : GpioPinMapping
	{
		public GpioPinMappingRow(int row, int pinNumber)
			: base(GpioMappingType.Row, row, pinNumber)
		{
		}

		public int Row
		{
			get
			{
				return this.MatrixValue;
			}
			set
			{
				this.MatrixValue = value;
			}
		}
	}	
}
