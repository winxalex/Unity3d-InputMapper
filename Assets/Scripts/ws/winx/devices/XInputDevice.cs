using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ws.winx.devices
{
	class XInputDevice:JoystickDevice
	{
        public readonly int Type;


        public XInputDevice(int id,int axes,int buttons,int type)
            : base(id, axes, buttons)
        {
            this.Type = type;
        }

        public void SetMotor(float leftMotor, float rightMotor)
        {
            ((XInputDriver)this.driver).SetMotor(this,leftMotor,rightMotor);
        }

       
	}
}
