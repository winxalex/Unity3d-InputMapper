using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.devices;
using ws.winx.platform;

namespace ws.winx
{
	class MyJoystickDevice:JoystickDevice
	{
		public MyJoystickDevice(int id,int pid,int vid,int axes,int buttons,IDriver driver):base(id,pid,vid,axes,buttons,driver){
		}

        public override int GetInput()
        {
            return base.GetInput();
        }

        public override bool GetKey(int code)
        {
			// if (MyCodeExtension.GAMEPAD_DPAD_UP.SINGLE == code)
//            {
//
//            }
            return base.GetKey(code);
        }
	}
}
