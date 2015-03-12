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
	[System.Serializable]
	public class InputAction
	{
		/*
		 * Don't use letters for designators
		 */
		public static String DOUBLE_DESIGNATOR = "(x2)";
		public static String ANALOG_DESIGNATOR = "(~)";
		public static String LONG_DESIGNATOR = "(-)";
		public static String SHIFT_DESIGNATOR = "#";
		public static char SPACE_DESIGNATOR = '+';
		
		
		/* keyboard/mouse/joystick key/button sensitivity time between 2 clicks 
         * (time between 2 clicks)
		 */
		public static float DOUBLE_CLICK_SENSITIVITY = 0.2f;
		public static float SINGLE_CLICK_SENSITIVITY = 0.3f;
		public static float LONG_CLICK_SENSITIVITY = 0.4f;
		public static float COMBINATION_CLICK_SENSITIVITY = 0.55f;//Combination click sens should be > then long click so you can made combos with long clicks
		
		
		
		
		
		
		
		
		private int __defaultCode = 0;
		private InputActionType __defaultType = InputActionType.SINGLE;
		protected bool _isKey = false;
		protected bool _isJoystick = false;
		protected bool _isMouse = false;
		protected bool _fromAny = false;
		
		#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[IgnoreDataMemberAttribute]
		#endif
		[field: NonSerialized]
		protected int
			_code = 0;//KeyCode.None;
		
		#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[IgnoreDataMemberAttribute]
		#endif
		[field: NonSerialized]
		protected InputActionType
			_type = InputActionType.SINGLE;
		
		#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[IgnoreDataMemberAttribute]
		#endif
		//[field: NonSerialized]
		protected String _codeString;
		public float startTime;
		
		#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[DataMember(Name = "Code")]
		#endif
		
		public String codeString {
			get {
				if (_codeString == null)
					_codeString = String.Empty;
				return _codeString;
			}
			set {
				//!!1Deserialization happen here
				
				_codeString = value;
				
				//parse TYPE
				_type = InputActionType.SINGLE;
				
				if (_codeString.Contains (InputAction.DOUBLE_DESIGNATOR)) {
					_type = InputActionType.DOUBLE;
					_codeString = _codeString.Replace (InputAction.DOUBLE_DESIGNATOR, "");
					
					
				} else if (_codeString.Contains (InputAction.LONG_DESIGNATOR)) {
					_type = InputActionType.LONG;
					_codeString = _codeString.Replace (InputAction.LONG_DESIGNATOR, "");
					
				}
				
				__defaultType = _type;
			}
		}
		
		public bool fromAny {
			get {
				
				return _fromAny;
			}
		}
		
		//public int code
		//{
		//    get { return _code; }
		//    set { _code = value; }
		//}
		
		public bool isKey { get { return _isKey; } }
		
		public bool isMouse { get { return _isMouse; } }
		
		public bool isJoystick { get { return _isJoystick; } }
		
		public InputActionType type {
			get{ return _type;}
			set {
				_type = value;
				
			}
		}
		
		
		
		
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.input.InputAction"/> class.
		/// </summary>
		/// <param name="code">Code.</param>
		/// <param name="type">Type.</param>
		public InputAction (int code, InputActionType type=InputActionType.SINGLE, IDevice device=null)
		{
			
			if (code < InputCode.MAX_KEY_CODE) {
				
				code = InputCode.toCode ((KeyCode)code);
				
			}
			
			
			__defaultCode = _code = code;
			
			__defaultType = _type = type;
			
			//if(InputCode.toDeviceInx(_code)==(int)Joysticks.Joystick) _fromAny=true;
			//Debug.Log("From Any:"+_fromAny);
			
			
			
			setCode (code, device);
			
			//if(_codeString.IndexOf("Joy")>-1) throw new Exception("Use JoystickDevice.toCode function for Joystick inputs");
			
			if ((_isMouse = _codeString.IndexOf ("Mou") > -1)) {
				
				_isKey = true;
			}
			
			
			
		}
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.input.InputAction"/> class.
		/// </summary>
		/// <param name="code">Code.</param>
		/// <param name="type">Type.</param>
		public InputAction (KeyCode code, InputActionType type=InputActionType.SINGLE, IDevice device=null):this((int)code,type,device)
		{
			
			
			
			
			
			
		}
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.input.InputAction"/> class.
		/// </summary>
		/// <param name="code">Code in format like "Mouse1 or Joystick12AxisXPositive(x2) or B(-)"</param>
		public InputAction (String code, IDevice device=null)
		{
			codeString = code;
			
		}
		
		
		
		
		
		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void reset ()
		{
			_code = __defaultCode;
			_type = __defaultType;
		}
		
		public InputAction Clone ()
		{
			
			return new InputAction (_code);
		}
		
		
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ws.winx.input.InputAction"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ws.winx.input.InputAction"/>.</returns>
		public override string ToString ()
		{
			return _codeString;
		}
		
		
		
		/// <summary>
		/// Tos the enum string.
		/// </summary>
		/// <returns>The enum string.</returns>
		/// <param name="code">Code.</param>
		protected virtual string ToEnumString (int code)
		{
			
			// InputManager.keyCodeExtension
			return InputCode.toEnumString (code);
			
			
			
		}
		
		public void setCode (int newCode, IDevice device)
		{
			
			_code = newCode;
			
			if (device == null)
				_codeString = InputCode.toEnumString (newCode) + _type.ToDesignatorString ();
			else
				_codeString = InputCode.toProfiled (newCode, device) + _type.ToDesignatorString ();
			
		}
		
		
		
		/// <summary>
		/// Gets the code of deserialized codeString based on device profile
		/// if device=null codeString is evaluated as KeyCode keyboard,mouse,joystick
		/// </summary>
		/// <returns>The code.</returns>
		/// <param name="device">Device.</param>
		public int getCode (IDevice device)
		{
			
			
			if (_code == 0)
			if (device != null && device.profile != null) {//parsing by Device profile
				
				_code = InputCode.toCode (_codeString, device.profile);
				
				
			} else { //default parsing
				
				
				_isJoystick = _codeString.IndexOf ("Joy") > -1;
				
				if ((_isMouse = _codeString.IndexOf ("Mou") > -1) && _isJoystick) {
					
					_isKey = true;
				}
				
				
				
				if (_isJoystick) {
					
					
					
					_code = InputCode.toCode (_codeString);
					
					
					// if (InputCode.toDeviceInx(_code) == (int)Joysticks.Joystick) _fromAny = true;
					
					
				} else {
					// if (_isKey) code = code.ToUpper();
					
					_code = (int)Enum.Parse (typeof(KeyCode), _codeString, true);
				}
				
			}
			
			
			
			return _code;
		}
	}
}
