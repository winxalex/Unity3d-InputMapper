using System;
using UnityEngine;
using ws.winx.input;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ws.winx.devices;

namespace ws.winx.input
{


	#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
	[DataContract()]
    #endif
    public class InputAction
    {
		/*
		 * Don't use letters for designators
		 */
		public static String DOUBLE_DESIGNATOR = "(x2)";
		public static String ANALOG_DESIGNATOR ="(~)";
		public static String LONG_DESIGNATOR = "(-)";
		public static String SHIFT_DESIGNATOR = "#";
		public static char SPACE_DESIGNATOR ='+';


		/* keyboard/mouse/joystick key/button sensitivity time between 2 clicks 
         * (time between 2 clicks)
		 */
		public static float DOUBLE_CLICK_SENSITIVITY = 0.2f;
		public static float SINGLE_CLICK_SENSITIVITY = 0.3f;
		public static float LONG_CLICK_SENSITIVITY = 0.4f;
		public static float COMBINATION_CLICK_SENSITIVITY=0.55f;//Combination click sens should be > then long click so you can made combos with long clicks

	

		
		



		private int __defaultCode=0;
		private InputActionType __defaultType=InputActionType.SINGLE;

		protected bool _isKey=false;
		protected bool _isJoystick=false;
		protected bool _isMouse=false;
		protected bool _fromAny=false;
			
	    #if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[IgnoreDataMemberAttribute]
        #endif
		protected int _code=0;//KeyCode.None;
		
        #if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[IgnoreDataMemberAttribute]
        #endif
        protected InputActionType _type=InputActionType.SINGLE;
		
         #if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[IgnoreDataMemberAttribute]
        #endif
        protected String _codeString;
        public float startTime;

        #if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[DataMember(Name = "Code")]
        #endif
        public String codeString{
			get{ 
				if(_codeString==null) _codeString=ToString();
				return _codeString;
			}
			set{
				_codeString=value;
				parse(value);
			}
		}

		public bool fromAny{
			get{

				return _fromAny;}
		}

		public int code{
			get{return _code;}
			set{_code=value;}
		}

		public bool isKey { get{ return _isKey;} }
		public bool isMouse { get{ return _isMouse;} }
		public bool isJoystick {get{ return _isJoystick;} }
		
		public InputActionType type{
			get{return _type;}
			set{_type=value;}
		}






		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.input.InputAction"/> class.
		/// </summary>
		/// <param name="code">Code.</param>
		/// <param name="type">Type.</param>
		public InputAction(int code,InputActionType type=InputActionType.SINGLE){
			if(code<KeyCodeExtension.MAX_KEY_CODE){

				code=KeyCodeExtension.toCode((KeyCode) code);

			}

				if(KeyCodeExtension.toJoystickID(code)==(int)Joysticks.Joystick) _fromAny=true;


			//Debug.Log("From Any:"+_fromAny);
			__defaultCode=_code=code;
			__defaultType=_type=type;

		}
	      

		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.input.InputAction"/> class.
		/// </summary>
		/// <param name="code">Code.</param>
		/// <param name="type">Type.</param>
		public InputAction(KeyCode code,InputActionType type=InputActionType.SINGLE){
		

			__defaultCode=_code=KeyCodeExtension.toCode(code);

			__defaultType=_type=type;

			if(KeyCodeExtension.toJoystickID(_code)==(int)Joysticks.Joystick) _fromAny=true;
			Debug.Log("From Any:"+_fromAny);

			_codeString=ToString();

			//if(_codeString.IndexOf("Joy")>-1) throw new Exception("Use JoystickDevice.toCode function for Joystick inputs");

			if((_isMouse=_codeString.IndexOf("Mou")>-1)){
				
				_isKey=true;
			}
				
			if(_type!=InputActionType.SINGLE)
					_codeString+=_type.ToDesignatorString();



		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.input.InputAction"/> class.
		/// </summary>
		/// <param name="code">Code in format like "Mouse1+Joystick12AxisXPositive(x2)+B(-)"</param>
		public InputAction(String code){
			_codeString=code;
			parse(code);
//			Debug.Log("From Any:"+_fromAny);
		}





		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void reset(){
			_code=__defaultCode;
			_type=__defaultType;
		}



		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ws.winx.input.InputAction"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ws.winx.input.InputAction"/>.</returns>
		public override string ToString(){
			return ToEnumString(_code)+_type.ToDesignatorString();
		}



		/// <summary>
		/// Tos the enum string.
		/// </summary>
		/// <returns>The enum string.</returns>
		/// <param name="code">Code.</param>
		protected virtual string ToEnumString(int code){
		
           // InputManager.keyCodeExtension
            return KeyCodeExtension.toEnumString(code);
		
		

		}
	


		/// <summary>
		/// Parse the specified code.
		/// That could be some KeyCode or additional like
		/// Joystick3AxisYNegative | Joystick2PovAxisXForward 
		///
		/// </summary>
		/// <param name="code">Code.</param>
		protected void parse(String code){

			_isJoystick=code.IndexOf("Joy")>-1;

			if((_isMouse=code.IndexOf("Mou")>-1) && _isJoystick){

				_isKey=true;
			}

			_type=InputActionType.SINGLE;

			  if(code.Contains(InputAction.DOUBLE_DESIGNATOR)){
				_type=InputActionType.DOUBLE;
				code=code.Replace(InputAction.DOUBLE_DESIGNATOR,"");


			}else if(code.Contains(InputAction.LONG_DESIGNATOR)){
				_type=InputActionType.LONG;
				code=code.Replace(InputAction.LONG_DESIGNATOR,"");

			 }

			if(_isJoystick){ 
				 


					_code=KeyCodeExtension.toCode(code);


					if(KeyCodeExtension.toJoystickID(_code)==(int)Joysticks.Joystick) _fromAny=true;


			}
			else{
				if(_isKey) code=code.ToUpper();
				_code=(int)Enum.Parse(typeof(KeyCode),code);
			}





			__defaultType=_type;
			__defaultCode=_code;

		}

	
        

        
    
    }
}
