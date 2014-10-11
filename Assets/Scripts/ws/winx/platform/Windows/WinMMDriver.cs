#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32;
using UnityEngine;
using ws.winx.devices;

namespace ws.winx.platform.windows
{
    sealed class WinMMDriver : IDriver
    {
        #region Fields


        int devicesSupported;
       
       
        bool disposed;
        



		IHIDInterface _hidInterface;
        #endregion

        #region Constructors

        public WinMMDriver()
        {

            


           
        }

        #endregion







        #region IJoystickDriver implementation


        public void Update(IDevice device)
        {
			HIDReport report = _hidInterface.Read (device.PID);



            if (report.Status==HIDReport.ReadStatus.Success)
            {

                //while buttons
                int buttonInx = 0;
                var numButtons = device.Buttons.Count;

				uint ButtonsFlag=BitConverter.ToUInt32(report.Data,0);

                while (buttonInx < numButtons)
                {
                    //stick.SetButton (buttonInx, (info.Buttons & (1 << buttonInx)) != 0);
                    device.Buttons[buttonInx].value = ButtonsFlag & (1 << buttonInx);
                    buttonInx++;
                }


                //set analog axis
                IAxisDetails axisDetails;

                int axisIndex = 0;
                int numAxis = device.Axis.Count - device.numPOV * 2;//minus POV axes

             ;

                // UnityEngine.Debug.Log("XPos:"+info.XPos+" YPos:" + info.YPos + " ZPos:" + info.ZPos+" RPos:"+info.RPos+" UPos:"+info.UPos);


                //	axisDetails = joystick.Axis[0];

                //axisDetails.value = CalculateOffset((float)joyPositions[0], axisDetails.min, axisDetails.max);

                //axisDetails = joystick.Axis[1];

                //axisDetails.value = CalculateOffset((float)joyPositions[1], axisDetails.min, axisDetails.max);

				float value;

                while (axisIndex < numAxis)
                {
                    axisDetails = device.Axis[axisIndex];
                    if (axisDetails != null){

						value=BitConverter.ToInt32(report.Data,axisIndex*4+6);

                        if (axisDetails.isTrigger)
                            axisDetails.value = NormalizeTrigger(value, axisDetails.min, axisDetails.max);
                        else
                            axisDetails.value = NormalizeAxis(value, axisDetails.min, axisDetails.max);
					}

                    axisIndex++;
                }


                //UnityEngine.Debug.Log("YPos: "+info.YPos+"Joy:"+joystick.ID+" vle"+joystick.Axis[JoystickAxis.AxisY].value);

                //set Point of View(Hat) if exist
                if ((device.numPOV) > 0)
                {

                    int x = 0;
                    int y = 0;


                    //	UnityEngine.Debug.Log("Pov is"+povPos);
					JoystickPovPosition povPos=(JoystickPovPosition)BitConverter.ToUInt16(report.Data,4);

                    if (povPos != JoystickPovPosition.Centered)
                    {
                        if (povPos > JoystickPovPosition.Left || povPos < JoystickPovPosition.Right)
                        { y = 1; }
                        if ((povPos > 0) && (povPos < JoystickPovPosition.Backward))
                        { x = 1; }
                        if ((povPos > JoystickPovPosition.Right) && (povPos < JoystickPovPosition.Left))
                        { y = -1; }
                        if (povPos > JoystickPovPosition.Backward)
                        { x = -1; }
                    }

                    //UnityEngine.Debug.Log(x+" "+y);


                    device.Axis[JoystickAxis.AxisPovX].value = x;
                    device.Axis[JoystickAxis.AxisPovY].value = y;



                }





            }
          
        }










        public IDevice ResolveDevice(IHIDDevice hidDevice)
        {
			_hidInterface = hidDevice.hidInterface;
				
				JoystickDevice joystick;

            //Get jostick capabilities
            Native.JoyCaps caps;
            Native.JoystickError result = Native.JoystickError.InvalidParameters;

            int i;
            for (i = 0; i < 16; i++)
            {

                result = Native.joyGetDevCaps(i, out caps, Native.JoyCaps.SizeInBytes);


                if (result == Native.JoystickError.NoError && caps.PID == hidDevice.PID && hidDevice.VID == caps.VID)
                {


                    //UnityEngine.Debug.Log("ID:"+i+" on PID:"+info.PID+" VID:"+info.VID+" info:"+info.DevicePath+"Bts"+caps.NumButtons.ToString()+"axes"+caps.NumAxes
                    //  +"ProdID"+caps.PID+" name:"+caps.ProductName+" regkey"+caps.RegKey+"Man:"+caps.VID);







                    int num_axes = caps.NumAxes;


                    joystick = new JoystickDevice(i, hidDevice.PID, hidDevice.VID, 8, caps.NumButtons, this);
                    joystick.Extension = new WinDefaultExtension();

                    int buttonIndex = 0;
                    for (; buttonIndex < caps.NumButtons; buttonIndex++)
                    {
                        joystick.Buttons[buttonIndex] = new ButtonDetails();
                    }


                    // Make sure to reverse the vertical axes, so that +1 points up and -1 points down.
                    int axis = 0;
                    AxisDetails axisDetails;
                    if (axis < num_axes)
                    {
                        axisDetails = new AxisDetails();
                        axisDetails.max = caps.XMax;
                        axisDetails.min = caps.XMin;
                        joystick.Axis[axis] = axisDetails;
                        axis++;
                    }
                    if (axis < num_axes)
                    {
                        axisDetails = new AxisDetails();
                        axisDetails.max = caps.YMax;
                        axisDetails.min = caps.YMin;
                        joystick.Axis[axis] = axisDetails;

                       

                      
                        //	stick.Details.Min[axis] = caps.YMin; stick.Details.Max[axis] = caps.YMax; 
                        axis++;
                    }
                    if (axis < num_axes)
                    {
                        axisDetails = new AxisDetails();
                        axisDetails.max = caps.ZMax;
                        axisDetails.min = caps.ZMin;
                        joystick.Axis[axis] = axisDetails;
                      
                        axis++;
                    }

                    if (axis < num_axes)
                    {
                        axisDetails = new AxisDetails();
                        axisDetails.min = caps.RMin;
                        axisDetails.max = caps.RMax;
                        joystick.Axis[axis] = axisDetails;
                      
                        axis++;
                    }

                    if (axis < num_axes)
                    {
                        axisDetails = new AxisDetails();
                        axisDetails.min = caps.UMin;
                        axisDetails.max = caps.UMax;
                        joystick.Axis[axis] = axisDetails;
                        axis++;
                    }

                    if (axis < num_axes)
                    {
                        axisDetails = new AxisDetails();
                        axisDetails.max = caps.VMax;
                        axisDetails.min = caps.VMin;
                        joystick.Axis[axis] = axisDetails;
                        axis++;
                    }

                    if ((caps.Capabilities & Native.JoystCapsFlags.HasPov) != 0)
                    {
                        joystick.Axis[JoystickAxis.AxisPovX] = new AxisDetails();
                        joystick.Axis[JoystickAxis.AxisPovY] = new AxisDetails();


                        joystick.numPOV = 1;

//                        WinDefaultExtension extension = joystick.Extension as WinDefaultExtension;
//
//                        extension.PovType = Native.PovType.Exists;
//                        if ((caps.Capabilities & Native.JoystCapsFlags.HasPov4Dir) != 0)
//                            extension.PovType |= Native.PovType.Discrete;
//                        if ((caps.Capabilities & Native.JoystCapsFlags.HasPovContinuous) != 0)
//                            extension.PovType |= Native.PovType.Continuous;
                    }



                  //  UnityEngine.Debug.Log(" max:" + caps.YMax + " min:" + caps.YMin + " max:" + caps.ZMax + " min:" + caps.ZMin);

                   // UnityEngine.Debug.Log(" max:" + caps.RMax + " min:" + caps.RMin + " max:" + caps.UMax + " min:" + caps.UMin);

                    return joystick;


                }
            }


            return null;

            //return (IDevice<IAxisDetails, IButtonDetails, IDeviceExtension>)joystick;
        }
        #endregion


        /// <summary>
        ///  Normalize raw axis value to 0-1 range.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="dreadZone"></param>
        /// <returns></returns>
        public float NormalizeTrigger(float pos, int min, int max, float dreadZone = 0.001f)
        {
            float value = 1 - pos / (max - min);

            UnityEngine.Debug.Log("trigger:"+pos);

            if (value > 1)
                return 1;

            if (value < dreadZone && value > -dreadZone)
                return 0;

            return value;

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
            //value = (ivalue - devicePrivate->axisRanges[axisIndex][0]) / (float) (devicePrivate->axisRanges[axisIndex][1] - devicePrivate->axisRanges[axisIndex][0]) * 2.0f - 1.0f;


            float value = (2 * (pos - min)) / (max - min) - 1;
            if (value > 1)
                return 1;
            else if (value < -1)
                return -1;
            else if (value < dreadZone && value > -dreadZone)
                return 0;
            else
                return value;
        }




        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                }

                disposed = true;
            }
        }

        ~WinMMDriver()
        {
            Dispose(false);
        }

        #endregion

       









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
                }//set
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






        public sealed class WinDefaultExtension : IDeviceExtension
        {
            
        }









    }
}
#endif