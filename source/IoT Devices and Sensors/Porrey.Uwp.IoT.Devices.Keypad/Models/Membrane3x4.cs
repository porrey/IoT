using System;
using Windows.Devices.Gpio;

namespace Porrey.Uwp.IoT.Devices.KeyPad
{
	public class Membrane3x4 : KeyPad
	{
		public Membrane3x4(params IGpioPinMapping[] mappings)
			: base(7, mappings)
		{
		}

		protected override ICharacterGridMapping[] OnGetCharacterMappings()
		{
			return new CharacterGridMapping[]
			{
				new CharacterGridMapping() { Row = 1, Column = 1, Value = '1' },
				new CharacterGridMapping() { Row = 1, Column = 2, Value = '2' },
				new CharacterGridMapping() { Row = 1, Column = 3, Value = '3' },

				new CharacterGridMapping() { Row = 2, Column = 1, Value = '4' },
				new CharacterGridMapping() { Row = 2, Column = 2, Value = '5' },
				new CharacterGridMapping() { Row = 2, Column = 3, Value = '6' },

				new CharacterGridMapping() { Row = 3, Column = 1, Value = '7' },
				new CharacterGridMapping() { Row = 3, Column = 2, Value = '8' },
				new CharacterGridMapping() { Row = 3, Column = 3, Value = '9' },

				new CharacterGridMapping() { Row = 4, Column = 1, Value = '*' },
				new CharacterGridMapping() { Row = 4, Column = 2, Value = '0' },
				new CharacterGridMapping() { Row = 4, Column = 3, Value = '#' },
			};
		}
	}
}
