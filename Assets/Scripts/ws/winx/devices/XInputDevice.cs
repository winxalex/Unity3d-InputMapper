using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.platform;
using ws.winx.drivers;

namespace ws.winx.devices
{

    
//0x00	All off
//0x01	All blinking
//0x02	1 flashes, then on
//0x03	2 flashes, then on
//0x04	3 flashes, then on
//0x05	4 flashes, then on
//0x06	1 on
//0x07	2 on
//0x08	3 on
//0x09	4 on
//0x0A	Rotating (e.g. 1-2-4-3)
//0x0B	Blinking*
//0x0C	Slow blinking*
//0x0D	Alternating (e.g. 1+4-2+3), then back to previous*


	public class XInputDevice:JoystickDevice
	{
        public readonly int Type;

        public enum LedMode
        {
            OFF=0x00,   //	All off
            BLINKING=0x01,	//All blinking
            LED1_ON=0x02	//1 flashes, then on
            //0x03	2 flashes, then on
            //0x04	3 flashes, then on
            //0x05	4 flashes, then on
            //0x06	1 on
            //0x07	2 on
            //0x08	3 on
            //0x09	4 on
            //0x0A	Rotating (e.g. 1-2-4-3)
            //0x0B	Blinking*
            //0x0C	Slow blinking*
            //0x0D	Alternating (e.g. 1+4-2+3), then back to previous*
        }


        public XInputDevice(int id,int pid,int vid,int axes,int buttons,IDriver driver,int type)
            : base(id,pid,vid, axes, buttons,driver)
        {
            this.Type = type;
        }

        public void SetLED(byte mode)
        {
          
            ((XInputDriver)this.driver).SetLed(this, mode);
          
        }

        public void SetMotor(byte leftMotor, byte rightMotor)
        {
		    ((XInputDriver)this.driver).SetMotor(this,leftMotor,rightMotor);
        }

       
	}
}
