using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.platform;
using UnityEngine;
using ws.winx.devices;

namespace ws.winx.drivers
{
	public class UnityDriver:IDriver
	{


        public devices.IDevice ResolveDevice(IHIDDevice info)
        {
             int i = 0;
            JoystickDevice device=new JoystickDevice(info.index, info.PID, info.VID, 12,10, this);

            int numAxis = device.Axis.Count;
            int numButtons = device.Buttons.Count;

             for (; i < numAxis; i++)
             {
                 device.Axis[i] = new AxisDetails();
                
             }

             for (i=0; i < numButtons; i++)
             {
 device.Buttons[i] = new ButtonDetails();
             }

             return device;
        }

        public void Update(devices.IDevice device)
        {
            int i = 0;
            int numAxis = device.Axis.Count;
            int numButtons=device.Buttons.Count;
                   
            int index=device.ID;

           // Debug.Log("axis value raw:" + Input.GetAxisRaw("10") + " " + Input.GetAxis("11"));
            //Debug.Log("axis value raw:" + );
            //   joystick.Axis[0].value=Input.GetAxis("00");//index-of joystick, i-ord number of axis
              // Debug.Log("axis value:" + joystick.Axis[0].value + " state:" + joystick.Axis[0].buttonState);
            string axisValue = "";
               for (; i < numAxis; i++)
               {
                   device.Axis[i].value = (Input.GetAxis(index.ToString() + i.ToString())+1f)*0.5f;//index-of joystick, i-ord number of axis

                   axisValue += Input.GetAxis(index.ToString() + i.ToString())+" ";

               }

             //  Debug.Log(axisValue);


            for (i=0; i < numButtons; i++)
            {
               
                device.Buttons[i].value =Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (index+1)+"Button"+i))==true ? 1f:0f;

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
}
