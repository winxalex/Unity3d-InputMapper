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

      
        public string id
        {
            get { return Name; }
            set
            {
                String[] parts;
                int inx;
                parts = value.Split('-');
                //044f-b653-NAME FF
                //Name (Vendor: 044f Product: b653) Chrome
                if (parts.Length > 2)
                {
                    Name = parts[2];
                    PID = Convert.ToInt32(parts[1].Trim(), 16);
                    VID = Convert.ToInt32(parts[0].Trim(), 16);
                }
                else
                    if ((inx = value.IndexOf("(")) > -1)
                    {
                        Name = value.Substring(0, inx - 1);
                        parts = value.Substring(inx, value.Length - inx - 1).Replace("Product", "").Split(':');

                        PID = Convert.ToInt32(parts[2].Trim(), 16);
                        VID = Convert.ToInt32(parts[1].Trim(), 16);
                    }
                    else
                        Name = value;


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


        protected void onPositionUpdate( object sender,WebMessageArgs args){
            if(_readCallback!=null){
                 WebHIDReport hidReport=(WebHIDReport)Json.Deserialize<WebHIDReport>(args.RawMessage);

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




        override public IHIDInterface hidInterface
        {
            get 
            {
                ((WebHIDInterface)this.hidInterface).webHIDBehaviour.PositionUpdateEvent += new EventHandler<WebMessageArgs>(onPositionUpdate);
                return _hidInterface;
            }
            internal set
            {
                _hidInterface = value;
            }

        }


        public GenericHIDDevice()
            : base(0, 0, 0, IntPtr.Zero, null, String.Empty)
        {
            
        }

       
    }
}
#endif