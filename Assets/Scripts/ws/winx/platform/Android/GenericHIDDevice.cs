//#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.devices;
using UnityEngine;


namespace ws.winx.platform.android
{
    public class GenericHIDDevice :  HIDDevice
    {
        ReadWriteListenerProxy _listener;

        private int _numAxes;

        public int numAxes
        {
            get { return _numAxes; }
            set { _numAxes = value; }
        }
        private int _numButtons;

        public int numButtons
        {
            get { return _numButtons; }
            set { _numButtons = value; }
        }



      
        public override void GenericHIDDevice(AndroidJavaObject device)
        {
            this.PID=device.Get<int>("PID");
            this.VID=device.Get<int>("PID");
            _device=device;
            _listener = new ReadWriteListenerProxy();
        }

        public override void Read(HIDDevice.ReadCallback callback)
        {
             _device.Call("read",
        }

        //public void write(final byte[] from, IReadWriteListener listener, int timeout)
        public override void Write(object data)
        {
            _device.Call("write",(byte[]) data,_listener,0);
            
        }


       




        protected long _timestamp;
        private ws.winx.platform.HIDDevice.ReadCallback _readCallback;
        private IHIDInterface _hidInterface;
private  AndroidJavaObject _device;

        public long timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }




      

       
    }
}
//#endif