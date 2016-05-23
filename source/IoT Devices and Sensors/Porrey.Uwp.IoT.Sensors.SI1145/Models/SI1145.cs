// Copyright © 2015-2106 Daniel Porrey. All Rights Reserved.
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
using System;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace Porrey.Uwp.IoT.Sensors
{
    /// <summary>
    /// Enables direct access to the Adafruit SI1145 breakout board. This library
    /// was ported from the Adafruit Arduino library.
    /// </summary>
    public class SI1145 : I2c, ISI1145
	{
		#region Constants
		/* COMMANDS */
		public const byte SI1145_PARAM_QUERY = 0x80;
		public const byte SI1145_PARAM_SET = 0xA0;
		public const byte SI1145_NOP = 0x0;
		public const byte SI1145_RESET = 0x01;
		public const byte SI1145_BUSADDR = 0x02;
		public const byte SI1145_PS_FORCE = 0x05;
		public const byte SI1145_ALS_FORCE = 0x06;
		public const byte SI1145_PSALS_FORCE = 0x07;
		public const byte SI1145_PS_PAUSE = 0x09;
		public const byte SI1145_ALS_PAUSE = 0x0A;
		public const byte SI1145_PSALS_PAUSE = 0xB;
		public const byte SI1145_PS_AUTO = 0x0D;
		public const byte SI1145_ALS_AUTO = 0x0E;
		public const byte SI1145_PSALS_AUTO = 0x0F;
		public const byte SI1145_GET_CAL = 0x12;

		/* Parameters */
		public const byte SI1145_PARAM_I2CADDR = 0x00;
		public const byte SI1145_PARAM_CHLIST = 0x01;
		public const byte SI1145_PARAM_CHLIST_ENUV = 0x80;
		public const byte SI1145_PARAM_CHLIST_ENAUX = 0x40;
		public const byte SI1145_PARAM_CHLIST_ENALSIR = 0x20;
		public const byte SI1145_PARAM_CHLIST_ENALSVIS = 0x10;
		public const byte SI1145_PARAM_CHLIST_ENPS1 = 0x01;
		public const byte SI1145_PARAM_CHLIST_ENPS2 = 0x02;
		public const byte SI1145_PARAM_CHLIST_ENPS3 = 0x04;

		public const byte SI1145_PARAM_PSLED12SEL = 0x02;
		public const byte SI1145_PARAM_PSLED12SEL_PS2NONE = 0x00;
		public const byte SI1145_PARAM_PSLED12SEL_PS2LED1 = 0x10;
		public const byte SI1145_PARAM_PSLED12SEL_PS2LED2 = 0x20;
		public const byte SI1145_PARAM_PSLED12SEL_PS2LED3 = 0x40;
		public const byte SI1145_PARAM_PSLED12SEL_PS1NONE = 0x00;
		public const byte SI1145_PARAM_PSLED12SEL_PS1LED1 = 0x01;
		public const byte SI1145_PARAM_PSLED12SEL_PS1LED2 = 0x02;
		public const byte SI1145_PARAM_PSLED12SEL_PS1LED3 = 0x04;

		public const byte SI1145_PARAM_PSLED3SEL = 0x03;
		public const byte SI1145_PARAM_PSENCODE = 0x05;
		public const byte SI1145_PARAM_ALSENCODE = 0x06;

		public const byte SI1145_PARAM_PS1ADCMUX = 0x07;
		public const byte SI1145_PARAM_PS2ADCMUX = 0x08;
		public const byte SI1145_PARAM_PS3ADCMUX = 0x09;
		public const byte SI1145_PARAM_PSADCOUNTER = 0x0A;
		public const byte SI1145_PARAM_PSADCGAIN = 0x0B;
		public const byte SI1145_PARAM_PSADCMISC = 0x0C;
		public const byte SI1145_PARAM_PSADCMISC_RANGE = 0x20;
		public const byte SI1145_PARAM_PSADCMISC_PSMODE = 0x04;

		public const byte SI1145_PARAM_ALSIRADCMUX = 0x0E;
		public const byte SI1145_PARAM_AUXADCMUX = 0x0F;

		public const byte SI1145_PARAM_ALSVISADCOUNTER = 0x10;
		public const byte SI1145_PARAM_ALSVISADCGAIN = 0x11;
		public const byte SI1145_PARAM_ALSVISADCMISC = 0x12;
		public const byte SI1145_PARAM_ALSVISADCMISC_VISRANGE = 0x20;

		public const byte SI1145_PARAM_ALSIRADCOUNTER = 0x1D;
		public const byte SI1145_PARAM_ALSIRADCGAIN = 0x1E;
		public const byte SI1145_PARAM_ALSIRADCMISC = 0x1F;
		public const byte SI1145_PARAM_ALSIRADCMISC_RANGE = 0x20;

		public const byte SI1145_PARAM_ADCCOUNTER_511CLK = 0x70;

		public const byte SI1145_PARAM_ADCMUX_SMALLIR = 0x00;
		public const byte SI1145_PARAM_ADCMUX_LARGEIR = 0x03;

		/* REGISTERS */
		public const byte SI1145_REG_PARTID = 0x00;
		public const byte SI1145_REG_REVID = 0x01;
		public const byte SI1145_REG_SEQID = 0x02;

		public const byte SI1145_REG_INTCFG = 0x03;
		public const byte SI1145_REG_INTCFG_INTOE = 0x01;
		public const byte SI1145_REG_INTCFG_INTMODE = 0x02;

		public const byte SI1145_REG_IRQEN = 0x04;
		public const byte SI1145_REG_IRQEN_ALSEVERYSAMPLE = 0x01;
		public const byte SI1145_REG_IRQEN_PS1EVERYSAMPLE = 0x04;
		public const byte SI1145_REG_IRQEN_PS2EVERYSAMPLE = 0x08;
		public const byte SI1145_REG_IRQEN_PS3EVERYSAMPLE = 0x10;

		public const byte SI1145_REG_IRQMODE1 = 0x05;
		public const byte SI1145_REG_IRQMODE2 = 0x06;

		public const byte SI1145_REG_HWKEY = 0x07;
		public const byte SI1145_REG_MEASRATE0 = 0x08;
		public const byte SI1145_REG_MEASRATE1 = 0x09;
		public const byte SI1145_REG_PSRATE = 0x0A;
		public const byte SI1145_REG_PSLED21 = 0x0F;
		public const byte SI1145_REG_PSLED3 = 0x10;
		public const byte SI1145_REG_UCOEFF0 = 0x13;
		public const byte SI1145_REG_UCOEFF1 = 0x14;
		public const byte SI1145_REG_UCOEFF2 = 0x15;
		public const byte SI1145_REG_UCOEFF3 = 0x16;
		public const byte SI1145_REG_PARAMWR = 0x17;
		public const byte SI1145_REG_COMMAND = 0x18;
		public const byte SI1145_REG_RESPONSE = 0x20;
		public const byte SI1145_REG_IRQSTAT = 0x21;
		public const byte SI1145_REG_IRQSTAT_ALS = 0x01;

		public const byte SI1145_REG_ALSVISDATA0 = 0x22;
		public const byte SI1145_REG_ALSVISDATA1 = 0x23;
		public const byte SI1145_REG_ALSIRDATA0 = 0x24;
		public const byte SI1145_REG_ALSIRDATA1 = 0x25;
		public const byte SI1145_REG_PS1DATA0 = 0x26;
		public const byte SI1145_REG_PS1DATA1 = 0x27;
		public const byte SI1145_REG_PS2DATA0 = 0x28;
		public const byte SI1145_REG_PS2DATA1 = 0x29;
		public const byte SI1145_REG_PS3DATA0 = 0x2A;
		public const byte SI1145_REG_PS3DATA1 = 0x2B;
		public const byte SI1145_REG_UVINDEX0 = 0x2C;
		public const byte SI1145_REG_UVINDEX1 = 0x2D;
		public const byte SI1145_REG_PARAMRD = 0x2E;
		public const byte SI1145_REG_CHIPSTAT = 0x30;

		public const byte SI1145_ADDR = 0x60;
		public const byte SI1145_ID = 0x45;
		#endregion

		public SI1145() : base(SI1145_ADDR, I2cBusSpeed.FastMode)
		{
		}

		protected async override Task OnInitializeAsync()
		{
			byte[] id = await this.ReadRegisterBytesAsync(SI1145_REG_PARTID, 1);

			if (id[0] == SI1145_ID)
			{
				await this.ResetAsync();

				// ***
				// *** Enable UVindex measurement coefficients!
				// ***
				await this.WriteAsync(new byte[] { SI1145_REG_UCOEFF0, 0x29 });
				await this.WriteAsync(new byte[] { SI1145_REG_UCOEFF1, 0x89 });
				await this.WriteAsync(new byte[] { SI1145_REG_UCOEFF2, 0x02 });
				await this.WriteAsync(new byte[] { SI1145_REG_UCOEFF3, 0x00 });

				// ***
				// *** Enable UV sensor
				// ***
				await this.WriteParameter(SI1145_PARAM_CHLIST, SI1145_PARAM_CHLIST_ENUV | SI1145_PARAM_CHLIST_ENALSIR | SI1145_PARAM_CHLIST_ENALSVIS | SI1145_PARAM_CHLIST_ENPS1);

				// ***
				// *** Enable interrupt on every sample
				// ***
				await this.WriteAsync(new byte[] { SI1145_REG_INTCFG, SI1145_REG_INTCFG_INTOE });
				await this.WriteAsync(new byte[] { SI1145_REG_IRQEN, SI1145_REG_IRQEN_ALSEVERYSAMPLE });

				// ***
				// *** Proximity Sense 1
				// ***
				// *** program LED current
				await this.WriteAsync(new byte[] { SI1145_REG_PSLED21, 0x03 }); // 20mA for LED 1 only
				await this.WriteParameter(SI1145_PARAM_PS1ADCMUX, SI1145_PARAM_ADCMUX_LARGEIR);

				// proximity sensor #1 uses LED #1
				await this.WriteParameter(SI1145_PARAM_PSLED12SEL, SI1145_PARAM_PSLED12SEL_PS1LED1);

				// fastest clocks, clock div 1
				await this.WriteParameter(SI1145_PARAM_PSADCGAIN, 0);

				// take 511 clocks to measure
				await this.WriteParameter(SI1145_PARAM_PSADCOUNTER, SI1145_PARAM_ADCCOUNTER_511CLK);

				// in proximity mode, high range
				await this.WriteParameter(SI1145_PARAM_PSADCMISC, SI1145_PARAM_PSADCMISC_RANGE | SI1145_PARAM_PSADCMISC_PSMODE);

				await this.WriteParameter(SI1145_PARAM_ALSIRADCMUX, SI1145_PARAM_ADCMUX_SMALLIR);

				// fastest clocks, clock div 1
				await this.WriteParameter(SI1145_PARAM_ALSIRADCGAIN, 0);

				// take 511 clocks to measure
				await this.WriteParameter(SI1145_PARAM_ALSIRADCOUNTER, SI1145_PARAM_ADCCOUNTER_511CLK);

				// in high range mode
				await this.WriteParameter(SI1145_PARAM_ALSIRADCMISC, SI1145_PARAM_ALSIRADCMISC_RANGE);

				// fastest clocks, clock div 1
				await this.WriteParameter(SI1145_PARAM_ALSVISADCGAIN, 0);

				// take 511 clocks to measure
				await this.WriteParameter(SI1145_PARAM_ALSVISADCOUNTER, SI1145_PARAM_ADCCOUNTER_511CLK);

				// in high range mode (not normal signal)
				await this.WriteParameter(SI1145_PARAM_ALSVISADCMISC, SI1145_PARAM_ALSVISADCMISC_VISRANGE);

				// measurement rate for auto
				await this.WriteParameter(SI1145_REG_MEASRATE0, 0xFF); // 255 * 31.25uS = 8ms

				// auto run
				await this.WriteParameter(SI1145_REG_COMMAND, SI1145_PSALS_AUTO);
			}
			else
			{
				throw new DeviceNotFoundException("SI1145");
			}
		}

		protected async override Task OnResetAsync()
		{
			await this.WriteAsync(new byte[] { SI1145_REG_MEASRATE0, 0 });
			await this.WriteAsync(new byte[] { SI1145_REG_MEASRATE1, 0 });
			await this.WriteAsync(new byte[] { SI1145_REG_IRQEN, 0 });
			await this.WriteAsync(new byte[] { SI1145_REG_IRQMODE1, 0 });
			await this.WriteAsync(new byte[] { SI1145_REG_IRQMODE2, 0 });
			await this.WriteAsync(new byte[] { SI1145_REG_INTCFG, 0 });
			await this.WriteAsync(new byte[] { SI1145_REG_IRQSTAT, 0xFF });

			await this.WriteAsync(new byte[] { SI1145_REG_COMMAND, SI1145_RESET });
			await Task.Delay(10);
			await this.WriteAsync(new byte[] { SI1145_REG_HWKEY, 0x17 });
			await Task.Delay(10);
		}

		public async Task<int> GetVisibleAsync()
		{
			int returnValue = 0;

			byte[] readBuffer = new byte[2];
			await this.WriteReadAsync(new byte[] { SI1145_REG_ALSVISDATA0 }, readBuffer);
			returnValue = (int)BitConverter.ToInt16(readBuffer, 0);

			return returnValue;
		}

		public async Task<int> GetUvAsync()
		{
			int returnValue = 0;

			byte[] readBuffer = new byte[2];
			await this.WriteReadAsync(new byte[] { SI1145_REG_UVINDEX0 }, readBuffer);
			returnValue = (int)BitConverter.ToInt16(readBuffer, 0);

			return returnValue;
		}

		public async Task<int> GetIrAsync()
		{
			int returnValue = 0;

			byte[] readBuffer = new byte[2];
			await this.WriteReadAsync(new byte[] { SI1145_REG_ALSIRDATA0 }, readBuffer);
			returnValue = (int)BitConverter.ToInt16(readBuffer, 0);

			return returnValue;
		}

		public async Task<int> GetProximityAsync()
		{
			int returnValue = 0;

			byte[] readBuffer = new byte[2];
			await this.WriteReadAsync(new byte[] { SI1145_REG_PS1DATA0 }, readBuffer);
			returnValue = (int)BitConverter.ToInt16(readBuffer, 0);

			return returnValue;
		}

		public async Task WriteParameter(byte parameter, byte value)
		{
			await this.WriteAsync(new byte[] { SI1145_REG_PARAMWR, value });
			await this.WriteAsync(new byte[] { SI1145_REG_COMMAND, (byte)(parameter | SI1145_PARAM_SET) });
		}

		public async Task<byte> ReadParam(byte parameter)
		{
			byte[] readBuffer = new byte[1];
			await this.WriteReadAsync(new byte[] { SI1145_REG_COMMAND, (byte)(parameter | SI1145_PARAM_QUERY) }, readBuffer);

			return readBuffer[0];
		}
	}
}
