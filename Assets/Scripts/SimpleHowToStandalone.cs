using UnityEngine;
using System;
using ws.winx.unity;
using ws.winx.gui;
using ws.winx.input;
using ws.winx.input.states;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ws.winx.drivers;
using ws.winx.utils;


namespace ws.winx
{

	/// <summary>
	/// Simple how to.
	/// !!! USE THIS SAMPLE WHEN U TARGETING OSX OR WIN (not with ANDROID Or WEB)
	/// </summary>
	public class SimpleHowToStandalone : MonoBehaviour
	{
		

		
		
		bool _settingsLoaded = false;
	
		
		// Use this for initialization
		void Start()
		{




			


			
		//
		

			
			//supporting devices with custom drivers
			//When you add them add specialized first then XInputDriver  then wide range supporting drivers UnityDriver
//			#if (UNITY_STANDALONE_WIN)
            //			InputManager.AddDriver(new ThrustMasterDriver());
            //			InputManager.AddDriver(new WiiDriver());
            			InputManager.AddDriver(new XInputDriver());
			//change default driver
			//InputManager.hidInterface.defaultDriver=new UnityDriver();

//			
//			#endif
//			
			#if (UNITY_STANDALONE_OSX)
           			InputManager.AddDriver(new ThrustMasterDriver());
  //          			InputManager.AddDriver(new XInputDriver());
			//change default driver
			//InputManager.hidInterface.defaultDriver=new UnityDriver();

			#endif
	
		
			InputManager.hidInterface.Enumerate();
				
			
			//if you want to load some states from .xml and add custom manually => first load settings xml then add
			//!!!Application.streamingAssetPath gives "Raw" folder in web player
			
			#if (UNITY_STANDALONE || UNITY_EDITOR ) && !UNITY_WEBPLAYER && !UNITY_ANDROID

			UserInterfaceWindow ui = this.GetComponent<UserInterfaceWindow>();

			
			
			if (ui != null && ui.settingsXML == null)
			{//settingsXML would trigger internal loading mechanism (only for testing)
				
				InputManager.loadSettings(Path.Combine(Application.streamingAssetsPath, "InputSettings.xml"));
				
				
				
				ui.StateInputCombinations = InputManager.Settings.stateInputs;
			}
			
			
			manuallyAddStateAndHandlers();
			
			#endif
			

			
	
			
		}
		
		
		void onUp(object o, EventArgs args)
		{
			Debug.Log("Up");
		}
		
		void onDown(object o, EventArgs args)
		{
			Debug.Log("Down");
		}
		
		void Handle1(object o, EventArgs args)
		{
			Debug.Log("Handle1");
		}
		
		void Handle2(object o, EventArgs args)
		{
			Debug.Log("Handle2");
		}
		

		
	
		
		
		
		
		void manuallyAddStateAndHandlers()
		{
			
			
			
			//   UnityEngine.Debug.Log(InputManager.Log());
			
			//		adding input-states pairs manually
			//			InputManager.MapStateToInput("My State1",new InputCombination(KeyCodeExtension.toCode(Joysticks.Joystick1,JoystickAxis.AxisPovX,JoystickPovPosition.Forward),(int)KeyCode.Joystick4Button9,(int)KeyCode.P,(int)KeyCode.JoystickButton0));
			//			InputManager.MapStateToInput("My State2",new InputCombination(KeyCode.Joystick4Button9,KeyCode.P,KeyCode.JoystickButton0));
			//			InputManager.MapStateToInput("My State3",new InputCombination("A(x2)+Mouse1+JoystickButton31"));
			//			InputManager.MapStateToInput("My State1",new InputCombination("Mouse1+Joystick12AxisXPositive(x2)+B"));
			
			
			
			////easiest way to map state to combination (ex.of single W and C click)
			if (!InputManager.HasInputState ("ManualAddedSTATE1"))
								InputManager.MapStateToInput ("ManualAddedSTATE1", InputCode.Joystick0AxisX);// InputCode.W.SINGLE, InputCode.C.SINGLE);
			
			UnityEngine.Debug.Log("Log:" + InputManager.Log());
			
			
			////Event Based input handling
			InputEvent ev = new InputEvent("ManualAddedSTATE1");
			//InputEvent ev = new InputEvent((int)States.SomeState);
			
	
			ev.UP += new EventHandler(onUp);
			ev.DOWN += new EventHandler(onDown);
			
			_settingsLoaded = true;
			
			
			
		}
		
		
		// Update is called once per frame
		void Update()
		{
			
			
			
			//Use is mapping states so no quering keys during gameplay
			if (InputManager.EditMode || !_settingsLoaded) return;


			
			if (InputManager.GetInputDown ((int)States.Wave)) {
				GameObject.Find("Dude").GetComponent<Animator>().Play("Wave");
			}
			
			//	
			//
			if (InputManager.GetInputHold (Animator.StringToHash ("WalkBackward"))) {
				Debug.Log ("WalkBackward-Hold");
			}
			
			
			if (InputManager.GetInputDown (Animator.StringToHash ("WalkBackward"))) {
				Debug.Log ("WalkBackward-Down");
			}
			
			if (InputManager.GetInputUp (Animator.StringToHash ("WalkBackward"))) {
				Debug.Log ("WalkBackward-Up");
			}
			
			
			if (InputManager.GetInputHold (Animator.StringToHash ("WalkForward"))) {
				Debug.Log ("WalkForward-Hold");
			}
			
			if (InputManager.GetInputDown (Animator.StringToHash ("WalkForward"))) {
				Debug.Log ("WalkForward-Down");
			}
			
			if (InputManager.GetInputUp (Animator.StringToHash ("WalkForward"))) {
				Debug.Log ("WalkForward-Up");
			}
			//
			////
			
			//Bind Axis as one part
			
			//						InputManager.MapStateToInput ("WalkForward", KeyCodeExtension.W.SINGLE);
			//						InputManager.MapStateToInput ("WalkForward", 1, KeyCodeExtension.Joystick1AxisXPositive.SINGLE);
			//
			//
			//						InputManager.MapStateToInput ("WalkBackward", KeyCodeExtension.S.SINGLE);
			//						InputManager.MapStateToInput ("WalkBackward", 1, KeyCodeExtension.Joystick1AxisYNegative.SINGLE);
			//
			//						
			
			//			float axisPos = InputManager.GetInput (Animator.StringToHash ("WalkForward"), 0.3f, 0.1f, 0.2f);
			//
			//			float axisNeg= InputManager.GetInput (Animator.StringToHash ("WalkBackward"),  0.3f, 0.1f, 0.1f);
			//
			//			float analogVal=axisPos - axisNeg;
			
			//Debug.Log (analogVal);//would go from  -1 to 1
			
			
			
			
			// Hardware normalized value in range of -1f to 1f (keys,mouse would return 0f or 1f, triggers 0f to 1f)
			//float analogVal2= InputManager.GetInput (Animator.StringToHash ("WalkBackward"));
			//Debug.Log (analogVal2);
			
			
//			float analogVal2= InputManager.GetInput (Animator.StringToHash ("ManualAddedSTATE1"));
//			Debug.Log (analogVal2);
			
		}
		
		
		
		
	

		
		
		
		
		
		
		/// <summary>
		/// DONT FORGET TO CLEAN AFTER YOURSELF
		/// </summary>
		void OnDestroy()
		{
			InputManager.Dispose();
		}
	}
}

