#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ws.winx.devices;
using ws.winx.unity;

namespace ws.winx.platform.android
{
    public class AndroidHIDInterface : IHIDInterface
    {
        #region Fields

        public const string TAG = "AndroidHIDInterface";

        private List<IDriver> __drivers;// = new List<IJoystickDriver>();

        private IDriver __defaultJoystickDriver = new UnityDriver();
        JoystickDevicesCollection __joysticks;
        GameObject _container;

        //link towards Browser
        internal readonly AndroidHIDBehaviour droidHIDBehaviour;
        private Dictionary<IDevice, HIDDevice> __Generics;


        #endregion


        public AndroidHIDInterface(List<IDriver> drivers)
        {
            UnityEngine.Debug.Log("AndroidHIDInterface");
            __drivers = drivers;
            __joysticks = new JoystickDevicesCollection();
            __Generics = new Dictionary<IDevice, HIDDevice>();

            _container = new GameObject("AndroidHIDBehaviourGO");
            droidHIDBehaviour = _container.AddComponent<AndroidHIDBehaviour>();
          
            droidHIDBehaviour.DeviceDisconnectedEvent += new EventHandler<AndroidMessageArgs>(DeviceDisconnectedEventHandler);
            droidHIDBehaviour.DeviceConnectedEvent += new EventHandler<AndroidMessageArgs>(DeviceConnectedEventHandler);

            
           // droidHIDBehaviour.Enumerate();
        }



        public void DeviceConnectedEventHandler(object sender, AndroidMessageArgs args)
        {
            AndroidJavaObject device = (AndroidJavaObject)args.data;


            if (__joysticks.FindBy(device.Get<int>("PID")) == null)
            {
                // UnityEngine.Debug.Log(args.Message);
                GenericHIDDevice info = new GenericHIDDevice(__joysticks.Count, device, this);

                info.hidInterface = this;

                ResolveDevice(info);
            }
        }

        public void DeviceDisconnectedEventHandler(object sender, AndroidMessageArgs args)
        {

            int pid = (int)args.data;
            IDevice device = __joysticks.FindBy(pid);
            if (device != null)
            {
                this.droidHIDBehaviour.Log(TAG, "Device " + device.Name + " index:" + device.ID + " Removed");
                this.__Generics.Remove(device);
                __joysticks.Remove(device.ID);
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
                        //new IntPtr just for compatibility
                        __joysticks[new IntPtr(joyDevice.PID)] = joyDevice;

                        this.droidHIDBehaviour.Log("AndroidHIDInterface", "Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " attached to " + driver.GetType().ToString());
                        this.__Generics[joyDevice] = deviceInfo;
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

                    this.droidHIDBehaviour.Log("AndroidHIDInterface", "Device index:" + joyDevice.ID + " PID:" + joyDevice.PID + " VID:" + joyDevice.VID + " attached to " + __defaultJoystickDriver.GetType().ToString() + " Path:" + deviceInfo.DevicePath + " Name:" + joyDevice.Name);
                    this.__Generics[joyDevice] = deviceInfo;
                }
                else
                {
                    Debug.LogWarning("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " not found compatible driver thru WinHIDInterface!");

                }

            }


        }

        public IDeviceCollection Devices
        {
            get { return __joysticks; }
        }

        public Dictionary<devices.IDevice, HIDDevice> Generics
        {
            get { return __Generics; }
        }

        public void Read(devices.IDevice device, HIDDevice.ReadCallback callback)
        {
            this.__Generics[device].Read(callback);
        }

        public void Write(object data, devices.IDevice device, HIDDevice.WriteCallback callback)
        {
            throw new NotImplementedException();
        }

        public void Write(object data, devices.IDevice device)
        {
            this.__Generics[device].Write(data);
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
            readonly Dictionary<IntPtr, IDevice> JoystickDevices;
            // readonly Dictionary<IntPtr, IDevice<IAxisDetails, IButtonDetails, IDeviceExtension>> JoystickDevices;

            readonly Dictionary<int, IntPtr> JoystickIDToDevice;


            List<IDevice> _iterationCacheList;
            bool _isEnumeratorDirty = true;

            #endregion

#region Constructors

            internal JoystickDevicesCollection()
            {

                JoystickIDToDevice = new Dictionary<int, IntPtr>();

                JoystickDevices = new Dictionary<IntPtr, IDevice>();
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




            public IDevice this[int index]
            {
                get { return JoystickDevices[JoystickIDToDevice[index]]; }

            }



            public IDevice this[IntPtr pidPointer]
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

                    _iterationCacheList = JoystickDevices.Values.ToList<IDevice>();



                    _isEnumeratorDirty = false;


                }

                return _iterationCacheList.GetEnumerator();

            }

            public IDevice FindBy(int pid)
            {
                return JoystickDevices.Where(z => z.Value.PID == pid).FirstOrDefault().Value;
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

     

        }
        #endregion
    

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
#endif