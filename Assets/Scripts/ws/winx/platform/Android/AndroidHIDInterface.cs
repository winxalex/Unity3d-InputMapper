#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ws.winx.devices;
using ws.winx.unity;
using ws.winx.drivers;

namespace ws.winx.platform.android
{
    public class AndroidHIDInterface : IHIDInterface,IDisposable
    {
        #region Fields

        public const string TAG = "AndroidHIDInterface";

        private List<IDriver> __drivers;

        private IDriver __defaultJoystickDriver = new UnityDriver();
      
        GameObject _container;

		bool hidCallbacksRegistered;

        //link towards Browser
        internal readonly AndroidHIDBehaviour droidHIDBehaviour;
        private Dictionary<int, HIDDevice> __Generics;

		public event EventHandler<DeviceEventArgs<int>> DeviceDisconnectEvent;
		public event EventHandler<DeviceEventArgs<IDevice>> DeviceConnectEvent;




        #endregion


        public AndroidHIDInterface()
        {
            UnityEngine.Debug.Log("AndroidHIDInterface");
            __drivers = new List<IDriver>();
           
            __Generics = new Dictionary<int, HIDDevice>();

            _container = new GameObject("AndroidHIDBehaviourGO");
            droidHIDBehaviour = _container.AddComponent<AndroidHIDBehaviour>();
          
              
           // 
        }

	
        public void DeviceConnectedEventHandler(object sender, AndroidMessageArgs<AndroidJavaObject> args)
        {
            AndroidJavaObject device = args.data;
			int pid = device.Get<int> ("PID");

			if (!__Generics.ContainsKey(pid))
            {
                // UnityEngine.Debug.Log(args.Message);
                GenericHIDDevice info = new GenericHIDDevice(__Generics.Count, device, this);

                info.hidInterface = this;

                ResolveDevice(info);
            }
        }

        public void DeviceDisconnectedEventHandler(object sender, AndroidMessageArgs<int> args)
        {

            int pid = args.data;
           
            if (__Generics.ContainsKey(pid))
            {
				HIDDevice device=__Generics[pid];
                this.droidHIDBehaviour.Log(TAG, "Device " + device.Name + " index:" + device.index+ " Removed");
                this.__Generics.Remove(pid);

				this.DeviceDisconnectEvent(this,new DeviceEventArgs<int>(pid));
               
            }



        }


        public IDriver defaultDriver
        {
            get
            {
                return __defaultJoystickDriver;
            }
             set
            {
                 __defaultJoystickDriver=value;
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
                       

                        Debug.Log("Device index:" + deviceInfo.index + " PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID 
							          + " attached to " + driver.GetType().ToString());
                      //  this.droidHIDBehaviour.Log("AndroidHIDInterface", "Device index:"+joyDevice.ID+" PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " attached to " + driver.GetType().ToString());
                        this.__Generics[deviceInfo.PID] = deviceInfo;

							this.DeviceConnectEvent(this,new DeviceEventArgs<IDevice>(joyDevice));

                        break;
                    }
                }

            if (joyDevice == null)
            {//set default driver as resolver if no custom driver match device
                joyDevice = defaultDriver.ResolveDevice(deviceInfo);


                if (joyDevice != null)
                {

                      // Debug.Log(__joysticks[deviceInfo.index]);
                    Debug.Log("Device index:" + deviceInfo.index + " PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " attached to " + __defaultJoystickDriver.GetType().ToString());
                     
                   // this.droidHIDBehaviour.Log("AndroidHIDInterface", "Device index:" + joyDevice.ID + " PID:" + joyDevice.PID + " VID:" + joyDevice.VID + " attached to " + __defaultJoystickDriver.GetType().ToString() + " Path:" + deviceInfo.DevicePath + " Name:" + joyDevice.Name);
                    this.__Generics[joyDevice.PID] = deviceInfo;

							this.DeviceConnectEvent(this,new DeviceEventArgs<IDevice>(joyDevice));
                }
                else
                {
                    Debug.LogWarning("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " not found compatible driver thru WinHIDInterface!");

                }

            }


        }

       

        public Dictionary<int, HIDDevice> Generics
        {
            get { return __Generics; }
        }

		#region IHIDInterface implementation

		public void AddDriver (IDriver driver)
		{
			__drivers.Add (driver);
		}

		public bool Contains (int pid)
		{
			return __Generics != null && __Generics.ContainsKey (pid);
		}



		public void Enumerate(){
			
			if(!hidCallbacksRegistered){
				droidHIDBehaviour.DeviceDisconnectedEvent += new EventHandler<AndroidMessageArgs<int>>(DeviceDisconnectedEventHandler);
				droidHIDBehaviour.DeviceConnectedEvent += new EventHandler<AndroidMessageArgs<AndroidJavaObject>>(DeviceConnectedEventHandler);
				hidCallbacksRegistered=true;
			}
			
			droidHIDBehaviour.Enumerate();
		}


		public void Write (object data, int device, HIDDevice.WriteCallback callback)
		{
			throw new NotImplementedException ();
		}


        public HIDReport ReadDefault(int pid){
			throw new NotImplementedException ();
		}

		public HIDReport ReadBuffered(int pid){
			throw new NotImplementedException ();
		}

        public void Read(int pid, HIDDevice.ReadCallback callback,int timeout=0xffff)
        {
            this.__Generics[pid].Read(callback,timeout);
        }

        public void Read(int pid, HIDDevice.ReadCallback callback)
        {
            this.__Generics[pid].Read(callback,0);
        }

        public void Write(object data, int pid, HIDDevice.WriteCallback callback,int timeout=0xffff)
        {
            this.__Generics[pid].Write(data,callback,timeout);
        }

        public void Write(object data, int pid)
        {
            this.__Generics[pid].Write(data);
        }



        public void Update()
        {
            throw new NotImplementedException();
        }




        #endregion     

        

    

        public void Dispose()
        {
			if(__Generics!=null){
	            foreach (KeyValuePair<int, HIDDevice> entry in __Generics)
	            {
	                entry.Value.Dispose();
	            }

	            __Generics.Clear();
			}


			Debug.Log ("Try to remove Drivers");
			if(__drivers!=null) __drivers.Clear();


        }
    }
}
#endif