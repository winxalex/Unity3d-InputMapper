//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using ws.winx.devices;
using System.Collections;
namespace ws.winx.platform
{
		public interface IDeviceCollection:IEnumerable
		{
			IJoystickDevice this [int index] 
			//IJoystickDevice<IAxisDetails,IButtonDetails,IDeviceExtension> this [int index] 
			{
				get; 
		    }

			IJoystickDevice this [IntPtr device]{get;}
			//IJoystickDevice<IAxisDetails,IButtonDetails,IDeviceExtension> this [IntPtr device]{get;}

			void Remove (IntPtr device);
            void Remove(int inx);

			bool ContainsKey(int key);
			bool ContainsKey(IntPtr key);
			int Count{get;}
				
			
		}
}
