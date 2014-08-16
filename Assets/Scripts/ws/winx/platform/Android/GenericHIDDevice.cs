#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.devices;
using UnityEngine;
using System.Timers;

namespace ws.winx.platform.android
{
    public class GenericHIDDevice : HIDDevice  //TODO thru  public ReadWriteListenerProxy(int index ) : base("ws.winx.hid.IReadWriteListener") {}
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


        int _InputReportByteLength=8;

        override public int InputReportByteLength
        {
            get { return _InputReportByteLength; }
            set {
                if (value < 2) throw new Exception("InputReportByteLength should be >1 ");  _InputReportByteLength = value; }
        }
        
        int _OutputReportByteLength=8;

        override public int OutputReportByteLength
        {
            get { return _OutputReportByteLength; }
            set { if (value < 2) throw new Exception("InputReportByteLength should be >1 ");  _OutputReportByteLength = value; }
        }




        public GenericHIDDevice(int inx, AndroidJavaObject device, IHIDInterface hidInterface)
            : base(inx, device.Get<int>("VID"), device.Get<int>("PID"), IntPtr.Zero, hidInterface, device.Get<string>("path"))
        {
            
            _device=device;
            _listener = new ReadWriteListenerProxy(inx);
        }


        //	public void read(byte[] into,IReadWriteListener listener, int timeout)
        public override void Read(HIDDevice.ReadCallback callback)
        {
            _listener.addReadCallback(callback);
            byte[] from=new byte[_InputReportByteLength];

             System.Timers.Timer timers = new System.Timers.Timer(50);
            timers.Elapsed += new ElapsedEventHandler((sender,args)=>{onReadTimeOut(callback,sender, args);}  
               
                );
            timers.Start();

         //   UnityEngine.Debug.Log("GenericHIDDevice >>>>> try read");  
            _device.Call("read", from, _listener, 0);
          //  UnityEngine.Debug.Log("GenericHIDDevice >>>>> read out");  
        }

        private void onReadTimeOut(ReadCallback callback,object sender, ElapsedEventArgs args)
        {
            System.Timers.Timer timers = ((System.Timers.Timer)sender);
            timers.Stop();
            timers = null;

           // UnityEngine.Debug.Log("timeout");

            if ((callback != null)) callback.Invoke(__lastHIDReport);
           
        }

        //public void write(final byte[] from, IReadWriteListener listener, int timeout)
        public override void Write(object data)
        {
            _device.Call("write",(byte[]) data,0);
           
        }


        public override void Write(object data, HIDDevice.WriteCallback callback)
        {
            _listener.addWriteCallback(callback);
            _device.Call("write", (byte[])data, _listener, 0);
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
#endif