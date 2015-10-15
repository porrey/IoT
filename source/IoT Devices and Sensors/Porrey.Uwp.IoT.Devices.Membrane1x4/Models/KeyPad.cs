using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Devices.Gpio;

namespace Porrey.Uwp.IoT.Devices.KeyPad
{
	public abstract class KeyPad : IKeyPad
	{
		public event EventHandler<KeyPadEventArgs> KeyEvent = null;

		private int _pinCount = 0;
		private TimeSpan _debounceTimeout = TimeSpan.FromMilliseconds(25);
		private readonly List<IGpioPinMapping> _pinMappings = new List<IGpioPinMapping>();
		private ICharacterGridMapping[] _characterGridMappings = null;
		private bool _evaluatingRows = false;
		private readonly object _lock = new object();
		private ICharacterGridMapping _lastCharacterDown = null;
		private bool _enableButtonUpEvent = false;

		public KeyPad(int pinCount, IGpioPinMapping[] mappings)
		{
			this.CharacterGridMappings = this.OnGetCharacterMappings();
			this.PinCount = pinCount;

			if (mappings.Length != pinCount)
			{
				throw new ArgumentOutOfRangeException(string.Format("The number of pins required for this keypad is {0}.", pinCount));
			}

			// ***
			// *** Add the mappings to the internal list
			// ***
			this.PinMappings.AddRange(mappings);

			// ***
			// *** Initialize the rows
			// ***
			foreach (var mapping in this.RowMappings)
			{
				mapping.InitializeRow();
			}

			// ***
			// *** Initialize the columns
			// ***
			foreach (var mapping in this.ColumnMappings)
			{
				mapping.InitializeColumn(this.DebounceTimeout);
				mapping.GpioPin.ValueChanged += Pin_ValueChanged;
			}
		}

		public virtual List<IGpioPinMapping> PinMappings
		{
			get
			{
				return _pinMappings;
			}
		}

		public virtual IEnumerable<IGpioPinMapping> RowMappings
		{
			get
			{
				return _pinMappings.Where(t => t.MappingType == GpioMappingType.Row);
			}
		}

		public virtual IEnumerable<IGpioPinMapping> ColumnMappings
		{
			get
			{
				return _pinMappings.Where(t => t.MappingType == GpioMappingType.Column);
			}
		}

		public virtual int PinCount
		{
			get
			{
				return _pinCount;
			}
			protected set
			{
				this._pinCount = value;
			}
		}

		public virtual bool EnableButtonUpEvent
		{
			get
			{
				return _enableButtonUpEvent;
			}
			set
			{
				this._enableButtonUpEvent = value;
			}
		}

		public virtual TimeSpan DebounceTimeout
		{
			get
			{
				return _debounceTimeout;
			}
			set
			{
				this._debounceTimeout = value;
			}
		}

		protected ICharacterGridMapping[] CharacterGridMappings
		{
			get
			{
				return _characterGridMappings;
			}
			set
			{
				this._characterGridMappings = value;
			}
		}

		private void Pin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
		{
			lock (_lock)
			{
				if (!_evaluatingRows)
				{
					IGpioPinMapping mapping = this.PinMappings.Where(t => t.PinNumber == sender.PinNumber).SingleOrDefault();

					if (mapping != null)
					{
						PinChangedStatus status = mapping.GetChangedStatus(args.Edge);
						if (status != PinChangedStatus.None)
						{
							this.OnPinValueChanged(mapping, status);
						}
					}
				}
			}
		}

		protected virtual void OnPinValueChanged(IGpioPinMapping mapping, PinChangedStatus status)
		{
			try
			{
				// ***
				// *** Need to ignore other keypad changes until this is complete
				// ***
				_evaluatingRows = true;

				if (status == PinChangedStatus.WentHigh)
				{
					// ***
					// *** Once the column and row is known we can determine the key
					// ***
					ICharacterGridMapping characterGridMapping = null;

					// ***
					// *** Need to change each row to LOW until the pin we have changes again. This helps determine which
					// *** row is selected.
					// ***
					foreach (var rowMapping in this.RowMappings)
					{
						try
						{
							rowMapping.GpioPin.Write(GpioPinValue.Low);
							GpioPinValue newValue = mapping.GpioPin.Read();

							if (newValue == GpioPinValue.Low)
							{
								// ***
								// *** Select the character mapping
								// ***
								characterGridMapping = this.CharacterGridMappings.Where(t => t.Row == rowMapping.MatrixValue && t.Column == mapping.MatrixValue).SingleOrDefault();

								if (characterGridMapping != null)
								{
									if (this.EnableButtonUpEvent) { _lastCharacterDown = characterGridMapping; }
									break;
								}
							}
						}
						finally
						{
							// ***
							// *** Need the pin to be on high in it's default state
							// ***
							rowMapping.GpioPin.Write(GpioPinValue.High);
						}
					}

					if (characterGridMapping != null)
					{
						this.OnKeyEvent(new KeyPadEventArgs(characterGridMapping.Value, KeyDownEventType.ButtonDown));
					}
				}
				else
				{
					if (_lastCharacterDown != null)
					{
						char key = _lastCharacterDown.Value;
						_lastCharacterDown = null;
						this.OnKeyEvent(new KeyPadEventArgs(key, KeyDownEventType.ButtonUp));
					}
				}
			}
			finally
			{
				_evaluatingRows = false;
			}
		}

		protected virtual void OnKeyEvent(KeyPadEventArgs e)
		{
			if (this.KeyEvent != null)
			{
				this.KeyEvent(this, e);
			}
		}

		protected abstract ICharacterGridMapping[] OnGetCharacterMappings();

		public void Dispose()
		{
			try
			{
				foreach (var pinMapping in this.PinMappings)
				{
					pinMapping.Dispose();
				}
			}
			finally
			{
				this.OnDispose();
			}
		}

		protected virtual void OnDispose()
		{
		}
	}
}
