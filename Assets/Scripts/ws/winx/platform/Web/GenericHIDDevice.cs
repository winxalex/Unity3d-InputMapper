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


		string _ID;
      	public new string ID {
			get {
				return _ID;
			}
			set {
				_ID=value;
			}
		}
		


        //public override void Read(HIDDevice.ReadCallback callback)
        //{
        //     _readCallback=callback;

        //    //read from the device thru behavior comunicaiton channal (SendMessage technology)
        //     ((WebHIDInterface)this.hidInterface).webHIDBehaviour.Read(this.index);
        //}


        override public HIDReport ReadBuffered(){

                 
                return __lastHIDReport;
        }

        internal void onPositionUpdate( object sender,WebMessageArgs<WebHIDReport> args){
           
				WebHIDReport hidReport=args.RawMessage;

                //TODO add syncro lock
                if(hidReport.index==this.index)
                  __lastHIDReport=hidReport;
            
        }


        //internal void onPositionUpdate( object sender,WebMessageArgs<WebHIDReport> args){
        //    if(_readCallback!=null){
        //        WebHIDReport hidReport=args.RawMessage;

        //        if(hidReport.index==this.index)
        //          _readCallback.Invoke(hidReport);
        //    }
        //}




        protected long _timestamp;
        private ReadCallback _readCallback;
        private IHIDInterface _hidInterface;

        public long timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }


  int _InputReportByteLength=8;

        override public int InputReportByteLength
        {
            get { return _InputReportByteLength; }
            set {
                if (value < 2) throw new Exception("InputReportByteLength should be >1 ");  
                _InputReportByteLength = value;
                __lastHIDReport.Data = new byte[_InputReportByteLength];
            }
        }
        
        int _OutputReportByteLength=8;

        override public int OutputReportByteLength
        {
            get { return _OutputReportByteLength; }
            set { if (value < 2) throw new Exception("InputReportByteLength should be >1 ");  _OutputReportByteLength = value; }
        }


        private byte[] CreateInputBuffer()
        {
            return CreateBuffer((int)InputReportByteLength - 1);
        }

        private byte[] CreateOutputBuffer()
        {
            return CreateBuffer((int)OutputReportByteLength - 1);
        }

        

        private static byte[] CreateBuffer(int length)
        {
            byte[] buffer = null;
            Array.Resize(ref buffer, length + 1);
            return buffer;
        }
     
         private WebHIDReport __lastHIDReport;

        public GenericHIDDevice()
            : base(0, 0, 0,String.Empty, IntPtr.Zero, null, String.Empty)
        {
            __lastHIDReport = new WebHIDReport(this.index, CreateInputBuffer(),HIDReport.ReadStatus.NoDataRead);
        }

       
    }
}
#endif