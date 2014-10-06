using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ws.winx.input;
using ws.winx.devices;
using UnityEngine;


namespace ws.winx
{
    public class MyCodeExtension
    {
       //public const int _GAMEPAD_DPAD_UP=KeyCodeExtension.toCode(Joysticks.Joystick, JoystickAxis.AxisPovY, JoystickPovPosition.Forward);
        //KeyCodeExtension.toCode(Joysticks.Joystick, JoystickAxis.AxisPovY,JoystickPovPosition.Forward)
        protected enum XinputComponents:int{
            GAMEPAD_DPAD_UP = 0x0000
                ,
         //   XINPUT_GAMEPAD_DPAD_DOWN = 0x00000002,
            //XINPUT_GAMEPAD_DPAD_LEFT = 0x00000004,
            //XINPUT_GAMEPAD_DPAD_RIGHT = 0x00000008,
            //XINPUT_GAMEPAD_START = 0x00000010,
            //XINPUT_GAMEPAD_BACK = 0x00000020,
            //XINPUT_GAMEPAD_LEFT_THUMB = 0x00000040,
            //XINPUT_GAMEPAD_RIGHT_THUMB = 0x00000080,
            //XINPUT_GAMEPAD_LEFT_SHOULDER = 0x0100,
            //XINPUT_GAMEPAD_RIGHT_SHOULDER = 0x0200,
            //XINPUT_GAMEPAD_A = 0x1000,
            //XINPUT_GAMEPAD_B = 0x2000,
            //XINPUT_GAMEPAD_X = 0x4000,
            //XINPUT_GAMEPAD_Y = 0x8000

          
        }

    
		public static KeyCodeExtension.InputActionFactory GAMEPAD_UP { get { return KeyCodeExtension.JoystickAxisYPositive; } }
	

        public class InputActionModified: InputAction
        {
            

            public InputActionModified (int code,InputActionType type): base(code, type)
            {
               
            }


            /// <summary>
            /// Tos the enum string.
            /// </summary>
            /// <returns>The enum string.</returns>
            /// <param name="code">Code.</param>
            override protected string ToEnumString(int code)
            {
                return MyCodeExtension.toEnumString(code);
               
            }
        }



      


        public static string toEnumString(int code)
        {
            if (Enum.IsDefined(typeof(XinputComponents), code))
                return ((XinputComponents)code).ToString();
            else
                return KeyCodeExtension.toEnumString(code);
        }


    }
}

	

