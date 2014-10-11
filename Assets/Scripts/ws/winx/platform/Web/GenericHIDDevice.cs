#if UNITY_WEBPLAYER	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.devices;

namespace ws.winx.platform.web
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

      
		private string _id;

        public string id
        {
            get { return _id; }
            set
            {
               
				_id=value;

            }
        }


        public override void Read(HIDDevice.ReadCallback callback)
        {
             _readCallback=callback;

            //read from the device thru behavior comunicaiton channal (SendMessage technology)
             ((WebHIDInterface)this.hidInterface).webHIDBehaviour.Read(this.index);
        }


        //override public void Read(ReadCallback callback)
        //{
        //  

             
        //   // WebHIDInterface(this.hidInterface).w += callback;
        //}


		internal void onPositionUpdate( object sender,WebMessageArgs<WebHIDReport> args){
            if(_readCallback!=null){
				WebHIDReport hidReport=args.RawMessage;

                if(hidReport.index==this.index)
                  _readCallback.Invoke(hidReport);
            }
        }




        protected long _timestamp;
        private ReadCallback _readCallback;
        private IHIDInterface _hidInterface;

        public long timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }




     


        public GenericHIDDevice()
            : base(0, 0, 0, IntPtr.Zero, null, String.Empty)
        {
            
        }

       
    }
}
#endif