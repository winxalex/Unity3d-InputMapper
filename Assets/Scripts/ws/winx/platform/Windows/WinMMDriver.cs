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
using ws.winx.drivers;

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
		public IDevice ResolveDevice(IHIDDevice hidDevice)
		{
			_hidInterface = hidDevice.hidInterface;
			
			JoystickDevice device;
			
			//Get jostick capabilities
			Native.JoyCaps caps;
			Native.JoystickError result = Native.JoystickError.InvalidParameters;
			
			System.Threading.Thread.Sleep(100);
			
			DeviceProfile profile = null;
			String profileKey=hidDevice.hidInterface.defaultDriver is UnityDriver? hidDevice.Name : hidDevice.VID.ToString("X4")+"#"+hidDevice.PID.ToString("X4");
			if (hidDevice.hidInterface.Profiles.ContainsKey(profileKey)) {
				try{
					profile = hidDevice.hidInterface.LoadProfile (hidDevice.hidInterface.Profiles [profileKey]);
				}catch(Exception ex){
					UnityEngine.Debug.LogException(ex);
				}
			}

			
			int i;
			for (i = 0; i < 16; i++)
			{
				
				result = Native.joyGetDevCaps(i, out caps, Native.JoyCaps.SizeInBytes);
				
				
				if (result == Native.JoystickError.NoError && caps.PID == hidDevice.PID && hidDevice.VID == caps.VID)
				{
					
					
					//UnityEngine.Debug.Log("ID:"+i+" on PID:"+info.PID+" VID:"+info.VID+" info:"+info.DevicePath+"Bts"+caps.NumButtons.ToString()+"axes"+caps.NumAxes
					//  +"ProdID"+caps.PID+" name:"+caps.ProductName+" regkey"+caps.RegKey+"Man:"+caps.VID);
					
					
					
					
					
					
					
					int num_axes = caps.NumAxes;
					
					//!!! save ordNumber(I don't have still function that would return ordNum for WinMM from PID);
					((GenericHIDDevice)hidDevice).ord = i;
					
					
					device = new JoystickDevice(hidDevice.index, hidDevice.PID, hidDevice.VID,hidDevice.ID, 8, caps.NumButtons, this);
					device.Extension = new WinDefaultExtension();
					device.Name = hidDevice.Name;
					device.profile = profile;
					
					int buttonIndex = 0;
					for (; buttonIndex < caps.NumButtons; buttonIndex++)
					{
						device.Buttons[buttonIndex] = new ButtonDetails();
						
						if (profile != null && profile.buttonNaming.Length > buttonIndex)
						{
							device.Buttons[buttonIndex].name = profile.buttonNaming[buttonIndex];
						}
					}
					
					
					// Make sure to reverse the vertical axes, so that +1 points up and -1 points down.
					int axis = 0;
					AxisDetails axisDetails;
					if (axis < num_axes)
					{
						axisDetails = new AxisDetails();
						axisDetails.max = caps.XMax;
						axisDetails.min = caps.XMin;
						device.Axis[axis] = axisDetails;
						//if(axisDetails.min==0 && axisDetails.max==255) axisDetails.isTrigger=true;
						
						if (profile != null && profile.axisNaming.Length > axis)
						{
							axisDetails.name = profile.axisNaming[axis];
							
						}
						
						axis++;
					}
					if (axis < num_axes)
					{
						axisDetails = new AxisDetails();
						axisDetails.max = caps.YMax;
						axisDetails.min = caps.YMin;
						// if (axisDetails.min == 0 && axisDetails.max == 32767) axisDetails.isTrigger = true;
						device.Axis[axis] = axisDetails;
						
						
						if (profile != null && profile.axisNaming.Length > axis)
						{
							axisDetails.name = profile.axisNaming[axis];
							
						}
						
						//	stick.Details.Min[axis] = caps.YMin; stick.Details.Max[axis] = caps.YMax; 
						axis++;
					}
					if (axis < num_axes)
					{
						axisDetails = new AxisDetails();
						axisDetails.max = caps.ZMax;
						axisDetails.min = caps.ZMin;
						//if(axisDetails.min==0) axisDetails.isTrigger=true;
						device.Axis[axis] = axisDetails;
						
						if (profile != null && profile.axisNaming.Length > axis)
						{
							axisDetails.name = profile.axisNaming[axis];
							
						}
						
						axis++;
					}
					
					if (axis < num_axes)
					{
						axisDetails = new AxisDetails();
						axisDetails.min = caps.RMin;
						axisDetails.max = caps.RMax;
						//    if (axisDetails.min == 0 && axisDetails.max == 255) axisDetails.isTrigger = true;
						device.Axis[axis] = axisDetails;
						
						if (profile != null && profile.axisNaming.Length > axis)
						{
							axisDetails.name = profile.axisNaming[axis];
							
						}
						
						axis++;
					}
					
					if (axis < num_axes)
					{
						axisDetails = new AxisDetails();
						axisDetails.min = caps.UMin;
						axisDetails.max = caps.UMax;
						//     if (axisDetails.min == 0 && axisDetails.max == 255) axisDetails.isTrigger = true;
						device.Axis[axis] = axisDetails;
						if (profile != null && profile.axisNaming.Length > axis)
						{
							axisDetails.name = profile.axisNaming[axis];
							
						}
						axis++;
					}
					
					if (axis < num_axes)
					{
						axisDetails = new AxisDetails();
						axisDetails.max = caps.VMax;
						axisDetails.min = caps.VMin;
						//     if (axisDetails.min == 0 && axisDetails.max == 255) axisDetails.isTrigger = true;
						device.Axis[axis] = axisDetails;
						
						if (profile != null && profile.axisNaming.Length > axis)
						{
							axisDetails.name = profile.axisNaming[axis];
							
						}
						axis++;
					}
					
					if ((caps.Capabilities & Native.JoystCapsFlags.HasPov) != 0)
					{
						device.Axis[JoystickAxis.AxisPovX] = new AxisDetails();
						device.Axis[JoystickAxis.AxisPovY] = new AxisDetails();
						
						if (profile != null && profile.axisNaming.Length > (int)JoystickAxis.AxisPovX)
						{
							device.Axis[JoystickAxis.AxisPovX].name = profile.axisNaming[(int)JoystickAxis.AxisPovX];
							
						}
						
						if (profile != null && profile.axisNaming.Length > (int)JoystickAxis.AxisPovY)
						{
							device.Axis[JoystickAxis.AxisPovY].name = profile.axisNaming[(int)JoystickAxis.AxisPovY];
							
						}
						
						
						device.numPOV = 1;
						
						//                        WinDefaultExtension extension = joystick.Extension as WinDefaultExtension;
						//
						//                        extension.PovType = Native.PovType.Exists;
						//                        if ((caps.Capabilities & Native.JoystCapsFlags.HasPov4Dir) != 0)
						//                            extension.PovType |= Native.PovType.Discrete;
						//                        if ((caps.Capabilities & Native.JoystCapsFlags.HasPovContinuous) != 0)
						//                            extension.PovType |= Native.PovType.Continuous;
					}
					
					
					
					//  UnityEngine.Debug.Log(" max:" + caps.YMax + " min:" + caps.YMin + " max:" + caps.ZMax + " min:" + caps.ZMin);
					
					//    UnityEngine.Debug.Log(" max:" + caps.RMax + " min:" + caps.RMin + " max:" + caps.UMax + " min:" + caps.UMin);
					
					return device;
					
					
				}
			}
			
			
			return null;
			
			//return (IDevice<IAxisDetails, IButtonDetails, IDeviceExtension>)joystick;
		}

        public void Update(IDevice device)
        {

			if (device != null && _hidInterface != null && _hidInterface.Contains (device.ID)) {
								HIDReport report = _hidInterface.ReadDefault (device.ID);

            
								//return;

								if (report!=null && (report.Status == HIDReport.ReadStatus.Success || report.Status==HIDReport.ReadStatus.Buffered)) {

										//while buttons
										int buttonInx = 0;
										var numButtons = device.Buttons.Count;

										uint ButtonsFlag = BitConverter.ToUInt32 (report.Data, 0);

										//UnityEngine.Debug.Log(BitConverter.ToString(report.Data,0,4));


										while (buttonInx < numButtons) {
												//stick.SetButton (buttonInx, (info.Buttons & (1 << buttonInx)) != 0);
												device.Buttons [buttonInx].value = ButtonsFlag & (1 << buttonInx);
												buttonInx++;
										}


										//set analog axis
										IAxisDetails axisDetails;

										int axisIndex = 0;
										int numAxis = device.Axis.Count - device.numPOV * 2;//minus POV axes

             

										// UnityEngine.Debug.Log("XPos:"+info.XPos+" YPos:" + info.YPos + " ZPos:" + info.ZPos+" RPos:"+info.RPos+" UPos:"+info.UPos);


										//	axisDetails = joystick.Axis[0];

										//axisDetails.value = CalculateOffset((float)joyPositions[0], axisDetails.min, axisDetails.max);

										//axisDetails = joystick.Axis[1];

										//axisDetails.value = CalculateOffset((float)joyPositions[1], axisDetails.min, axisDetails.max);

										float value;

										while (axisIndex < numAxis) {
												axisDetails = device.Axis [axisIndex];
												if (axisDetails != null) {

														value = BitConverter.ToInt32 (report.Data, axisIndex * 4 + 6);

                                                       
														if (axisDetails.isTrigger)
																axisDetails.value = NormalizeTrigger (value, axisDetails.min, axisDetails.max);
														else
																axisDetails.value = NormalizeAxis (value, axisDetails.min, axisDetails.max);
												}

												axisIndex++;
										}


										//UnityEngine.Debug.Log("YPos: "+info.YPos+"Joy:"+joystick.ID+" vle"+joystick.Axis[JoystickAxis.AxisY].value);

										//set Point of View(Hat) if exist
										if ((device.numPOV) > 0) {

												float x = 0;
												float y = 0;


												ushort povPos = BitConverter.ToUInt16 (report.Data, 4);

                                                if (povPos != 0xFFFF)
                                                {
                                                    
                                                        hatValueToXY(povPos / 4500, 8, out x, out y);



                                                }
                             

											//	UnityEngine.Debug.Log("povPos:"+povPos+":"+x+" "+y);


												device.Axis [JoystickAxis.AxisPovX].value = x;
												device.Axis [JoystickAxis.AxisPovY].value = y;



										}





								}
						}
          
        }



        //				  7    0     1            
        //				   \   |   /
        //				6 _____|______2
        // 					  /|\
        //					/  |  \
        //				   5   4    3

        /// <summary>
        /// Hats the value to X.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="range">Range.</param>
        /// <param name="outX">Out x.</param>
        /// <param name="outY">Out y.</param>
        void hatValueToXY(int value, int range, out float outX, out float outY)
        {

            outX = outY = 0f;
            int rangeHalf = range >> 1;
            int rangeQuat = range >> 2;

            if (value > 0 && value < rangeHalf)
            {
                outX = 1f;

            }
            else if (value > rangeHalf)
            {
                outX = -1f;
            }

            if (value > rangeQuat * 3 || value < rangeQuat)
            {
                outY = 1f;

            }
            else if (value > rangeQuat && value < rangeQuat * 3)
            {
                outY = -1f;
            }

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

            //UnityEngine.Debug.Log("trigger:"+pos);

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
            ButtonState _buttonState;
            string _name;

            #region IDeviceDetails implementation
            public string name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }


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




            public ButtonState buttonState
            {
                get { return _buttonState; }
            }



            public float value
            {
                get
                {
                    return _value;
                    //return (_buttonState==ButtonState.Hold || _buttonState==ButtonState.Down);
                }
                set
                {

                    _value = value;

                    //  UnityEngine.Debug.Log("Value:" + _value);

                    //if pressed==TRUE
                    //TODO check the code with triggers
                    if (value > 0)
                    {
                        if (_buttonState == ButtonState.None
                            || _buttonState == ButtonState.Up)
                        {

                            _buttonState = ButtonState.Down;



                        }
                        else
                        {
                            //if (buttonState == ButtonState.Down)
                            _buttonState = ButtonState.Hold;

                        }


                    }
                    else
                    { //
                        if (_buttonState == ButtonState.Down
                            || _buttonState == ButtonState.Hold)
                        {
                            _buttonState = ButtonState.Up;
                        }
                        else
                        {//if(buttonState==ButtonState.Up){
                            _buttonState = ButtonState.None;
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
            ButtonState _buttonState = ButtonState.None;
            bool _isNullable;
            bool _isHat;
            bool _isTrigger;
            string _name;

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
            public string name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }

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

            public ButtonState buttonState
            {
                get { return _buttonState; }
            }
			public float value
			{
				get { return _value; }
				set
				{
					
					if (value == -1 || value==1)
					{
						if (_buttonState == ButtonState.None)
						  //  || _buttonState == ButtonState.Up)
						{
							
							_buttonState = ButtonState.Down;
							
							//Debug.Log("val:"+value+"_buttonState:"+_buttonState);
							
						}
						else
						{
							_buttonState = ButtonState.Hold;
						}
						
						
					}
					else
					{
						
						if (_buttonState == ButtonState.Down
						    || _buttonState == ButtonState.Hold)
						{
							
							//if previous value was >0 => PosToUp
							if (_value>0)
								_buttonState = ButtonState.PosToUp;
							else
								_buttonState = ButtonState.NegToUp;
							
							//Debug.Log("val:"+value+"_buttonState:"+_buttonState);
							
						}
						else
						{//if(buttonState==ButtonState.Up){
							_buttonState = ButtonState.None;
						}
						
						
					}
					
					
					_value = value;
					
					
					
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