namespace Porrey.Uwp.IoT.Devices.KeyPad
{
	public class GpioPinMappingColumn : GpioPinMapping
	{
		public GpioPinMappingColumn(int column, int pinNumber)
			: base(GpioMappingType.Column, column, pinNumber)
		{
		}

		public int Column
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
