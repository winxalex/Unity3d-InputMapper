using ws.winx.platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace ws.winx.devices
{
    /// <summary>
    /// Defines a common interface for all joystick devices.
    /// </summary>
//    public interface IDevice<t,k,p> 
//		where t:IAxisDetails
//		where k:IButtonDetails
//		where p:IDeviceExtension
//    {
	public interface IDevice
        
	{
		int VID{get;}
		int PID{get;}
		int Index{get;}
        int numPOV { get; set; }
        bool isReady { get; }
		IDriver driver{get;} 
		string Name{get;set;}
     

			
		      
		IDeviceExtension Extension{get;set;}
		JoystickAxisCollection<IAxisDetails>  Axis {get;}
		JoystickButtonCollection<IButtonDetails> Buttons {get;}

		void Update();
		int GetInputCode();
		bool GetInputHold(int code);
		bool GetInputDown(int code);
		bool GetInputUp(int code);
		float GetInput (int code);
		bool GetAnyInputDown();
    }

  
}
