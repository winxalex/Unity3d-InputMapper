using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ws.winx.devices
{
	class XInputDevice:JoystickDevice
	{
        public XInputDevice(int id,int axes,int buttons)
            : base(id, axes, buttons)
        {
        }

        public void SetMotor(float leftMotor, float rightMotor)
        {
            ((XInputDriver)this.driver).SetMotor(this,leftMotor,rightMotor);
        }

       
	}
}
