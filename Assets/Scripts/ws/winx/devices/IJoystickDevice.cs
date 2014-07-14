using ws.winx.platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace ws.winx.devices
{
    /// <summary>
    /// Defines a common interface for all joystick devices.
    /// </summary>
//    public interface IJoystickDevice<t,k,p> 
//		where t:IAxisDetails
//		where k:IButtonDetails
//		where p:IDeviceExtension
//    {
	public interface IJoystickDevice
        
	{
		int VID{get;set;}
		int PID{get;set;}
		int ID{get;set;}
        int numPOV { get; set; }
		IJoystickDriver driver{get;set;} 
		string description{get;set;}

			
		      
		IDeviceExtension Extension{get;set;}
		JoystickAxisCollection<IAxisDetails>  Axis {get;}
		JoystickButtonCollection<IButtonDetails> Buttons {get;}

		void Update();
		int GetInput();
		bool GetKey(int code);
		bool GetKeyDown(int code);
		bool GetKeyUp(int code);
		float GetAxis (int code);
		bool GetAnyKeyDown();
    }

  
}
