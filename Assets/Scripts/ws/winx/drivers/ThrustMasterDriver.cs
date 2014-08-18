using System;
using ws.winx.devices;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using ws.winx.platform;
using System.Timers;



namespace ws.winx.drivers
{
	public class ThrustMasterDriver:IDriver
	{

        private IHIDInterface __hidInterface;
       
     
      

        public IDevice ResolveDevice(IHIDDevice hidDevice)
        {
            if (hidDevice.PID == 46675 && hidDevice.VID == 1103)
            {

                JoystickDevice joystick = new ThrustmasterRGTFFDDevice(hidDevice.index, hidDevice.PID, hidDevice.VID, 8, 10, this);
               
                //JoystickDevice joystick = new JoystickDevice(hidDevice.index, hidDevice.PID, hidDevice.VID, 8, 10, this);

                this.__hidInterface = hidDevice.hidInterface;


                int index=0;
                for (; index < 10; index++)
                {
                    joystick.Buttons[index] = new ButtonDetails();
                }


                for (index = 0; index < 8; index++)
                {

                    joystick.Axis[index] = new AxisDetails();

                }


           

                    return joystick;
            }

            return null;
        }


     

        public void Update(IDevice device)
        {
            
            if(__hidInterface.Generics.ContainsKey(device))
            __hidInterface.Generics[device].Read(onRead);
           
        }


        /// <summary>
        /// Move FFD motor of the wheel left or right
        /// </summary>
        /// <param name="forceXY">0xFF - 0xA7(left) and 0x00-0x64(rights) are measurable by feeling </param>
        internal void SetMotor(IDevice device, byte forceX,byte forceY,HIDDevice.WriteCallback callback)
        {
            if (__hidInterface.Generics.ContainsKey(device))
            {
                //TODO check if device use sbytes for 0x80 to 0x7f (-128 to 127)
                //Couldn't figure out if forceY doing something

                byte[] data=new byte[5];
                data[0]=0x40;
                data[1]=forceX;
                data[2]=forceY;
                data[3] = 0x00;
                data[4] = 0x00;


                __hidInterface.Generics[device].Write(data, callback);
            }
        }
        

       
     
        protected void onRead(object data)
        {
            HIDReport report = data as HIDReport;

           

           // Debug.Log("ThustmasterDriver>>onRead:" + data);

            if (report != null && (report.Status == HIDReport.ReadStatus.Success) && report.Data[0] == 0x01)
            {


              // Debug.Log("ThustmasterDriver>>onRead:processRead" + BitConverter.ToString(report.Data));

                JoystickDevice device = __hidInterface.Devices[report.index] as JoystickDevice;

            //    Debug.Log("ThustmasterDriver>>onRead:Index" + report.index);
                //do something with the data
                //01 00 BC 87 FF FF FF FF                            ..ј‡яяяя
                //01 - is requestID (should be one for bulk data)
                //00 - button clicked info (00100011) button 0,1,5 are clicked
                //0011 1111 1111
                //
                // 87B (10bits) - Wheel axis (max. left -512 - max right 511) 

                // BC (4 bits from 2 to 6th)(min.0 to 7) FF- centered   Hat(POV)

                //FF - trigger left gear shifiter | (Break)
                //FF - trigger right shifiter  | (Gas)
                //FF - trigger (Step Shifter)
                //FF - trigger Dial
              
               






               


                if (device != null)
                {
                    int numButtons = device.Buttons.Count;
                    int buttonInx = 0;

                    // last 2bits of data2 + 8 bits of data1 
                    // 00 BE 
                    int buttonInfo = (report.Data[1] | ((report.Data[2] & 0x3) << 8));

             //       UnityEngine.Debug.Log("buttonSequence:"+Convert.ToString(buttonInfo, 2));

                    while (buttonInx < numButtons)
                    {
                        //stick.SetButton (buttonInx, (info.Buttons & (1 << buttonInx)) != 0);
                       // UnityEngine.Debug.Log(Convert.ToString(buttonInfo,2));
                        
                        device.Buttons[buttonInx].value =Convert.ToSingle(buttonInfo & (1 << buttonInx));
                      
                        buttonInx++;
                    }

                   // UnityEngine.Debug.Log("but0:" + device.Buttons[0].value + " " + device.Buttons[0].buttonState+" frame "+device.LastFrameNum+"phase:"+report.Status);


                    //HAT
                    float x=0;
                    float y=0;

                    //from 2bit to 6bit
                    int direction = (report.Data[2] >> 2) & 0xF;

                   

                    if(direction<0xff){

                        //TODO optmize this
                        if (direction==0){
                            y=1;
                           
                        }else if(direction==1){
                            y=1;
                            x=1;
                        }else if(direction==2){
                            x=1;
                        }else if(direction==3){
                            y=-1;
                            x=1;
                        }else if(direction==4){
                            y=-1;
                        }else if(direction==5){
                            y=-1;
                            x=-1;
                        }else if(direction==6){
                            x=-1;
                        }
                        else if(direction==7){
                            x=-1;
                            y=1;
                        }


                    }

                   // UnityEngine.Debug.Log("Direction:" + direction+"x="+x+" y="+y);
                    
                    device.Axis[JoystickAxis.AxisPovX].value=x;
                    device.Axis[JoystickAxis.AxisPovY].value=y;

                    //X-Axis (8 bits of Data3 + 2bits of Data2
                    device.Axis[0].value = NormalizeAxis((float)((unchecked((sbyte)report.Data[3]) << 2) | (report.Data[2] >> 6)), -512, 511);
                   // UnityEngine.Debug.Log("AxisX:" + device.Axis[0].value);


                    //Y-Axis
                    device.Axis[1].value = NormalizeTrigger((float)report.Data[4], 0, 255);
                   // UnityEngine.Debug.Log("AxisY:" + device.Axis[1].value + " state: " + device.Axis[1].buttonState);


                    //Z-Axis
                    device.Axis[2].value = NormalizeTrigger((float)report.Data[5], 0, 255);

                    device.Axis[3].value = NormalizeTrigger((float)report.Data[6], 0, 255);


                    // UnityEngine.Debug.Log("Axis:" + device.Axis[0].value +","+ device.Axis[1].value +","+ device.Axis[2].value+"," + device.Axis[3].value);
                 
                }


             
                
               
            }  
            
           
        }



             /// <summary>
        /// Normalize raw axis value to -1 to 1 range.
        /// </summary>
        /// <returns>The offset.</returns>
        /// <param name="pos">Position.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        /// <param name="dreadZone">Dread zone.</param>
        public float NormalizeAxis(float pos, int min, int max, float dreadZone = 0.001f)
        {
           float value;

          // UnityEngine.Debug.Log(pos);

            if(pos>0) value=pos/max;

            else if(pos<0) value=-pos/min;

            else return 0f;
               
            
            if (value > 1)
                return 1;
            else if (value < -1)
                return -1;
            else if (value < dreadZone && value > -dreadZone)
                return 0;
            else
                return value;
        }

        /// <summary>
        ///  Normalize raw axis value to 0 - 1 range.
        /// </summary>
        /// <param name="pos">255(released) - 0(full pressed) </param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="dreadZone"></param>
        /// <returns></returns>
        public float NormalizeTrigger(float pos, int min, int max, float dreadZone = 0.001f)
        {
            float value =1- pos / (max - min);

            //UnityEngine.Debug.Log("trigger:"+pos+ "value:"+value);

            if (value > 1)
                return 1;

            if (value < dreadZone && value > -dreadZone)
                return 0;

            return value;

        }




        internal void StopMotor(IDevice device)
        {
            byte[] data = new byte[5];
            data[0] = 0x40;
            data[1] = 0x7f;
            data[2] = 0xff;
            data[3] = 0x00;
            data[4] = 0x00;


            this.__hidInterface.Generics[device].Write(data);
               
        }

        internal void StopMotor(IDevice device, HIDDevice.WriteCallback callback)
        {
            byte[] data = new byte[5];
            data[0] = 0x40;
            data[1] = 0x7f;
            data[2] = 0xff;
            data[3] = 0x00;
            data[4] = 0x00;


            this.__hidInterface.Generics[device].Write(data, callback);
        }

    
    }
    




    #region ButtonDetails
    public sealed class ButtonDetails : IButtonDetails
    {

        #region Fields

        float _value;
        uint _uid;
        JoystickButtonState _buttonState;

        #region IDeviceDetails implementation


        public uint uid
        {
            get
            {
                return _uid;
            }
            set
            {
                _uid = value;
            }
        }




        public JoystickButtonState buttonState
        {
            get { return _buttonState; }
        }



        public float value
        {
            get
            {
                return _value;
                //return (_buttonState==JoystickButtonState.Hold || _buttonState==JoystickButtonState.Down);
            }
            set
            {

                _value = value;

                //  UnityEngine.Debug.Log("Value:" + _value);

                //if pressed==TRUE
                //TODO check the code with triggers
                if (value > 0)
                {
                    if (_buttonState == JoystickButtonState.None
                        || _buttonState == JoystickButtonState.Up)
                    {

                        _buttonState = JoystickButtonState.Down;



                    }
                    else
                    {
                        //if (buttonState == JoystickButtonState.Down)
                        _buttonState = JoystickButtonState.Hold;

                    }


                }
                else
                { //
                    if (_buttonState == JoystickButtonState.Down
                        || _buttonState == JoystickButtonState.Hold)
                    {
                        _buttonState = JoystickButtonState.Up;
                    }
                    else
                    {//if(buttonState==JoystickButtonState.Up){
                        _buttonState = JoystickButtonState.None;
                    }

                }
            }
        }
        #endregion
        #endregion

        #region Constructor
        public ButtonDetails(uint uid = 0) { this.uid = uid; }
        #endregion






    }

    #endregion

    #region AxisDetails
    public sealed class AxisDetails : IAxisDetails
    {

        #region Fields
        float _value;
        int _uid;
        int _min;
        int _max;
        JoystickButtonState _buttonState = JoystickButtonState.None;
        bool _isNullable;
        bool _isHat;
        bool _isTrigger;


        #region IAxisDetails implementation

        public bool isTrigger
        {
            get
            {
                return _isTrigger;
            }
            set
            {
                _isTrigger = value;
            }
        }


        public int min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
            }
        }


        public int max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }


        public bool isNullable
        {
            get
            {
                return _isNullable;
            }
            set
            {
                _isNullable = value;
            }
        }


        public bool isHat
        {
            get
            {
                return _isHat;
            }
            set
            {
                _isHat = value;
            }
        }


        #endregion


        #region IDeviceDetails implementation


        public uint uid
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        #endregion

        public JoystickButtonState buttonState
        {
            get { return _buttonState; }
        }
        public float value
        {
            get { return _value; }
            set
            {

                if (value == 0)
                {
                    if (_buttonState == JoystickButtonState.Down
                        || _buttonState == JoystickButtonState.Hold)
                    {

                        //axis float value isn't yet update so it have value before getting 0
                        if (_value > 0)//0 come after positive values
                            _buttonState = JoystickButtonState.PosToUp;
                        else
                            _buttonState = JoystickButtonState.NegToUp;

                    }
                    else
                    {//if(buttonState==JoystickButtonState.Up){
                        _buttonState = JoystickButtonState.None;
                    }


                }
                else
                //!!! value can jump from >0 to <0 without go to 0(might go to "Down" directly for triggers axis)
                {
                    if (_value > 0 && value < 0)
                    {
                        _buttonState = JoystickButtonState.PosToUp;
                    }
                    else if (_value < 0 && value > 0)
                    {
                        _buttonState = JoystickButtonState.NegToUp;
                    }
                    else if (_buttonState == JoystickButtonState.None
                       || _buttonState == JoystickButtonState.PosToUp || _buttonState == JoystickButtonState.NegToUp)
                    {

                        _buttonState = JoystickButtonState.Down;

                    }
                    else
                    {
                        _buttonState = JoystickButtonState.Hold;
                    }


                }




                _value = value;

                //UnityEngine.Debug.Log("ButtonState:"+_buttonState+"_value:"+_value);

            }//set
        }

        #endregion

    }

    #endregion




   
}
