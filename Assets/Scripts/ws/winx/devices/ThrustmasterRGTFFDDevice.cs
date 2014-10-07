using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.platform;
using ws.winx.platform.windows;
using ws.winx.drivers;


namespace ws.winx.devices
{
	public class ThrustmasterRGTFFDDevice:JoystickDevice
	{
        public ThrustmasterRGTFFDDevice(int id, int pid, int vid, int axes, int buttons, IDriver driver)
            : base(id,pid,vid,axes,buttons,driver)
        {
        }


        /// <summary>
        /// Move FFD motor of the wheel left or right
        /// </summary>
        /// <param name="forces">0xFF - 0xA7(left) and 0x00-0x64(rights) are measurable by feeling </param>
        public void SetMotor(byte forceX,byte forceY,HIDDevice.WriteCallback callback)
        {
            ((ThrustMasterDriver)this.driver).SetMotor(this, forceX,forceY, callback);
        }

        public void StopMotor()
        {
            ((ThrustMasterDriver)this.driver).StopMotor(this);
        }

        public void StopMotor(HIDDevice.WriteCallback callback)
        {
            ((ThrustMasterDriver)this.driver).StopMotor(this,callback);
        }
    }
}
