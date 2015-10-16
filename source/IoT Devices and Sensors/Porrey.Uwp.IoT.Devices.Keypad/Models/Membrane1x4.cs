namespace Porrey.Uwp.IoT.Devices.KeyPad
{
	public class Membrane1x4 : KeyPad
	{
		public Membrane1x4(params IGpioPinMapping[] mappings)
			: base(5, mappings)
		{
		}

		protected override ICharacterGridMapping[] OnGetCharacterMappings()
		{
			return new CharacterGridMapping[]
			{
				new CharacterGridMapping() { Row = 1, Column = 1, Value = '1' },
				new CharacterGridMapping() { Row = 1, Column = 2, Value = '2' },
				new CharacterGridMapping() { Row = 1, Column = 3, Value = '3' },
				new CharacterGridMapping() { Row = 1, Column = 4, Value = '4' },
			};
		}
	}
}
