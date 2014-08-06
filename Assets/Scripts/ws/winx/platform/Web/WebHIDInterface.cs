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
        private List<IJoystickDriver> __drivers;// = new List<IJoystickDriver>();
       
        private IJoystickDriver __defaultJoystickDriver;
        JoystickDevicesCollection __joysticks;
        GameObject _container;

        //link towards Browser
        internal readonly WebHIDBehaviour webHIDBehaviour;
        private Dictionary<IJoystickDevice, HIDDevice> __Generics;

       
        #endregion

#region Constructors
        public WebHIDInterface(List<IJoystickDriver> drivers)
        {
            __drivers = drivers;
            __joysticks = new JoystickDevicesCollection();
            __Generics=new Dictionary<IJoystickDevice,HIDDevice>();

            //"{"id":"feed-face-VJoy Virtual Joystick","axes":[0.000015259021893143654,0.000015259021893143654,0.000015259021893143654,0,0,0,0,0,0,-1,0,0,0,0,0,0],"buttons":[{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{}],"index":0}"

            _container = new GameObject("WebHIDBehaviourGO");
            webHIDBehaviour= _container.AddComponent<WebHIDBehaviour>();
            webHIDBehaviour.DeviceDisconnectedEvent += new EventHandler<WebMessageArgs>(DeviceDisconnectedEventHandler);
            webHIDBehaviour.DeviceConnectedEvent += new EventHandler<WebMessageArgs>(DeviceConnectedEventHandler);
            webHIDBehaviour.GamePadEventsSupportEvent += new EventHandler<WebMessageArgs>(GamePadEventsSupportHandler);

        }
        #endregion

        #region IHIDInterface implementation

        public Dictionary<IJoystickDevice, HIDDevice> Generics
        {
            get { return __Generics; }

        }


        public IJoystickDriver defaultDriver
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

        public IDeviceCollection Devices
        {
            get { return __joysticks; }
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
                        Debug.Log("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " attached to " + driver.GetType().ToString());
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

                    Debug.Log(__joysticks[deviceInfo.index]);

                    Debug.Log("Device index:" + joyDevice.ID + " PID:" + joyDevice.PID + " VID:" + joyDevice.VID + " attached to " + __defaultJoystickDriver.GetType().ToString() + " Path:" + deviceInfo.DevicePath + " Name:" + joyDevice.Name);
                     this.__Generics[joyDevice]=deviceInfo;
                }
                else
                {
                    Debug.LogWarning("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " not found compatible driver thru WinHIDInterface!");

                }

            }


        }

        #endregion



        public void GamePadEventsSupportHandler(object sender, WebMessageArgs args)
        {
           
        }

        public void DeviceConnectedEventHandler(object sender,WebMessageArgs args)
        {
           // UnityEngine.Debug.Log(args.Message);
            GenericHIDDevice info=Json.Deserialize<GenericHIDDevice>(args.RawMessage) as GenericHIDDevice;
           
          
             if(!__joysticks.ContainsKey(info.index))
             {
                 info.hidInterface = this;
                 
                 ResolveDevice(info);
             }
        }

        public void DeviceDisconnectedEventHandler(object sender, WebMessageArgs args)
        {
          
            int id = Int32.Parse(args.RawMessage);
            Debug.Log("Device "+__joysticks[id].Name+" index:" +id+" Removed");
            this.__Generics.Remove(__joysticks[id]);
            __joysticks.Remove(id);
           
           
             
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



           
        }

        public void Dispose()
        {
            this.__Generics.Clear();
            __joysticks.Clear();
        }


       
    }
}
#endif