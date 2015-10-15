using System;
using System.Collections.Generic;

namespace Porrey.Uwp.IoT.Devices.KeyPad
{
	public interface IKeyPad : IDisposable
	{
		event EventHandler<KeyPadEventArgs> KeyEvent;

		bool EnableButtonUpEvent { get; set; }
		List<IGpioPinMapping> PinMappings { get; }
        IEnumerable<IGpioPinMapping> RowMappings { get; }
		IEnumerable<IGpioPinMapping> ColumnMappings { get; }
		int PinCount { get; }
		TimeSpan DebounceTimeout { get; set; }
	}
}