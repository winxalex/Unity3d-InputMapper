#if UNITY_WEBPLAYER	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ws.winx.devices;
using System.Timers;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;



namespace ws.winx.platform.web
{
    public class WebHIDInterface : IHIDInterface
    {
#region Fields
        private List<IDriver> __drivers;
       
        private IDriver __defaultJoystickDriver;
      
        GameObject _container;

        //link towards Browser
        internal readonly WebHIDBehaviour webHIDBehaviour;
        private Dictionary<string, HIDDevice> __Generics;
		private DeviceProfiles __profiles;

		public event EventHandler<DeviceEventArgs<string>> DeviceDisconnectEvent;
		public event EventHandler<DeviceEventArgs<IDevice>> DeviceConnectEvent;

       
        #endregion

#region Constructors
        public WebHIDInterface()
        {
            __drivers = new List<IDriver>();
           
            __Generics=new Dictionary<string,HIDDevice>();



            //"{"id":"feed-face-VJoy Virtual Joystick","axes":[0.000015259021893143654,0.000015259021893143654,0.000015259021893143654,0,0,0,0,0,0,-1,0,0,0,0,0,0],"buttons":[{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{}],"index":0}"

            _container = new GameObject("WebHIDBehaviourGO");
            webHIDBehaviour= _container.AddComponent<WebHIDBehaviour>();
          
           

        }
#endregion

#region IHIDInterface implementation



		public HIDReport ReadDefault (string id)
		{
			throw new NotImplementedException ();
		}

	

		public void Read (string id, HIDDevice.ReadCallback callback)
		{
			throw new NotImplementedException ();
		}

		public void Read (string id, HIDDevice.ReadCallback callback, int timeout)
		{
			throw new NotImplementedException ();
		}

		public void Write (object data, string id, HIDDevice.WriteCallback callback, int timeout)
		{
			throw new NotImplementedException ();
		}

		public void Write (object data, string id, HIDDevice.WriteCallback callback)
		{
			throw new NotImplementedException ();
		}

		public void Write (object data, string id)
		{
			throw new NotImplementedException ();
		}


		Dictionary<string, HIDDevice> IHIDInterface.Generics {
			get {
				throw new NotImplementedException ();
			}
		}



		public void SetProfiles(DeviceProfiles profiles){
			__profiles = profiles;


				}


		public void LoadProfiles(string fileName){
			
			__profiles=Resources.Load<DeviceProfiles> ("DeviceProfiles");
			
		}
		
		
		public DeviceProfile LoadProfile(string key){
			
			DeviceProfile profile=null;
			
			if (__profiles.vidpidProfileNameDict.ContainsKey (key)) {
				
				string profileName=__profiles.vidpidProfileNameDict[key];
				
				RuntimePlatform platform=Application.platform == RuntimePlatform.OSXWebPlayer ? RuntimePlatform.OSXPlayer: RuntimePlatform.WindowsPlayer;
				
				if(__profiles.runtimePlatformDeviceProfileDict[profileName].ContainsKey(platform)){
					
					profile=__profiles.runtimePlatformDeviceProfileDict[profileName][platform];
				}
				
			}
			
			
			return profile;
		}







		public void AddDriver (IDriver driver)
		{
			__drivers.Add (driver);
		}

		public bool Contains (string id)
		{
			return __Generics != null && __Generics.ContainsKey (id);
		}

        public void Enumerate ()
		{
		
			webHIDBehaviour.DeviceDisconnectedEvent += new EventHandler<WebMessageArgs<string>>(DeviceDisconnectedEventHandler);
			webHIDBehaviour.DeviceConnectedEvent += new EventHandler<WebMessageArgs<GenericHIDDevice>>(DeviceConnectedEventHandler);
			webHIDBehaviour.GamePadEventsSupportEvent += new EventHandler<WebMessageArgs<bool>>(GamePadEventsSupportHandler);
		}

	

     


        public HIDReport ReadBuffered(string id)
        {
            return this.__Generics[id].ReadBuffered();
        }



        public Dictionary<string, HIDDevice> Generics
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
                        this.__Generics[deviceInfo.ID]=deviceInfo;
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

					this.__Generics[deviceInfo.ID]=deviceInfo;
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



           
          
             if(!__Generics.ContainsKey(info.ID))
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
        public void DeviceDisconnectedEventHandler(object sender, WebMessageArgs<string> args)
        {
            
		

		

			if (__Generics.ContainsKey (args.RawMessage)) {
				string ID=args.RawMessage;
				string Name=__Generics[ID].Name;
				int PID=__Generics[ID].PID;
				this.webHIDBehaviour.Log ("Device " + Name + " PID:" + PID + " Removed");
				this.__Generics.Remove (ID);
	
						}
           
           
             
        }



      
        

        public void Update()
        {
            throw new NotImplementedException();
        }




           
        

        public void Dispose()
        {
			
			if (Generics != null) {
				foreach (KeyValuePair<string, HIDDevice> entry in Generics) {
					entry.Value.Dispose ();
				}
				
				
				Generics.Clear ();
			}

			Debug.Log ("Try to remove Drivers");
			if(__drivers!=null) __drivers.Clear();
           
        }





       
    }
}
#endif