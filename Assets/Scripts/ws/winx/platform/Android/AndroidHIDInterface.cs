using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ws.winx.devices;
using ws.winx.unity;

namespace ws.winx.platform.android
{
	public class AndroidHIDInterface:IHIDInterface
	{
        #region Fields

        public const string TAG="AndroidHIDInterface";

        private List<IDriver> __drivers;// = new List<IJoystickDriver>();

        private IDriver __defaultJoystickDriver=new UnityDriver();
        JoystickDevicesCollection __joysticks;
        GameObject _container;

        //link towards Browser
        internal readonly AndroidHIDBehaviour webHIDBehaviour;
        private Dictionary<IJoystickDevice, HIDDevice> __Generics;


        #endregion


        public AndroidHIDInterface(List<IDriver> drivers){

            __drivers = drivers;
            __joysticks = new JoystickDevicesCollection();
            __Generics = new Dictionary<IJoystickDevice, HIDDevice>();

           _container = new GameObject("AndroidHIDBehaviourGO");
            webHIDBehaviour = _container.AddComponent<AndroidHIDBehaviour>();
            webHIDBehaviour.DeviceDisconnectedEvent += new EventHandler<AndroidMessageArgs>(DeviceDisconnectedEventHandler);
            webHIDBehaviour.DeviceConnectedEvent += new EventHandler<AndroidMessageArgs>(DeviceConnectedEventHandler);
 
        }



         public void DeviceConnectedEventHandler(object sender,AndroidMessageArgs args)
        {
           // UnityEngine.Debug.Log(args.Message);
            GenericHIDDevice info=args.data;
           
          
             if(!__joysticks.ContainsKey(info.index))
             {
                 info.hidInterface = this;
                 
                 ResolveDevice(info);
             }
        }

        public void DeviceDisconnectedEventHandler(object sender, AndroidMessageArgs args)
        {
          
            int id = Int32.Parse(args.RawMessage);
            this.webHIDBehaviour.Log(TAG,"Device " + __joysticks[id].Name + " index:" + id + " Removed");
            this.__Generics.Remove(__joysticks[id]);
            __joysticks.Remove(id);
           
           
             
        }


        public IDriver defaultDriver
        {
            get
            {
                return __defaultJoystickDriver;
            }
            
        }


        
        /// <summary>
        /// Try to attach compatible driver based on device info
        /// </summary>
        /// <param name="deviceInfo"></param>
        protected void ResolveDevice(HIDDevice deviceInfo)
        {

            IJoystickDevice joyDevice = null;

            //loop thru drivers and attach the driver to device if compatible
            if (__drivers != null)
                foreach (var driver in __drivers)
                {
                    joyDevice = driver.ResolveDevice(deviceInfo);
                    if (joyDevice != null)
                    {
                        //new IntPtr just for compatibility
                        __joysticks[new IntPtr(joyDevice.PID)] = joyDevice;

                        this.webHIDBehaviour.Log("WebHIDInterface","Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " attached to " + driver.GetType().ToString());
                        this.__Generics[joyDevice]=deviceInfo;
                        break;
                    }
                }

            if (joyDevice == null)
            {//set default driver as resolver if no custom driver match device
                joyDevice = defaultDriver.ResolveDevice(deviceInfo);


                if (joyDevice != null)
                {

                    //new IntPtr just for compatibility
                    __joysticks[new IntPtr(joyDevice.PID)] = joyDevice;

                   // Debug.Log(__joysticks[deviceInfo.index]);

                    this.webHIDBehaviour.Log("WebHIDInterface","Device index:" + joyDevice.ID + " PID:" + joyDevice.PID + " VID:" + joyDevice.VID + " attached to " + __defaultJoystickDriver.GetType().ToString() + " Path:" + deviceInfo.DevicePath + " Name:" + joyDevice.Name);
                     this.__Generics[joyDevice]=deviceInfo;
                }
                else
                {
                    Debug.LogWarning("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " not found compatible driver thru WinHIDInterface!");

                }

            }


        }

        public IDeviceCollection Devices
        {
            get {return __joysticks; }
        }

        public Dictionary<devices.IJoystickDevice, HIDDevice> Generics
        {
            get { return __Generics;}
        }

        public void Read(devices.IJoystickDevice device, HIDDevice.ReadCallback callback)
        {
            throw new NotImplementedException();
        }

        public void Write(object data, devices.IJoystickDevice device, HIDDevice.WriteCallback callback)
        {
            throw new NotImplementedException();
        }

        public void Write(object data, devices.IJoystickDevice device)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        
#region JoystickDevicesCollection

        /// <summary>
        /// Defines a collection of JoystickAxes.
        /// </summary>
        public sealed class JoystickDevicesCollection : IDeviceCollection
        {
#region Fields
                readonly Dictionary<IntPtr, IJoystickDevice> JoystickDevices;
                   // readonly Dictionary<IntPtr, IJoystickDevice<IAxisDetails, IButtonDetails, IDeviceExtension>> JoystickDevices;

                readonly Dictionary<int, IntPtr> JoystickIDToDevice;


            List<IJoystickDevice> _iterationCacheList;
            bool _isEnumeratorDirty = true;

#endregion

#region Constructors

            internal JoystickDevicesCollection()
            {
                
                JoystickIDToDevice = new Dictionary<int, IntPtr>();

                JoystickDevices = new Dictionary<IntPtr, IJoystickDevice>();
            }

#endregion

#region Public Members

#endregion

#region IDeviceCollection implementation

            public void Remove(IntPtr device)
            {
                JoystickIDToDevice.Remove(JoystickDevices[device].ID);
                JoystickDevices.Remove(device);

                _isEnumeratorDirty = true;
            }


            public void Remove(int inx)
            {
                IntPtr device = JoystickIDToDevice[inx];
                JoystickIDToDevice.Remove(inx);
                JoystickDevices.Remove(device);

                _isEnumeratorDirty = true;
            }




            public IJoystickDevice this[int index]
            {
                get { return JoystickDevices[JoystickIDToDevice[index]]; }
                				
            }



            public IJoystickDevice this[IntPtr pidPointer]
            {
                get { throw new Exception("Devices should be retrived only thru index"); }

                set
                {
                    JoystickDevices[pidPointer] = value;
                    JoystickIDToDevice[value.ID] = pidPointer;

                    _isEnumeratorDirty = true;
                }
               
            }


            public bool ContainsKey(int key)
            {
                return JoystickIDToDevice.ContainsKey(key);
            }

            public bool ContainsKey(IntPtr key)
            {
                return JoystickDevices.ContainsKey(key);
            }

            public System.Collections.IEnumerator GetEnumerator()
            {
                if (_isEnumeratorDirty)
                {
					
                    _iterationCacheList = JoystickDevices.Values.ToList<IJoystickDevice>();

					

                    _isEnumeratorDirty = false;


                }

                return _iterationCacheList.GetEnumerator();

            }


            public void Clear()
            {
                JoystickIDToDevice.Clear();
                JoystickDevices.Clear();
            }


            /// <summary>
            /// Gets a System.Int32 indicating the available amount of JoystickDevices.
            /// </summary>
            public int Count
            {
                get { return JoystickDevices.Count; }
            }

#endregion

#endregion




        public void Dispose()
        {
            throw new NotImplementedException();
        }



    
    }
}
