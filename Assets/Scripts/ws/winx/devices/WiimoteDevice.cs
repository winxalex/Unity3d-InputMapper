using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ws.winx.platform;



namespace ws.winx.devices
{
    /// <summary>
    /// The extension plugged into the Wiimote
    /// </summary>
    //[DataContract]
    public enum WiiExtensionType : byte
    {
        /// <summary>
        /// No extension
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Nunchuk extension
        /// </summary>
        Nunchuk = 0xfe,
        /// <summary>
        /// Classic Controller extension
        /// </summary>
        ClassicController = 0xfd,

        // hmm...what's 0xfc?

        /// <summary>
        /// Guitar controller from Guitar Hero
        /// </summary>
        Guitar = 0xfb
    };


    /// <summary>
    /// The mode of data reported for the IR sensor
    /// </summary>
   // [DataContract]
    public enum IRMode : byte
    {
        /// <summary>
        /// IR sensor off
        /// </summary>
        Off = 0x00,
        /// <summary>
        /// Basic mode
        /// </summary>
        Basic = 0x01,	// 10 bytes
        /// <summary>
        /// Extended mode
        /// </summary>
        Extended = 0x03,	// 12 bytes
        /// <summary>
        /// Full mode (unsupported)
        /// </summary>
        Full = 0x05,	// 16 bytes * 2 (format unknown)
    };




	class WiimoteDevice:JoystickDevice,IDisposable
	{
      

        protected uint _rumbleBit;
        protected bool _rumble;
        protected WiiExtensionType _ExtensionType;
        protected bool _extensionDevice;
        protected byte _battery;
        protected IRMode _irmode;
        
        #region IRSensor
        public class IRSensor{

            public IRSensor()
            {
                _found = false;
                _size=0;
                ir_axis_collection = new JoystickAxisCollection<IAxisDetails>(2);
            }

            protected int _size;
            protected bool _found;
            private JoystickAxisCollection<IAxisDetails> ir_axis_collection;


            public JoystickAxisCollection<IAxisDetails> Axis
            {
                get { return ir_axis_collection; }
            }

            public int Size
            {
              get { return _size; }
              set { _size = value; }
            }

            public bool Found
            {
              get { return _found; }
              set { _found = value; }
            }
        }

        #endregion

        protected IRSensor[] _IR_SENSORS;

        public IRSensor[] IR_SENSORS
        {
            get { return _IR_SENSORS; }
           
        }

        protected Vector2 _irPoint;

      
       

        protected bool[] _LED;

        public bool[] LED
        {
            get { return _LED; }
           
        }

  

        // sure, we could find this out the hard way using HID, but trust me, it's 22
        private const int REPORT_LENGTH = 22;


        public WiimoteDevice(int id,int pid,int vid, int axes, int buttons,int leds,int irs,IJoystickDriver driver)
            : base(id,pid,vid,axes,buttons,driver)
        {

            _IR_SENSORS = new IRSensor[irs];
            _irPoint = new Vector2();
                  
               for(int i=0;i<irs;i++)
                   IR_SENSORS[i]=new IRSensor();
               


            _LED = new bool[leds];

        }


     



        public IRMode Mode
        {
            get { return _irmode; }
            set { _irmode = value; }
        }


        public WiiExtensionType ExtensionType
        {
            get
            {
                return _ExtensionType;
            }
            set
            {
                _ExtensionType = value;

               
            }
        }


        public byte Battery
        {
            get
            {
                return _battery;
            }
            set
            {
                _battery = value;


            }
        }


        public bool hasExtensionDevice
        {
            get
            {
                return _extensionDevice;
            }
            set
            {
                _extensionDevice = value;

             
            }
        }

        public bool Rumble
        {
            get
            {
                return _rumble;
            }
            set
            {
                _rumble = value;

                _rumbleBit = _rumble ? (uint)0x1 : (uint)0x0;
            }
        }

        internal uint RumbleBit
        {
            get
            {
               return _rumbleBit;
            }
        }


         public float GetExtensionYaw(){

           throw new NotImplementedException();
        }

         public float GetExtensionRaw(){

           throw new NotImplementedException();
        }

        public float GetYaw(){
           throw new NotImplementedException();
        }

         public float GetPitch(){

           throw new NotImplementedException();
        }
         public float GetRoll(){
             
//              bool wiimote::EstimateOrientationFrom (wiimote_state::acceleration &accel)
//    {
//    // Orientation estimate from acceleration data (shared between wiimote and nunchuk)
//    //  return true if the orientation was updated

//    //  assume the controller is stationary if the acceleration vector is near
//    //  1g for several updates (this may not always be correct)
//    float length_sq = square(accel.X) + square(accel.Y) + square(accel.Z);

//    // TODO: as I'm comparing _squared_ length, I really need different
//    //		  min/max epsilons...
//    #define DOT(x1,y1,z1, x2,y2,z2)	((x1*x2) + (y1*y2) + (z1*z2))

//    static const float epsilon = 0.2f;
//    if((length_sq >= (1.f-epsilon)) && (length_sq <= (1.f+epsilon)))
//        {
//        if(++WiimoteNearGUpdates < 2)
//            return false;

//        // wiimote seems to be stationary:  normalize the current acceleration
//        //  (ie. the assumed gravity vector)
//        float inv_len = 1.f / sqrt(length_sq);
//        float x = accel.X * inv_len;
//        float y = accel.Y * inv_len;
//        float z = accel.Z * inv_len;

//        // copy the values
//        accel.Orientation.X = x;
//        accel.Orientation.Y = y;
//        accel.Orientation.Z = z;

//        // and extract pitch & roll from them:
//        // (may not be optimal)
//        float pitch = -asin(y)    * 57.2957795f;
////		float roll  =  asin(x)    * 57.2957795f;
//        float roll  =  atan2(x,z) * 57.2957795f;
//        if(z < 0) {
//            pitch = (y < 0)?  180 - pitch : -180 - pitch;
//            roll  = (x < 0)? -180 - roll  :  180 - roll;
//            }

//        accel.Orientation.Pitch = pitch;
//        accel.Orientation.Roll  = roll;

//        // show that we just updated orientation
//        accel.Orientation.UpdateAge = 0;
//#ifdef BEEP_ON_ORIENTATION_ESTIMATE
//        Beep(2000, 1);
//#endif
//        return true; // updated
//        }

//    // not updated this time:
//    WiimoteNearGUpdates	= 0;
//    // age the last orientation update
//    accel.Orientation.UpdateAge++;
//    return false;
//    }
           throw new NotImplementedException();
        }
       


        public UnityEngine.Vector2 GetIRPoint(){



            switch (this.Mode)
            {
                case IRMode.Basic:
                    if (this.IR_SENSORS[0].Found && this.IR_SENSORS[1].Found)
                    {
                        _irPoint.x = (this.IR_SENSORS[0].Axis[JoystickAxis.AxisX].value + this.IR_SENSORS[1].Axis[JoystickAxis.AxisX].value) / 2;
                        _irPoint.y = (this.IR_SENSORS[0].Axis[JoystickAxis.AxisY].value + this.IR_SENSORS[1].Axis[JoystickAxis.AxisY].value) / 2;

                    }
                    break;
                case IRMode.Extended:
                    
                    float sumX=0f;
                    float sumY=0f;
                    int len=IR_SENSORS.Length;
                    int activSensors = 0;

                    for (int i=0; i < len; i++)
                    {
                        if(IR_SENSORS[i].Found){
                            activSensors++;
                            sumX+=IR_SENSORS[i].Axis[0].value;
                            sumY+=IR_SENSORS[i].Axis[1].value;
                        }
                    }

                    if(activSensors>0){
                        _irPoint.x=sumX/activSensors;
                        _irPoint.y=sumY/activSensors;
                    }





                 break;
            }



return _irPoint;
          
           
        }

        /// <summary>
        /// Request Disconnaection
        /// </summary>
        public void Disconnect()
        {
          
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
