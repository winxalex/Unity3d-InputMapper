using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.platform;

namespace ws.winx.devices
{
	class XInputDevice:JoystickDevice
	{
        public readonly int Type;


        public XInputDevice(int id,int pid,int vid,int axes,int buttons,IJoystickDriver driver,int type)
            : base(id,pid,vid, axes, buttons,driver)
        {
            this.Type = type;
        }

        public void SetMotor(float leftMotor, float rightMotor)
        {
            ((XInputDriver)this.driver).SetMotor(this,leftMotor,rightMotor);
        }

       
	}
}
