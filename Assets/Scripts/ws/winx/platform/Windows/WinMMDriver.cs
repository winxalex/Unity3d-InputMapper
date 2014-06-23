﻿using System;
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
    sealed class WinMMDriver : IJoystickDriver
    {
        #region Fields


        int devicesSupported;
        JoyInfoEx info;
        int[] joyPositions;
        bool disposed;
        IHIDInterface _hidInterface;

        #endregion

        #region Constructors

        public WinMMDriver()
        {

            info = new JoyInfoEx();
            info.Size = JoyInfoEx.SizeInBytes;
            info.Flags = JoystickFlags.All;


            joyPositions = new int[6];
        }

        #endregion







        //        #region Private Members
        //
        //
        //
        //
        //				JoystickDevice<WinMMJoyDetails> OpenJoystick (int number)
        //				{
        //						JoystickDevice<WinMMJoyDetails> stick = null;
        //
        //						JoyCaps caps;
        //						JoystickError result = UnsafeNativeMethods.joyGetDevCaps (number, out caps, JoyCaps.SizeInBytes);
        //
        //
        //						if (result != JoystickError.NoError)
        //								return null;
        //
        //						int num_axes = caps.NumAxes;
        //						if ((caps.Capabilities & JoystCapsFlags.HasPov) != 0)
        //								num_axes += 2;
        //
        //						stick = new JoystickDevice<WinMMJoyDetails> (number, num_axes, caps.NumButtons);
        //						stick.Details = new WinMMJoyDetails (num_axes);
        //
        //						// Make sure to reverse the vertical axes, so that +1 points up and -1 points down.
        //						int axis = 0;
        //						if (axis < caps.NumAxes) {
        //								stick.Details.Min [axis] = caps.XMin;
        //								stick.Details.Max [axis] = caps.XMax;
        //								axis++;
        //						}
        //						if (axis < caps.NumAxes) {
        //								stick.Details.Min [axis] = caps.YMax;
        //								stick.Details.Max [axis] = caps.YMin; 
        //								//	stick.Details.Min[axis] = caps.YMin; stick.Details.Max[axis] = caps.YMax; 
        //								axis++;
        //						}
        //						if (axis < caps.NumAxes) {
        //								stick.Details.Min [axis] = caps.ZMax;
        //								stick.Details.Max [axis] = caps.ZMin;
        //								axis++;
        //						}
        //						if (axis < caps.NumAxes) {
        //								stick.Details.Min [axis] = caps.RMin;
        //								stick.Details.Max [axis] = caps.RMax;
        //								axis++;
        //						}
        //						if (axis < caps.NumAxes) {
        //								stick.Details.Min [axis] = caps.UMin;
        //								stick.Details.Max [axis] = caps.UMax;
        //								axis++;
        //						}
        //						if (axis < caps.NumAxes) {
        //								stick.Details.Min [axis] = caps.VMax;
        //								stick.Details.Max [axis] = caps.VMin;
        //								axis++;
        //						}
        //
        //						if ((caps.Capabilities & JoystCapsFlags.HasPov) != 0) {
        //								stick.Details.PovType = PovType.Exists;
        //								if ((caps.Capabilities & JoystCapsFlags.HasPov4Dir) != 0)
        //										stick.Details.PovType |= PovType.Discrete;
        //								if ((caps.Capabilities & JoystCapsFlags.HasPovContinuous) != 0)
        //										stick.Details.PovType |= PovType.Continuous;
        //						}
        //
        //          
        //						stick.Description = String.Format ("Joystick/Joystick #{0} ({1} axes, {2} buttons)", number, stick.Axis.Count, stick.numButtons);
        //						// Todo: Try to get the device name from the registry. Oh joy!
        //						//string key_path = String.Format("{0}\\{1}\\{2}", RegistryJoyConfig, caps.RegKey, RegstryJoyCurrent);
        //						//RegistryKey key = Registry.LocalMachine.OpenSubKey(key_path, false);
        //						//if (key == null)
        //						//    key = Registry.CurrentUser.OpenSubKey(key_path, false);
        //						//if (key == null)
        //						//    stick.Description = String.Format("USB Joystick {0} ({1} axes, {2} buttons)", number, stick.Axis.Count, stick.Button.Count);
        //						//else
        //						//{
        //						//    key.Close();
        //						//}
        //
        //						if (stick != null)
        //								UnityEngine.Debug.Log (String.Format ("Found joystick on device number {0}", number));
        //						return stick;
        //				}
        //
        //        #endregion

     

		#region IJoystickDriver implementation


        public void Update(IJoystickDevice joystick)
		//public void Update (IJoystickDevice<IAxisDetails, IButtonDetails, IDeviceExtension> joystick)
		{
			           


		//	UnityEngine.Debug.Log("Update Joy"+joystick.ID);

            //thru to get joystick info
            JoystickError result = UnsafeNativeMethods.joyGetPosEx(joystick.ID, ref info);

                //        for(int i=0;i<16;i++){
			
                //                result= UnsafeNativeMethods.joyGetPosEx(i, ref info);
                //UnityEngine.Debug.Log(result);
                //            if (result == JoystickError.NoError ){
                //                break;
                //                //UnityEngine.Debug.Log("ID:"+i+" on PID:"+info.PID+" ProductId"+caps.ProductId+" info:"+info.DevicePath);
                //            }
                //        }

            if (result == JoystickError.NoError)
            {

                //while buttons
                int buttonInx = 0;
                var numButtons = joystick.Buttons.Count;
                while (buttonInx < numButtons)
                {
                    //stick.SetButton (buttonInx, (info.Buttons & (1 << buttonInx)) != 0);
                    joystick.Buttons[buttonInx].value = info.Buttons & (1 << buttonInx);
                    buttonInx++;
                }


                //set analog axis
                IAxisDetails axisDetails;

                int axisIndex = 0;
                int numAxis = joystick.Axis.Count-2;//minus POV axes

                joyPositions[0] = info.XPos;
                joyPositions[1] = info.YPos;
                joyPositions[2] = info.ZPos;
                joyPositions[3] = info.RPos;
                joyPositions[4] = info.UPos;
                joyPositions[5] = info.VPos;




                while (axisIndex < numAxis)
                {
                    axisDetails = joystick.Axis[axisIndex];
                    axisDetails.value = CalculateOffset((float)joyPositions[axisIndex], axisDetails.min, axisDetails.max);


                    axisIndex++;
                }


				//UnityEngine.Debug.Log("YPos: "+info.YPos+"Joy:"+joystick.ID+" vle"+joystick.Axis[JoystickAxis.AxisY].value);

                //set Point of View(Hat) if exist
                if ((joystick.numPOV) != 0)
                {

                    int x = 0;
                    int y = 0;


				//	UnityEngine.Debug.Log("Pov is"+info.Pov);


					if ((JoystickPovPosition)info.Pov != JoystickPovPosition.Centered)
                    {
						if (info.Pov > (ushort)JoystickPovPosition.Left || info.Pov < (ushort)JoystickPovPosition.Right)
                        { y = 1; }
                        if ((info.Pov > 0) && (info.Pov < (int)JoystickPovPosition.Backward))
                        { x = 1; }
						if ((info.Pov > (ushort)JoystickPovPosition.Right) && (info.Pov < (ushort)JoystickPovPosition.Left))
                        { y = -1; }
						if (info.Pov > (ushort)JoystickPovPosition.Backward)
                        { x = -1; }
                    }

					//UnityEngine.Debug.Log(x+" "+y);
                    joystick.Axis[JoystickAxis.AxisPovX].value = x;
                    joystick.Axis[JoystickAxis.AxisPovY].value = y;



                }





            }
            else
            {
				if(_hidInterface.Devices.ContainsKey(joystick.ID)){
                _hidInterface.Devices.Remove(joystick.ID);
                UnityEngine.Debug.LogWarning("Device ID:"+joystick.ID+" PID:" + joystick.PID + " VID:" + joystick.VID + " Disconnected! Removed from HID!");
				}
            }
        }










        public IJoystickDevice ResolveDevice(IHIDDeviceInfo info) 
       // public IJoystickDevice<IAxisDetails, IButtonDetails, IDeviceExtension> ResolveDevice(IHIDDeviceInfo info)
        {
			_hidInterface=info.hidInterface;

            return CreateDevice(info);
        }

        public IJoystickDevice CreateDevice(IHIDDeviceInfo info)
       // public IJoystickDevice<IAxisDetails, IButtonDetails, IDeviceExtension> CreateDevice(IHIDDeviceInfo info)
        {
            JoystickDevice joystick;

            //Get jostick capabilities
			JoyCaps caps;
			JoystickError result=JoystickError.InvalidParameters;//info.id

			//TODO uncomment this if there is problems with devices
//			for(int i=0;i<16;i++){
//
//				result= UnsafeNativeMethods.joyGetDevCaps(i, out caps, JoyCaps.SizeInBytes);
//
//				if (result == JoystickError.NoError  && caps.ProductId==info.PID){
//					break;
//					//UnityEngine.Debug.Log("ID:"+i+" on PID:"+info.PID+" ProductId"+caps.ProductId+" info:"+info.DevicePath);
//				}
//			}

			result=UnsafeNativeMethods.joyGetDevCaps(info.id, out caps, JoyCaps.SizeInBytes);

            if (result != JoystickError.NoError)
             return null;

            int num_axes = caps.NumAxes;
            if ((caps.Capabilities & JoystCapsFlags.HasPov) != 0)
            {
                num_axes += 2;
            }

           

            joystick = new JoystickDevice(info.id, num_axes, caps.NumButtons);
            joystick.Extension = new WinDefaultExtension();

			int buttonIndex=0;
			for(;buttonIndex<caps.NumButtons;buttonIndex++){
				joystick.Buttons[buttonIndex]=new ButtonDetails();
			}


            // Make sure to reverse the vertical axes, so that +1 points up and -1 points down.
			int axis = 0;
			AxisDetails axisDetails;
            if (axis < caps.NumAxes)
            {
				axisDetails=new AxisDetails();
                axisDetails.max = caps.XMax;
				axisDetails.min = caps.XMin;
				joystick.Axis[axis]=axisDetails;
                axis++;
            }
            if (axis < caps.NumAxes)
            {
				axisDetails=new AxisDetails();
				axisDetails.max = caps.YMax;
				axisDetails.min = caps.YMin;
				joystick.Axis[axis]=axisDetails;
                //	stick.Details.Min[axis] = caps.YMin; stick.Details.Max[axis] = caps.YMax; 
                axis++;
            }
            if (axis < caps.NumAxes)
            {
				axisDetails=new AxisDetails();
				axisDetails.max = caps.ZMax;
				axisDetails.min = caps.ZMin;
				joystick.Axis[axis]=axisDetails;
                axis++;
            }
            if (axis < caps.NumAxes)
            {
				axisDetails=new AxisDetails();
				axisDetails.min = caps.RMin;
				axisDetails.max = caps.RMax;
				joystick.Axis[axis]=axisDetails;
                axis++;
            }
            if (axis < caps.NumAxes)
            {
				axisDetails=new AxisDetails();
				axisDetails.min = caps.UMin;
				axisDetails.max = caps.UMax;
				joystick.Axis[axis]=axisDetails;
                axis++;
            }
            if (axis < caps.NumAxes)
            {
				axisDetails=new AxisDetails();
				axisDetails.max = caps.VMax;
				axisDetails.min = caps.VMin;
				joystick.Axis[axis]=axisDetails;
                axis++;
            }

            if ((caps.Capabilities & JoystCapsFlags.HasPov) != 0)
            {
				joystick.Axis[JoystickAxis.AxisPovX]=new AxisDetails();
				joystick.Axis[JoystickAxis.AxisPovY]=new AxisDetails();


                joystick.numPOV = 1;
                WinDefaultExtension extension = joystick.Extension as WinDefaultExtension;

                extension.PovType = PovType.Exists;
                if ((caps.Capabilities & JoystCapsFlags.HasPov4Dir) != 0)
                    extension.PovType |= PovType.Discrete;
                if ((caps.Capabilities & JoystCapsFlags.HasPovContinuous) != 0)
                    extension.PovType |= PovType.Continuous;
            }






            return joystick;
			//return (IJoystickDevice<IAxisDetails, IButtonDetails, IDeviceExtension>)joystick;
        }
        #endregion







        public float CalculateOffset(float pos, int min, int max, float dreadZone = 0.001f)
        {
            //value = (ivalue - devicePrivate->axisRanges[axisIndex][0]) / (float) (devicePrivate->axisRanges[axisIndex][1] - devicePrivate->axisRanges[axisIndex][0]) * 2.0f - 1.0f;
			

            float offset = (2 * (pos - min)) / (max - min) - 1;
            if (offset > 1)
                return 1;
            else if (offset < -1)
                return -1;
            else if (offset < dreadZone && offset > -dreadZone)
                return 0;
            else
                return offset;
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

        #region UnsafeNativeMethods, flags, structs

        [Flags]
        public enum PovType
        {
            None = 0x0,
            Exists = 0x1,
            Discrete = 0x2,
            Continuous = 0x4
        }


        public enum JoystickError : uint
        {
            NoError = 0,
            InvalidParameters = 165,
            NoCanDo = 166,
            Unplugged = 167
            //MM_NoDriver = 6,
            //MM_InvalidParameter = 11
        }

        [Flags]
        public enum JoystCapsFlags
        {
            HasZ = 0x1,
            HasR = 0x2,
            HasU = 0x4,
            HasV = 0x8,
            HasPov = 0x16,
            HasPov4Dir = 0x32,
            HasPovContinuous = 0x64
        }


        
        public struct JoyCaps
        {
            public ushort Mid;
            public ushort ProductId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string
            ProductName;
            public int XMin;
            public int XMax;
            public int YMin;
            public int YMax;
            public int ZMin;
            public int ZMax;
            public int NumButtons;
            public int PeriodMin;
            public int PeriodMax;
            public int RMin;
            public int RMax;
            public int UMin;
            public int UMax;
            public int VMin;
            public int VMax;
            public JoystCapsFlags Capabilities;
            public int MaxAxes;
            public int NumAxes;
            public int MaxButtons;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string
            RegKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string
            OemVxD;
            public static readonly int SizeInBytes;

            static JoyCaps()
            {
                SizeInBytes = Marshal.SizeOf(default(JoyCaps));
            }
        }

        [Flags]
        enum JoystickFlags//:uint
        {
            X = 0x1,
            Y = 0x2,
            Z = 0x4,
            R = 0x8,
            U = 0x10,
            V = 0x20,
            Pov = 0x40,
            Buttons = 0x80,
//			JOY_RETURNCENTERED=0x00000400,
			All = X | Y | Z | R | U | V | Pov | Buttons
        }




        struct JoyInfoEx
        {

            public int Size;
            [MarshalAs(UnmanagedType.I4)]
            public JoystickFlags Flags;
            public int XPos;
            public int YPos;
            public int ZPos;
            public int RPos;
            public int UPos;
            public int VPos;
            public uint Buttons;
            public uint ButtonNumber;
            public ushort Pov;
            uint Reserved1;
            uint Reserved2;
            public static readonly int SizeInBytes;

            static JoyInfoEx()
            {

                SizeInBytes = Marshal.SizeOf(default(JoyInfoEx));

            }
        }

        static class UnsafeNativeMethods
        {
            [DllImport("Winmm.dll"), SuppressUnmanagedCodeSecurity]
            public static extern JoystickError joyGetPosEx(int uJoyID, ref JoyInfoEx pji);

            [DllImport("Winmm.dll"), SuppressUnmanagedCodeSecurity]
            public static extern JoystickError joyGetDevCaps(int uJoyID, out JoyCaps pjc, int cbjc);

            [DllImport("Winmm.dll"), SuppressUnmanagedCodeSecurity]
            public static extern int joyGetNumDevs();
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
            JoystickButtonState _buttonState;
            bool _isNullable;
            bool _isHat;


            #region IAxisDetails implementation


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
                    {
                        if (_buttonState == JoystickButtonState.None
                            || _buttonState == JoystickButtonState.Up)
                        {

                            _buttonState = JoystickButtonState.Down;

                        }
                        else
                        {
                            _buttonState = JoystickButtonState.Hold;
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
            public PovType PovType;
        }









    }
}