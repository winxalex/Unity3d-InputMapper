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
        JoystickDevicesCollection _joysticks;
        GameObject _container;
        WebHIDBehaviour w;

       
        #endregion

        #region Constructors
        public WebHIDInterface(List<IJoystickDriver> drivers)
        {
            __drivers = drivers;
            _joysticks = new JoystickDevicesCollection();

         //Json.GamePadInfo info=   Json.Deserialize("{\"buttons\":[1,0,0,0,0,0,0,0,0,0],\"axes\":[1.031280517578125,1,0,0,0,1,1,1,0,3.2857143878936768],\"timestamp\":17,\"index\":1,\"id\":\"Thrustmaster force feedback wheel (Vendor: 044f Product: b653)\"}") as Json.GamePadInfo;

      //Dictionary<string,object> obj=      Json.Deserialize("{\"0\":{\"buttons\":[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],\"axes\":[0.000015259021893143654,0.000015259021893143654,0.000015259021893143654,0.000015259021893143654,0.000015259021893143654,0.000015259021893143654,0.000015259021893143654,0.000015259021893143654,0,1.6666666269302368],\"timestamp\":1964503,\"index\":0,\"id\":\" (Vendor: feed Product: face)\"},\"1\":{\"buttons\":[1,0,0,0,0,0,0,0,0,0],\"axes\":[1.031280517578125,1,0,0,0,1,1,1,0,3.2857143878936768],\"timestamp\":17,\"index\":1,\"id\":\"Thrustmaster force feedback wheel (Vendor: 044f Product: b653)\"},\"length\":4}") as Dictionary<string,object>;

      //Dictionary<string, object> joy = obj["0"] as Dictionary<string, object>;
      //      List<object> buttons=joy["buttons"] as List<object>;
      //      string id= joy["id"] as string;

            //string value="fasdfasfasfsafaff (Vendor: 044f Product: b653)";
            //        int inx = value.IndexOf("(");
            //        if (inx > -1)
            //        {
            //           String Name = value.Substring(0, inx - 1);
            //            String[] parts = value.Substring(inx, value.Length - inx-1).Replace("Product", "").Split(':');

            //           int PID = Convert.ToInt32(parts[2].Trim(), 16);
            //          int  VID = Convert.ToInt32(parts[1].Trim(), 16);
            //        }

            try
            {
                throw new Exception("dump stack");
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
            }

            _container = new GameObject("WebHIDBehaviourGO");
            w= _container.AddComponent<WebHIDBehaviour>();
            w.DeviceDisconnectedEvent += new EventHandler<WebMessageArgs>(DeviceDisconnectedEventHandler);
            w.DeviceConnectedEvent += new EventHandler<WebMessageArgs>(DeviceConnectedEventHandler);
            w.GamePadEventsSupportEvent += new EventHandler<WebMessageArgs>(GamePadEventsSupportHandler);

        }
        #endregion


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
            get { return _joysticks; }
        }



        /// <summary>
        /// Try to attach compatible driver based on device info
        /// </summary>
        /// <param name="deviceInfo"></param>
        protected void ResolveDevice(HIDDeviceInfo deviceInfo)
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
                        _joysticks[new IntPtr(joyDevice.PID)] = joyDevice;
                        Debug.Log("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " attached to " + driver.GetType().ToString());

                        break;
                    }
                }

            if (joyDevice == null)
            {//set default driver as resolver if no custom driver match device
                joyDevice = defaultDriver.ResolveDevice(deviceInfo);


                if (joyDevice != null)
                {

                    //new IntPtr just for compatibility
                    _joysticks[new IntPtr(joyDevice.PID)] = joyDevice;

                    Debug.Log(_joysticks[deviceInfo.index]);

                    Debug.Log("Device index:" + joyDevice.ID + " PID:" + joyDevice.PID + " VID:" + joyDevice.VID + " attached to " + __defaultJoystickDriver.GetType().ToString() + " Path:" + deviceInfo.DevicePath + " Name:" + joyDevice.Name);

                }
                else
                {
                    Debug.LogWarning("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " not found compatible driver thru WinHIDInterface!");

                }

            }


        }


      


        public void GamePadEventsSupportHandler(object sender, WebMessageArgs args)
        {
           
        }

        public void DeviceConnectedEventHandler(object sender,WebMessageArgs args)
        {
            Json.GamePadInfo info=Json.Deserialize(args.Message) as Json.GamePadInfo;
             if(!_joysticks.ContainsKey(info.index))
             {
                 info.hidInterface = this;
                 ResolveDevice(info);
             }
        }

        public void DeviceDisconnectedEventHandler(object sender, WebMessageArgs args)
        {
          
            int id = Int32.Parse(args.Message);
            Debug.Log("Device "+_joysticks[id].Name+" index:" +id+" Removed");
            _joysticks.Remove(id);

             
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
            throw new NotImplementedException();
        }
    }
}
#endif