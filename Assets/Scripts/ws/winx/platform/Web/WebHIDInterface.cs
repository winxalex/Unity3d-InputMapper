#if UNITY_WEBPLAYER	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ws.winx.devices;
using System.Timers;
using System.Runtime.InteropServices;



namespace ws.winx.platform.web
{
    public class WebHIDInterface : IHIDInterface
    {
        #region Fields
        private List<IDriver> __drivers;// = new List<IJoystickDriver>();
       
        private IDriver __defaultJoystickDriver;
      
        GameObject _container;

        //link towards Browser
        internal readonly WebHIDBehaviour webHIDBehaviour;
        private Dictionary<int, HIDDevice> __Generics;

		public event EventHandler<DeviceEventArgs<int>> DeviceDisconnectEvent;
		public event EventHandler<DeviceEventArgs<IDevice>> DeviceConnectEvent;

       
        #endregion

#region Constructors
        public WebHIDInterface(List<IDriver> drivers)
        {
            __drivers = drivers;
           
            __Generics=new Dictionary<int,HIDDevice>();

            //"{"id":"feed-face-VJoy Virtual Joystick","axes":[0.000015259021893143654,0.000015259021893143654,0.000015259021893143654,0,0,0,0,0,0,-1,0,0,0,0,0,0],"buttons":[{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{}],"index":0}"

            _container = new GameObject("WebHIDBehaviourGO");
            webHIDBehaviour= _container.AddComponent<WebHIDBehaviour>();
          

        }
        #endregion

        #region IHIDInterface implementation

        public void Enumerate ()
		{
			webHIDBehaviour.DeviceDisconnectedEvent += new EventHandler<WebMessageArgs<int>>(DeviceDisconnectedEventHandler);
			webHIDBehaviour.DeviceConnectedEvent += new EventHandler<WebMessageArgs<GenericHIDDevice>>(DeviceConnectedEventHandler);
			webHIDBehaviour.GamePadEventsSupportEvent += new EventHandler<WebMessageArgs<bool>>(GamePadEventsSupportHandler);
		}

	

        public HIDReport ReadDefault(int pid)
        {
            throw new NotImplementedException ();
        }


        public HIDReport ReadBuffered(int pid)
        {
            return this.__Generics[pid].ReadBuffered();
        }

		public void Read (int pid, HIDDevice.ReadCallback callback, int timeout)
		{
			throw new NotImplementedException ();
		}
		public void Write (object data, int device, HIDDevice.WriteCallback callback, int timeout)
		{
			throw new NotImplementedException ();
		}
		


        public void Read(int pid, HIDDevice.ReadCallback callback)
        {
            throw new NotImplementedException ();
        }

        public void Write(object data, int pid, HIDDevice.WriteCallback callback)
        {
            throw new NotImplementedException();
        }

        public void Write(object data, int pid)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, HIDDevice> Generics
        {
            get { return __Generics; }

        }


        public IDriver defaultDriver
        {
            get
            {
                 if (__defaultJoystickDriver == null) { __defaultJoystickDriver = new WebDriver(); }
                return __defaultJoystickDriver;
            }
            set
            {
                __defaultJoystickDriver = value;
            }
        }

      



        /// <summary>
        /// Try to attach compatible driver based on device info
        /// </summary>
        /// <param name="deviceInfo"></param>
        protected void ResolveDevice(HIDDevice deviceInfo)
        {

            IDevice joyDevice = null;

            //loop thru drivers and attach the driver to device if compatible
            if (__drivers != null)
                foreach (var driver in __drivers)
                {
                    joyDevice = driver.ResolveDevice(deviceInfo);
                    if (joyDevice != null)
                    {
                      
                        this.webHIDBehaviour.Log("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " attached to " + driver.GetType().ToString());
                        this.__Generics[deviceInfo.PID]=deviceInfo;
                        break;
                    }
                }

            if (joyDevice == null)
            {//set default driver as resolver if no custom driver match device
                joyDevice = defaultDriver.ResolveDevice(deviceInfo);


                if (joyDevice != null)
                {

                   // Debug.Log(__joysticks[deviceInfo.index]);

                    this.webHIDBehaviour.Log("Device index:" + joyDevice.Index+ " PID:" + joyDevice.PID + " VID:" + joyDevice.VID + " attached to " + __defaultJoystickDriver.GetType().ToString() + " Path:" + deviceInfo.DevicePath + " Name:" + joyDevice.Name);
                    
					webHIDBehaviour.PositionUpdateEvent += new EventHandler<WebMessageArgs<WebHIDReport>>(((GenericHIDDevice)deviceInfo).onPositionUpdate);

					this.__Generics[deviceInfo.PID]=deviceInfo;
                }
                else
                {
                    Debug.LogWarning("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " not found compatible driver thru WinHIDInterface!");

                }

            }


        }

        #endregion



        public void GamePadEventsSupportHandler(object sender, WebMessageArgs<bool> args)
        {
           
        }



		/// <summary>
		/// Devices the connected event handler.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
        public void DeviceConnectedEventHandler(object sender,WebMessageArgs<GenericHIDDevice> args)
        {
           // UnityEngine.Debug.Log(args.Message);
			GenericHIDDevice info = args.RawMessage;



           
          
             if(!__Generics.ContainsKey(info.PID))
             {
                 info.hidInterface = this;
                 
                 ResolveDevice(info);
             }
        }



		/// <summary>
		/// Devices the disconnected event handler.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
        public void DeviceDisconnectedEventHandler(object sender, WebMessageArgs<int> args)
        {
            
		

		

			if (__Generics.ContainsKey (args.RawMessage)) {
				int PID=args.RawMessage;
				string Name=__Generics[PID].Name;
								this.webHIDBehaviour.Log ("Device " + Name + " PID:" + PID + " Removed");
				this.__Generics.Remove (PID);
	
						}
           
           
             
        }



      
        

        public void Update()
        {
            throw new NotImplementedException();
        }




           
        

        public void Dispose()
        {
            this.__Generics.Clear();
           
        }





       
    }
}
#endif