using Porrey.Uwp.IoT.Devices.KeyPad;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Keypad_Demo
{
	public sealed partial class MainPage : Page
	{
		private IKeyPad _keypad = null;

		public MainPage()
		{
			this.InitializeComponent();
			InitializeKeyPad(enableKeyupEvent: true);
		}

		private Task InitializeKeyPad(bool enableKeyupEvent)
		{
			IGpioPinMapping[] mappings = new IGpioPinMapping[]
			{
				new GpioPinMappingColumn(column: 3, pinNumber: 13),
				new GpioPinMappingColumn(column: 2, pinNumber: 16),
				new GpioPinMappingColumn(column: 1, pinNumber: 18),
				new GpioPinMappingRow(row: 4, pinNumber: 22),
				new GpioPinMappingRow(row: 3, pinNumber: 23),
				new GpioPinMappingRow(row: 2, pinNumber: 24),
				new GpioPinMappingRow(row: 1, pinNumber: 25),
			};

			// ***
			// *** Use the physical 3x4 keypad/comment
			// *** to use the emulator.
			// ***
			//_keypad = new Membrane3x4(mappings)
			//{
			//	EnableButtonUpEvent = enableKeyupEvent
			//};

			// ***
			// *** Uncomment to use the emulator/comment
			// *** to use the physical keypad.
			// ***
			_keypad = new Membrane3x4Emulator()
			{
				EnableButtonUpEvent = enableKeyupEvent
			};

			// ***
			// *** Return a task...
			// ***
			return Task.FromResult(0);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_keypad.KeyEvent += Keypad_KeyEvent;
			base.OnNavigatedTo(e);
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			_keypad.KeyEvent -= Keypad_KeyEvent;
			base.OnNavigatingFrom(e);
		}

		private async void Keypad_KeyEvent(object sender, KeyPadEventArgs e)
		{
			// ***
			// *** Select the button/border
			// ***
			Border button = null;
			switch (e.KeyValue)
			{
				case '0':
					button = this.Button_0;
					break;
				case '1':
					button = this.Button_1;
					break;
				case '2':
					button = this.Button_2;
					break;
				case '3':
					button = this.Button_3;
					break;
				case '4':
					button = this.Button_4;
					break;
				case '5':
					button = this.Button_5;
					break;
				case '6':
					button = this.Button_6;
					break;
				case '7':
					button = this.Button_7;
					break;
				case '8':
					button = this.Button_8;
					break;
				case '9':
					button = this.Button_9;
					break;
				case '*':
					button = this.Button_Star;
					break;
				case '#':
					button = this.Button_Pound;
					break;
			}

			// ***
			// *** Use the dispatcher to keep this on the foreground thread.
			// ***
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			{
				// ***
				// *** Store the current properties for border brush 
				// *** and thickness.
				// ***
				Brush previousBrush = button.BorderBrush;
				Thickness previousThickness = button.BorderThickness;

				// ***
				// *** Changed the properties, delay a short time, and
				// *** then restore them.
				// ***
				button.BorderBrush = new SolidColorBrush(e.KeyDownEventType == KeyDownEventType.ButtonDown ? Colors.Yellow : Colors.LightGreen);
				button.BorderThickness = new Thickness(8);

				await Task.Delay(150);

				button.BorderBrush = previousBrush;
				button.BorderThickness = previousThickness;
			});
		}
	}
}
