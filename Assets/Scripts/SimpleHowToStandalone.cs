using UnityEngine;
using System;
using ws.winx.unity;
using ws.winx.gui;
using ws.winx.input;
using ws.winx.input.states;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace ws.winx
{

	/// <summary>
	/// Simple how to.
	/// !!! USE THIS SAMPLE WHEN U TARGETING OSX OR WIN (not with ANDROID Or WEB)
	/// </summary>
	public class SimpleHowToStandalone : MonoBehaviour
	{
		

		
		Animator animator = null;
		bool _settingsLoaded = false;
	
		
		// Use this for initialization
		void Start()
		{

			
			animator = GameObject.FindObjectOfType<Animator>();
		
		

			
			//supporting devices with custom drivers
			//When you add them add specialized first then XInputDriver  then wide range supporting drivers UnityDriver
//			#if (UNITY_STANDALONE_WIN)
//			InputManager.AddDriver(new ThrustMasterDriver());
//			InputManager.AddDriver(new WiiDriver());
//			InputManager.AddDriver(new XInputDriver());
//			
//			#endif
//			
//			#if (UNITY_STANDALONE_OSX)
//			InputManager.AddDriver(new ThrustMasterDriver());
//			InputManager.AddDriver(new XInputDriver());
//			#endif
	
			//TODO think of better entry point
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
			if (!InputManager.HasInputState("ManualAddedSTATE"))
				InputManager.MapStateToInput("ManualAddedSTATE", KeyCodeExtension.W.SINGLE, KeyCodeExtension.C.SINGLE);
			
			UnityEngine.Debug.Log("Log:" + InputManager.Log());
			
			
			////Event Based input handling
			InputEvent ev = new InputEvent("ManualAddedSTATE");
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
			
			
			//Input.GetInput allows combos (combined input actions)
			if (InputManager.GetInputDown((int)States.Wave))// || InputManager.GetInput((int)States.Wave,true))
				// if (InputManager.GetInput((int)States.Wave,false))
			{
				Debug.Log("Wave Down");
				// animator.Play((int)States.Wave);
				animator.Play(Animator.StringToHash("Wave"));
			}
			
			
			if (InputManager.GetInputUp((int)States.MyCustomState))
			{
				Debug.Log(States.MyCustomState + "-Up");
				// animator.Play((int)States.Wave);
			}
			
			
		
			

			////
			//          //using input as analog value
			//			float analogValue=InputManager.GetInput((int)States.Walk_Forward,false,0.3f,0.1f,0f);
			//			analogValue-=InputManager.GetInput((int)States.Base_Layer_MyState,false,0.3f,0.1f,0f);
			//
			////			Debug.Log(analogValue);
			
			
			
			
			
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

