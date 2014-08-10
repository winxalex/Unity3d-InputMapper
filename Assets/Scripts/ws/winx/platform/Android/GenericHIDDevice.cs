#if UNITY_WEBPLAYER	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.devices;

namespace ws.winx.platform.android
{
    public class GenericHIDDevice : HIDDevice
    {

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

      
    

        public override void Read(HIDDevice.ReadCallback callback)
        {
            
        }


        public override void Write(object data)
        {
            _hidInterface.Write(data,);
        }


       




        protected long _timestamp;
        private ReadCallback _readCallback;
        private IHIDInterface _hidInterface;

        public long timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }




      

       
    }
}
#endif