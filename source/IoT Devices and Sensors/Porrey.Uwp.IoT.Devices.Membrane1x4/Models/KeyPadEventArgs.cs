using System;

namespace Porrey.Uwp.IoT.Devices.KeyPad
{
	public enum KeyDownEventType
	{
		ButtonDown,
		ButtonUp
	}

	public class KeyPadEventArgs : EventArgs
	{
		public KeyPadEventArgs(char keyValue, KeyDownEventType keyDownEventType)
		{
			this.KeyValue = keyValue;
			this.KeyDownEventType = keyDownEventType;
		}

		public char KeyValue { get; protected set; }
		public KeyDownEventType KeyDownEventType { get; protected set; }
	}
}