#if UNITY_WEBPLAYER	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.devices;
using UnityEngine;
using System.Reflection;


namespace ws.winx.platform.web
{
	public class WebDriver:IDriver
	{
        protected bool _isReady=true;
       // protected WebHIDBehaviour _webHidBehavior;
        protected IHIDInterface _hidInterface;
        protected EventHandler<WebMessageArgs> onUpdate;

        public IJoystickDevice ResolveDevice(IHIDDevice info)
        {

            GenericHIDDevice genericDevice = (GenericHIDDevice)info;

            _hidInterface = info.hidInterface;

            //(GenericHIDDevice),info.PositionUpdateEvent += new EventHandler<WebMessageArgs>(onPositionUpdate);
             // onUpdate=new EventHandler<WebMessageArgs>(onPositionUpdate);

            JoystickDevice joystick;

            int numAxes = genericDevice.numAxes; //info.Extension.axes;
            int numButtons = genericDevice.numButtons; //info.Extension.buttons;

         
              joystick = new JoystickDevice(info.index, info.PID, info.VID, Math.Max(8,numAxes),numButtons, this);
              joystick.Name = info.Name;

                    

                    int index = 0;

                    for (; index < numButtons; index++)
                    {
                        joystick.Buttons[index] = new ButtonDetails();
                    }

             
                    for (index = 0; index < numAxes; index++)
                    {
                                             
                          joystick.Axis[index] = new AxisDetails();
                       
                    }

                    //joystick.numPOV = 1;
               



                   // joystick.Axis[index - 1].isHat = true;
                   //joystick.Axis[index - 2].isHat = true;

                    return joystick;

        }


         protected void onPositionUpdate(object data){



             WebHIDReport report = (WebHIDReport)data;
            // GenericHIDDevice info = Json.Deserialize(args.Message) as GenericHIDDevice;

               

            
            
            
             JoystickDevice device = _hidInterface.Devices[report.index] as JoystickDevice; 
 
             //Device has already been updated in this call
             if(device.isReady) return;

            // UnityEngine.Debug.Log("onPositionUpdate:Joy" + info.index);
             

            // UnityEngine.Debug.Log(device);
            // UnityEngine.Debug.Log(info.axes);
            // UnityEngine.Debug.Log(info.axes.Count + " " + device.Axis.Count);


            //UPDATING AXIS BUTTONS
            
            //UnityEngine.Debug.Log("buttons" + info.buttons.);
            // UnityEngine.Debug.Log("buttons" + info.buttons.Count);
            
            int i=0;
              
             foreach (var obj in report.buttons)
             {
                 
                 //if(i==0) {_webHidBehavior.Log("button:"+obj+" "+obj.GetType()+" "+Convert.ToSingle(obj));
                 //    device.Buttons[i++].value = Convert.ToSingle(obj);
                 //}

                 device.Buttons[i++].value = Convert.ToSingle(obj);

             }


//_webHidBehavior.Log(device.Buttons[0].value.ToString() + device.Buttons[1].value.ToString()+device.Buttons[2].value.ToString());

             
             i = 0;
             float value = 0;

             float dreadZone=0.001f;//TODO this shouldn't be hard coded
            // string axisValues="";
           //  int numAxes=device.Axis.Count;


             //!!! FF gives 7 axis and the last 2 are POW on test Thrustmaster
             //!!! Chrome gives 10 axis and last 4 are POW 2 by 2 but they give some strange values 

             foreach (var obj in report.axes){

                 value=Convert.ToSingle(obj);
                if(value < dreadZone && value > -dreadZone)value = 0;

                 //LAST 4 axes are POV probably
                //if (numAxes > 8)
                //{
                //    if (i == (int)JoystickAxis.AxisPovX)
                //    {
                //        device.Axis[numAxes - 2].value = Convert.ToSingle(Math.Min(1f, Math.Max(-1, value)));
                //        i++;
                //        continue;
                //    }
                //    else if (i == (int)JoystickAxis.AxisPovY)
                //    {
                //        device.Axis[numAxes - 1].value = Convert.ToSingle(Math.Min(1f, Math.Max(-1, value)));
                //        i++;
                //        continue;
                //    }
                //    else if (i == numAxes - 2)
                //    {
                //        device.Axis[JoystickAxis.AxisPovX].value = Convert.ToSingle(Math.Min(1f, Math.Max(-1, value)));
                //        i++;
                //        continue;
                //    }
                //    else if (i == numAxes - 1)
                //    {
                //        device.Axis[JoystickAxis.AxisPovY].value = Convert.ToSingle(Math.Min(1f, Math.Max(-1, value)));
                //        i++;
                //        continue;
                //    }
                //}
                  
                    
                
               // UnityEngine.Debug.Log(obj.GetType());

                 device.Axis[i++].value =Convert.ToSingle(Math.Min(1f, Math.Max(-1,value )));
                    // axisValues
               //  UnityEngine.Debug.Log("axes value:" +device.Axis[i-1].value);
             }
             
             
            // switch(guess always the last axes are hats
             //if (numAxes > 8)
             //{
             //    value = device.Axis[JoystickAxis.AxisPovX].value;
             //    device.Axis[JoystickAxis.AxisPovX].value = device.Axis[numAxes - 2].value;
             //    device.Axis[numAxes - 2].value = value;

             //    value = device.Axis[JoystickAxis.AxisPovY].value;
             //    device.Axis[JoystickAxis.AxisPovY].value = device.Axis[numAxes - 1].value;
             //    device.Axis[numAxes - 1].value = value;
                 
             //}


//_webHidBehavior.Log("numaxis: "+device.Axis[0].value.ToString()+device.Axis[1].value.ToString()+ device.Axis[2].value.ToString() + device.Axis[8].value.ToString()+device.Axis[9].value.ToString());

			//_webHidBehavior.Log("numaxis: "+ device.Axis[7].value.ToString() + device.Axis[8].value.ToString()+device.Axis[9].value.ToString());

            // UnityEngine.Debug.Log(device.Axis[0].value + " " + device.Axis[1].value);
              device.isReady = true;
      

             
         }

         public void Update(IJoystickDevice device)
        {

           // Debug.Log("Update"+_isReady);
            if (device.isReady)
            {
                if(_hidInterface.Generics.ContainsKey(device)){
                  
                // Debug.Log("Request Update Joy"+joystick.ID);
                ((JoystickDevice)device).isReady = false;

                     _hidInterface.Read(device,onPositionUpdate);
                }

                 //read from generic device
                 
                //_webHidBehavior.joyGetPosEx(joystick,onUpdate);
            }


            
        }






#region ButtonDetails
        public sealed class ButtonDetails : IButtonDetails
        {

#region Fields

            float _value;
            uint _uid;
            JoystickButtonState _buttonState=JoystickButtonState.None;

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

//JoystickButtonState wasState=_buttonState;
//float wasValue=_value;

  

                    _value = value;

                    

                   
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

//UnityEngine.Debug.Log("Prev val:"+wasValue+"+Value:" + _value+"Pre state:"+wasState+" _buttonState:"+_buttonState);
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
#endif