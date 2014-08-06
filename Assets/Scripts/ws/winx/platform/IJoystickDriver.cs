
using System;
using System.Collections.Generic;
using System.Text;
using ws.winx.devices;

namespace ws.winx.platform
{
    /// <summary>
    /// Defines the interface for JoystickDevice drivers.
    /// </summary>
    //[Obsolete]
    public interface IDriver
    {

		/// <summary>
		/// Resolves the device.
		/// </summary>
		/// <returns>returns JoystickDevice if driver is for this device or null</returns>
		/// <param name="info">Info.</param>
         IJoystickDevice ResolveDevice (IHIDDevice info);
		//IJoystickDevice<IAxisDetails,IButtonDetails,IDeviceExtension> ResolveDevice (IHIDDeviceInfo info);
		//JoystickDevice<IAxisDetails,IButtonDetails,IDeviceExtension> ResolveDevice (IHIDDeviceInfo info);
         void Update(IJoystickDevice joystick);
		//void Update(IJoystickDevice<IAxisDetails,IButtonDetails,IDeviceExtension> joystick);

    }



}
