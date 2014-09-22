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


    /// <summary>
    /// 
    /// </summary>
   public enum PassThruMode : byte
    {
        None = 0x00,
        Nunchuck = 0x05,
        ClassicController = 0x07,

    }


   public enum ExtensionType : byte
   {
       None = 0x00,
       Nunchuck = 0x01,
       ClassicController = 0x02,
       ClassicControllerPro = 0x04,
       Guitar = 0x08,
       Drums = 0x10,
       MotionPlus = 0x20,
       BalancedBoard = 0x40,
       TaikoDrums = 0x80
   }

   public enum ProcessingMode
   {
       None=0,
       InProgress,
       AccCalibration,
       ExtCheck,
       MPlusCheck,
       MPlusCalibration,
       Update
   }



   /// <summary>
   /// The extension number of the currently plugged into the Wiimote
   /// </summary>
   public enum ExtensionNumber : long
   {


       /// <summary>
       /// No extension
       /// </summary>
       None = 0x000000000000,
       /// <summary>
       /// Nunchuk extension
       /// </summary>
       Nunchuk = 0x0000a4200000,
       /// <summary>
       /// Classic Controller extension
       /// </summary>
       ClassicController = 0x0000a4200101,
       /// <summary>
       /// Guitar controller from Guitar Hero 3/WorldTour
       /// </summary>
       Guitar = 0x0000a4200103,
       /// <summary>
       /// Drum controller from Guitar Hero: World Tour
       /// </summary>
       Drums = 0x0100a4200103,
       /// <summary>
       /// Wii Fit Balance Board controller
       /// </summary>
       BalanceBoard = 0x0000a4200402,
       /// <summary>
       /// Taiko "TaTaCon" drum controller
       /// </summary>
       TaikoDrum = 0x0000a4200111,
       /// <summary>
       /// Wii MotionPlus extension
       /// </summary>
       MotionPlus = 0x0000a4200405,
       //MotionPlusInside has 0x0100 A420 0405
       //static const QWORD MOTION_PLUS		   = 0x050420A40000ULL;
       //	static const QWORD MOTION_PLUS_DETECT  = 0x050020a60000ULL;
       //	static const QWORD MOTION_PLUS_DETECT2 = 0x050420a60000ULL;
       /// <summary>
       /// Partially inserted extension.  This is an error condition.
       /// </summary>
       ParitallyInserted = 0xffffffffffff
   };



	class WiimoteDevice:JoystickDevice,IDisposable
	{
      

        protected uint _rumbleBit;
        protected bool _rumble;
        protected WiiExtensionType _ExtensionType;
        protected bool _extensionDevice;
        protected float _battery;
        protected IRMode _irmode;

        public byte Extensions;
        
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

        public bool isCalibrated = false;

        public WiimoteDevice(int id,int pid,int vid, int axes, int buttons,int leds,int irs,IDriver driver)
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


        public float Battery
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
