using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using ws.winx.devices;
using ws.winx.drivers;
using ws.winx.gui;
using ws.winx.input;
using ws.winx.input.states;
using ws.winx.platform;
using ws.winx.unity;
using ws.winx.utils;


namespace ws.winx
{

	/// <summary>
	/// Custom driver how to.
	/// ThrustmasterRGTFFDDevice with custom driver ThrustMasterDriver.cs tested on WIN,ANDROID,OSX
	/// </summary>
	public class CustomDriverHowTo : MonoBehaviour
	{
			
	
		bool _settingsLoaded = false;
		private float vSliderValue;
		private ThrustmasterRGTFFDDevice TTFFDDevice;
		IEnumerator runEffectEnumerator;
		private byte forceX;
		private Timer timer;
		private float vSliderValuePrev;
	
	
		
		
		// Use this for initialization
		void Start()
		{

			
		
			
			vSliderValuePrev = vSliderValue = 128f;
			
			timer = new Timer(500.0);
			timer.Elapsed += new ElapsedEventHandler(onTimerElapsed);
			

			
			//supporting devices with custom drivers

			#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_ANDROID)
					InputManager.AddDriver(new ThrustMasterDriver());

			#endif
			
			

			InputManager.hidInterface.Enumerate();
			


			//if you want to load some states from .xml and add custom manually, first load settings xml
			//!!!Application.streamingAssetPath gives "Raw" folder in web player
			
			#if (UNITY_STANDALONE || UNITY_EDITOR ) && !UNITY_WEBPLAYER && !UNITY_ANDROID
			//UnityEngine.Debug.Log("Standalone");
			UserInterfaceWindow ui = this.GetComponent<UserInterfaceWindow>();

			
			if (ui != null && ui.settingsXML == null)
			{//settingsXML would trigger internal loading mechanism (only for testing)
				
				InputManager.loadSettings(Path.Combine(Application.streamingAssetsPath, "InputSettings.xml"));
				
				
				
				ui.StateInputCombinations = InputManager.Settings.stateInputs;
			}
			
			
			manuallyAddStateAndHandlers();
			
			#endif
			
			#region Load InputSettings.xml Android
			#if UNITY_ANDROID
			
			UserInterfaceWindow ui = this.GetComponent<UserInterfaceWindow>();

			Loader request = new Loader();
			
			
			if (Application.platform == RuntimePlatform.Android)
			{
				if (File.Exists(Application.persistentDataPath + "/" + "InputSettings.xml"))
				{
					
					if (ui != null)
					{
						Debug.Log("Game>> Try to load from " + Application.persistentDataPath);
						InputManager.loadSettings(Application.persistentDataPath + "/" + "InputSettings.xml");
						ui.StateInputCombinations = InputManager.Settings.stateInputs;
						manuallyAddStateAndHandlers();
						return;
						
					}
				}
				else
				{// content of StreamingAssets get packed inside .APK and need to be load with WWW
					request.Add(Path.Combine(Application.streamingAssetsPath, "InputSettings.xml"));
				}
				
				
				request.LoadComplete += new EventHandler<LoaderEvtArgs<List<WWW>>>(onLoadComplete);
				request.Error += new EventHandler<LoaderEvtArgs<String>>(onLoadError);
				request.LoadItemComplete += new EventHandler<LoaderEvtArgs<WWW>>(onLoadItemComplete);
				request.load();
			}
			else //TARGET=ANDROID but playing in EDITOR => use Standalone setup
			{
				if (ui != null && ui.settingsXML == null)
				{//settingsXML would trigger internal loading mechanism (only for testing)
					
					InputManager.loadSettings(Path.Combine(Application.streamingAssetsPath, "InputSettings.xml"));
					
					
					
					ui.StateInputCombinations = InputManager.Settings.stateInputs;
				}
				
				
				manuallyAddStateAndHandlers();
				
			}
			
			
			
			#endif
			#endregion
			

			
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
		
		#if (UNITY_WEBPLAYER || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_STANDALONE
		void onLoadComplete(object sender, LoaderEvtArgs<List<WWW>> args)
		{
			// Debug.Log(((List<WWW>)args.data).ElementAt(0).text);
			
			if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1) return;
			
			
			//UnityEngine.Debug.Log("WebPlayer " + Path.Combine(Path.Combine(Application.dataPath, "StreamingAssets"), "InputSettings.xml"));
			
			
			
			UserInterfaceWindow ui=this.GetComponent<UserInterfaceWindow>();
			
			
			if (ui != null)//without settingsXML defined =>load them manually and attach them
			{
				InputManager.loadSettingsFromText(args.data.ElementAt(0).text);
				ui.StateInputCombinations = InputManager.Settings.stateInputs;
			}
			
			
			manuallyAddStateAndHandlers();
			
		}
		
		void onLoadItemComplete(object sender, LoaderEvtArgs<WWW> args)
		{
			// Debug.Log(args.data.text);
		}
		
		
		void onLoadError(object sender, LoaderEvtArgs<String> args)
		{
			Debug.Log(args.data);
		}
		#endif
		
		
		
		
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
				InputManager.MapStateToInput("ManualAddedSTATE", InputCode.W.SINGLE, InputCode.C.SINGLE);
			
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
			
			
			
			
			
		}
		
		
		
		
		void OnGUI()
		{
			

			
			
			//don't take device here in the loop this is just for demo
			
			if (InputManager.Devices.ContainsIndex(0))
				TTFFDDevice = InputManager.Devices.GetDeviceAt(0) as ThrustmasterRGTFFDDevice;
			
			
			
			
			
			if (TTFFDDevice == null) return;
			
			//#if UNITY_ANDROID
			
			vSliderValue = GUI.HorizontalSlider(new Rect(25, 520, 400, 100), vSliderValue, 255.0F, 0.0F);
			// #endif
			
			if (vSliderValue != vSliderValuePrev)
				// device.SetMotor(Convert.ToByte(vSliderValue), 0xA7, onMotorSet);
				TTFFDDevice.SetMotor(Convert.ToByte(vSliderValue), Convert.ToByte(vSliderValue), onMotorSet);
			
			vSliderValuePrev = vSliderValue;
			
			
			
			if (GUI.Button(new Rect(25, 590, 100, 130), "Stop Motor"))
			{
				//timer.Stop();
				if(runEffectEnumerator!=null)
					StopCoroutine(runEffectEnumerator);
				TTFFDDevice.StopMotor(onMotorStop);
				vSliderValue = 128;
			}
			
			if (GUI.Button(new Rect(150, 590, 100, 130), "Rumble"))
			{
				
				runEffectEnumerator = runEffect();
				
				TTFFDDevice.StopMotor(onMotorStop);
				
				StartCoroutine(runEffectEnumerator);
				
				
				//char buf[] = {0x00, 0x01, 0x0f, 0xc0, 0x00, large, small, 0x00, 0x00, 0x00, 0x00, 0x00};
				
			}
		}
		
		
		void onMotorStop(bool success)
		{
			Debug.Log("Motor stop was successful:" + success);
		}
		
		void onMotorSet(bool success)
		{
			Debug.Log("Motor command was successful:" + success);
		}
		
		IEnumerator runEffect()
		{
			while (true)
			{
				forceX += 0xA7;
				TTFFDDevice.SetMotor(forceX, forceX, onMotorSet);
				
				yield return new WaitForSeconds(0.5f);
			}
			
			// yield break;
			
		}
		
		void onTimerElapsed(object sender, ElapsedEventArgs args)
		{
			forceX += 0xA7;
			TTFFDDevice.SetMotor(forceX, forceX, onMotorSet);
		}
		
		
		
		
		
		
		
		/// <summary>
		/// DONT FORGET TO CLEAN AFTER YOURSELF
		/// </summary>
		void OnDestroy()
		{
			if (TTFFDDevice != null)
				TTFFDDevice.StopMotor();
			InputManager.Dispose();
		}
	}
}
