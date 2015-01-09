using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization;
using ws.winx.devices;
using ws.winx.input;

namespace ws.winx.input
{
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
	[DataContract]
#endif
		public class InputCombination
		{


				public delegate bool InputDelegate (InputEx.KeyCodeInputResolverCallback keycodeInputHandlerCallback,InputAction action,ButtonState buttonState);

				protected List<InputAction> _actionsList;
				protected InputAction[] _actions;
				protected String _combinationString;
				protected float _analogValue = 0f;
				protected float _timeDelta;
				protected bool _isActive = false;

				private InputAction __currentInputAction {
						get{ return actions [__currentIndex]; }
				}

				private int __currentIndex = 0;
				/// <summary>
				/// time when action in combination has started
				/// (by diffrence this time with the current time and comparing to Combination sensitivity we could know 
				/// if combination sequence should be reseted)
				/// </summary>
				private float __actionHappenTime;
				private float __range;
				private KeyCode __lastCode;

#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[DataMember(Name = "InputActions")]
#endif
				public List<InputAction> actionsList {
						get { return _actionsList; }
						set {
								_actionsList = value;
								_combinationString = ToString (value);
								__currentIndex = 0;

						}
				}

				public InputAction[] actions {
						get { 

								if (_actions == null || _actions.Length != _actionsList.Count)
										_actions = _actionsList.ToArray ();

								return _actions; 
			
			
						}
				
				}

				public int numActions {
						get {
								return _actionsList.Count;
						}
				}

				public bool isActive {
						get { return _isActive; }
						set { _isActive = value; }
				}

				public String combinationString {
						get {
								if (_combinationString == null)
										_combinationString = ToString (_actionsList);
								return _combinationString;
						}
						set {
								_combinationString = value;
								_actionsList.Clear ();
								parse (combinationString);
						}
				}


				/// <summary>
				/// Initializes a new instance of the <see cref="ws.winx.input.InputCombination"/> class.
				/// </summary>
				/// <param name="actions">Codes ex. KeyCodeExtension.,...</param>
				public InputCombination (params InputAction[] actions)
				{
						//_actionsList=codes.Select(entry => new InputAction(entry)).ToList();

						_actionsList = new List<InputAction> ();
						for (int i = 0; i < actions.Length; i++)
								_actionsList.Add (actions [i]);

						


				}


				/// <summary>
				/// Initializes a new instance of the <see cref="ws.winx.input.InputCombination"/> class.
				/// </summary>
				/// <param name="codes">Codes.</param>
				public InputCombination (params KeyCode[] codes)
				{
						_actionsList = new List<InputAction> ();
						for (int i = 0; i < codes.Length; i++)
								_actionsList.Add (new InputAction (codes [i]));

						
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="ws.winx.input.InputCombination"/> class.
				/// </summary>
				/// <param name="codes">Codes ex. (int)KeyCode.P,(int)Joystick2Button12,InputCodes.toCode (Joysticks joy,JoystickAxis axis,JoystickPosition pos),...</param>
				public InputCombination (params int[] codes)
				{
						//_actionsList=codes.Select(entry => new InputAction(entry)).ToList();

						_actionsList = new List<InputAction> ();
						for (int i = 0; i < codes.Length; i++)
								_actionsList.Add (new InputAction (codes [i]));

						


				}






				/// <summary>
				/// Initializes a new instance of the <see cref="ws.winx.input.InputCombination"/> class.
				/// </summary>
				/// <param name="combinations">Combinations.</param>
				public InputCombination (List<InputAction> combinations)
				{
						_actionsList = combinations;


						


				}

				/// <summary>
				/// Initializes a new instance of the <see cref="ws.winx.input.InputCombination"/> class.
				/// </summary>
				/// <param name="combinationString">Combination string.</param>
				public InputCombination (String combinationString)
				{
						_actionsList = new List<InputAction> ();

						this.combinationString = combinationString;


						

				}

				public InputAction GetActionAt (int inx)
				{
						return _actionsList.ElementAt (inx);
				}

				public void Add (InputAction action)
				{
						_actionsList.Add (action);
						_combinationString = ToString (_actionsList);
						_actions = _actionsList.ToArray ();

					
				}

				public void Clear ()
				{
						_actionsList.Clear ();
						_combinationString = String.Empty;
						_actions = null;
				}

				public bool Pop ()
				{
						return _actionsList.Remove (_actionsList.Last ());
				}

				internal bool GetInputHold (IDevice device)
				{
						if (_actionsList.Count > 1 || __currentInputAction.type != InputActionType.SINGLE) { /*Debug.LogWarning("You found need of GetInputHold with combos. Fork code on github");*/
								return false;
						} else
								return InputEx.GetInputHold (__currentInputAction,device);
								
				}

				internal bool GetInputUp (IDevice device)
				{
						if (_actionsList.Count > 1 || __currentInputAction.type != InputActionType.SINGLE) { 
								/*Debug.LogWarning("You found need of GetInputUp with combos. Fork code on github");*/
								return false;
						} else
								return InputEx.GetInputUp (__currentInputAction,device);
				}

				internal bool GetInputDown (IDevice device,bool atOnce=false)
				{
						
						if (__currentInputAction.type == InputActionType.SINGLE && _actionsList.Count == 1) {
						
								return InputEx.GetInputDown (__currentInputAction,device);				

						
						} else {
								if (atOnce) {
										int len = actions.Length;

										for (int i=0; i<len; i++) {

                                            //LONG is counted as HOLD in this mode
                                            if(actions[i].type==InputActionType.LONG){
                                                if (!InputEx.GetInputHold(actions[i],device)) return false;
                                            }else if(!InputEx.GetInputDown (actions [i],device))
														return false;
										}

										return true;
								} else // Double,Long are also count as combinations handled by InputEx.GetAction
										return GetCombinationInput (device);
						}


				}


				




				//TODO this with corutine to compare performace
				internal bool GetCombinationInput (IDevice device)
				{


						if (InputEx.GetAction (__currentInputAction,device)) {//and// if code and type are ok go in
								//	UnityEngine.Debug.Log ("CODE:" + _pointer.Current.codeString);
                     

								//save time when action happened if not saved or reseted
								if (__actionHappenTime == 0) {
										__actionHappenTime = Time.time;
								}



								//check if time from one action to the other is less then InputAction.COMBINATION_CLICK_SENSITIVITY
								if (Time.time < __actionHappenTime + InputAction.COMBINATION_CLICK_SENSITIVITY) {

										//get the time when current action of combination happened
										__actionHappenTime = Time.time;

										__currentIndex++;

										//just move to next if possible => combination happend or reset if couldn't
										if (!(__currentIndex < actions.Length)) {
												__currentIndex = 0;
												return true;
										}
								} else {//reset cos time has passed for next action
										__currentIndex = 0;
										__actionHappenTime = 0;
										InputEx.LastCode = 0;
										//	UnityEngine.Debug.Log ("Reset Time Cos Time Passed (Too late):" + Time.time + " Time Allowed:" + (__actionHappenTime + InputAction.COMBINATION_CLICK_SENSITIVITY));

								}


								//UnityEngine.Debug.Log("CodeAfter:"+_pointer.Current.codeString);

								return false;
						}
						//UnityEngine.Debug.Log("CodeAfter New Between Code or not same type:"+_pointer.Current.codeString);


						//combination stated but never continue in allowed time InputAction.COMBINATION_CLICK_SENSITIVITY
						if (__actionHappenTime > 0 && Time.time > __actionHappenTime + InputAction.COMBINATION_CLICK_SENSITIVITY) {
								//UnityEngine.Debug.Log ("Reset in Idle " + Time.time + " Time Allowed:" + (__actionHappenTime + InputAction.COMBINATION_CLICK_SENSITIVITY));

								__currentIndex = 0;
								__actionHappenTime = 0;
								InputEx.LastCode = 0;
								return false;

						}

						//TODO check current type and time waiting for type of

						// time passed while waiting for double/long action to happen => don't reset we aren't idle
						if (Time.time > __currentInputAction.startTime + InputAction.COMBINATION_CLICK_SENSITIVITY && InputEx.LastCode == __currentInputAction.code) {// or waiting for double/long action to happen => don't reset we aren't idle

								//UnityEngine.Debug.Log ("Reset in cos time waiting for double/long passed" + Time.time + " Time Allowed:" + (_pointer.Current.startTime + InputAction.COMBINATION_CLICK_SENSITIVITY));

								__currentIndex = 0;
								__actionHappenTime = 0;
								InputEx.LastCode = 0;

								return false;
                     
						}






               
						//key happend that isn't expected inbetween combination sequence
						if (InputEx.anyKeyDown && InputEx.LastCode != __currentInputAction.code && __actionHappenTime > 0) {
								// UnityEngine.Debug.Log("Last Code:"+InputEx.LastCode+" current"+_pointer.Current.codeString);
								//  UnityEngine.Debug.Log("Reset cos some other key is pressed" + InputEx.anyKeyDown + " Unity anykey:" + Input.anyKeyDown);

								__currentIndex = 0;
								__actionHappenTime = 0;
								InputEx.LastCode = 0;
								return false;
						}
                     
				


		
			
						return false;
			
				}



				/// <summary>
				/// Gets the analog value.
				/// </summary>
				/// <returns> for analog input return values -1f to 1f or inverted depending of device</returns>
				internal float GetAnalogValue (IDevice device,float sensitivity, float dreadzone, float gravity)
				{
						//if key,mouse, joy button
						if (__currentInputAction.code < InputCode.MAX_KEY_CODE || InputCode.toAxis (__currentInputAction.code) == JoystickAxis.None) {
							if(InputEx.GetInputHold(__currentInputAction,device) || InputEx.GetInputDown(__currentInputAction,device))
							return 2f * (GetGenericAnalogValue (device,sensitivity, dreadzone, gravity) -0.5f);
							else
							return 0f;
						} else {
			
							return InputEx.GetInputAnalog (__currentInputAction,device);
		
						}

				}


				/// <summary>
				/// Gets the input normalized.
				/// </summary>
				/// <returns>The input normalized.</returns>
				/// <param name="sensitivity">Sensitivity.</param>
				/// <param name="dreadzone">Dreadzone.</param>
				/// <param name="gravity">Gravity.</param>
				internal float GetInputNormalized (IDevice device,float sensitivity, float dreadzone, float gravity)
				{

						if (_actionsList.Count > 1)
								return 0;


						JoystickAxis axis;

						//if key,mouse 
						if (__currentInputAction.code < InputCode.MAX_KEY_CODE) {
								return GetGenericAnalogValue (device,sensitivity, dreadzone, gravity);

						} else 
						//or button
						if ((axis = InputCode.toAxis (__currentInputAction.code)) == JoystickAxis.None) {
										return GetGenericAnalogValue (device,sensitivity, dreadzone, gravity);
						
						} else {

								int data = InputCode.toData (__currentInputAction.code);
								if (axis != JoystickAxis.AxisPovX && axis != JoystickAxis.AxisPovY && ((JoystickPosition)data) == JoystickPosition.Full) {
										//full Axis => normalize in range 0 to 1

										//if(InputEx.GetInput (__currentInputAction)>0 && InputEx.
										return 0f;
								} else {
								
										return Math.Abs (InputEx.GetInputAnalog (__currentInputAction,device));
								}


						}




				}

				/// <summary>
				/// Gets the interpolated analog value.
				/// </summary>
				/// <returns>The interpolated analog value.</returns>
				/// <param name="sensitivity">Sensitivity.</param>
				/// <param name="dreadzone">Dreadzone.</param>
				/// <param name="gravity">Gravity.</param>
				internal float GetGenericAnalogValue (IDevice device,float sensitivity, float dreadzone, float gravity)
				{


						if (InputEx.GetInputHold (__currentInputAction,device)) {
								_isActive = true;
								_timeDelta += Time.deltaTime * sensitivity;

								//timeDelta need to go from 0 to 1  (which mean from 0 to 100% of range difference)
								_analogValue = Mathf.Lerp (0, 1, Mathf.Clamp01 (_timeDelta));


								//Debug.Log("hold"+_analogValue);


						} else { //on KeyUp reset _timeDelta
								if (InputEx.GetInputUp (__currentInputAction,device)) {
										_isActive = false;
										_timeDelta = 0f;//reset

										//	Debug.Log("UP g="+gravity);

										//if gravity is not set => drop _analogValue to 0 immidiately
										if (!(gravity > 0))
												_analogValue = 0;

										return _analogValue;
								}


								//effect of gravity
								if (_analogValue != 0) {

										_timeDelta += Time.deltaTime * gravity;
										_analogValue = Mathf.Lerp (_analogValue, 0, Mathf.Clamp01 (_timeDelta));

										//	Debug.Log("gravity");
								}
						}


						if (_analogValue < dreadzone)
								_analogValue = 0f;


						return _analogValue;


				}

				public void reset ()
				{
						_timeDelta = 0f;
						_analogValue = 0f;
						_isActive = false;
				}

				public InputCombination Clone ()
				{
						return new InputCombination (_combinationString);
				}


				/// <summary>
				/// Returns a <see cref="System.String"/> that represents the current <see cref="ws.winx.input.InputCombination"/>.
				/// </summary>
				/// <returns>A <see cref="System.String"/> that represents the current <see cref="ws.winx.input.InputCombination"/>.</returns>
				public override string ToString ()
				{
						return combinationString;
				}


			


				/// <summary>
				/// Tos the string.
				/// </summary>
				/// <returns>The string.</returns>
				/// <param name="list">List.</param>
				protected string ToString (List<InputAction> list)
				{
						StringBuilder strBuilder = new StringBuilder ();
						List<InputAction>.Enumerator ptr = list.GetEnumerator ();//Reset pointer


						while (ptr.MoveNext()) {
								strBuilder.Append (ptr.Current.ToString () + InputAction.SPACE_DESIGNATOR);
						}

						strBuilder.Remove (strBuilder.Length - 1, 1);

						return strBuilder.ToString ();
				}

				/// <summary>
				/// Parse the specified combinationString.
				/// </summary>
				/// <param name="combinationString">Combination string.</param>
				protected void parse (String combinationString)
				{

						int len = combinationString.Length;
						int inx = 0;
						int lastInx = 0;

						if ((inx = combinationString.IndexOf (InputAction.SPACE_DESIGNATOR, inx)) > -1) {

								_actionsList.Add (new InputAction (combinationString.Substring (lastInx, inx - lastInx)));
								lastInx = ++inx;

								while (inx < len) {
										if ((inx = combinationString.IndexOf (InputAction.SPACE_DESIGNATOR, inx)) > -1) {
												_actionsList.Add (new InputAction (combinationString.Substring (lastInx, inx - lastInx)));
												lastInx = ++inx;
										} else {
												_actionsList.Add (new InputAction (combinationString.Substring (lastInx, len - lastInx)));
												break;
										}

								}
						} else {
								_actionsList.Add (new InputAction (combinationString));
						}




				}


		}
}
