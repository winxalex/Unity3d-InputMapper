
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
		public class WebDriverDemo : MonoBehaviour
		{			


				Animator animator = null;
				bool _settingsLoaded = false;

				// Use this for initialization
				void Start ()
				{
		
						if (Application.platform != RuntimePlatform.WindowsWebPlayer || Application.platform != RuntimePlatform.OSXWebPlayer) {
								Debug.LogError ("Set target in File>BuildSetting> WebPlayer");
								return;
						}
		
		
			
			
						//TODO think of better entry point
						InputManager.hidInterface.Enumerate ();
			
		
			

			
						#if(UNITY_WEBPLAYER || UNITY_EDITOR) && !UNITY_STANDALONE && !UNITY_ANDROID
			Loader request = new Loader();
			
			//UNITY_WEBPLAYER: Application.dataPath "http://localhost/appfolder/"
			request.Add(Application.dataPath+"/StreamingAssets/InputSettings.xml");
			
			
			request.LoadComplete += new EventHandler<LoaderEvtArgs<List<WWW>>>(onLoadComplete);
			request.Error += new EventHandler<LoaderEvtArgs<String>>(onLoadError);
			request.LoadItemComplete += new EventHandler<LoaderEvtArgs<WWW>>(onLoadItemComplete);
			request.load();
						#endif

			
			
			
			
			
			
				}
		
				void onUp (object o, EventArgs args)
				{
						Debug.Log ("Up");
				}
		
				void onDown (object o, EventArgs args)
				{
						Debug.Log ("Down");
				}
		
				void Handle1 (object o, EventArgs args)
				{
						Debug.Log ("Handle1");
				}
		
				void Handle2 (object o, EventArgs args)
				{
						Debug.Log ("Handle2");
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
		
		
		
		
				void manuallyAddStateAndHandlers ()
				{
			
	
			
						////easiest way to map state to combination (ex.of single W and C click)
						if (!InputManager.HasInputState ("ManualAddedSTATE"))
								InputManager.MapStateToInput ("ManualAddedSTATE", InputCode.W.SINGLE, InputCode.C.SINGLE);
			
						UnityEngine.Debug.Log ("Log:" + InputManager.Log ());
			
			
						////Event Based input handling
						InputEvent ev = new InputEvent ("ManualAddedSTATE");
						//InputEvent ev = new InputEvent((int)States.SomeState);
			
				
						ev.UP += new EventHandler (onUp);
						ev.DOWN += new EventHandler (onDown);
			
						_settingsLoaded = true;
			
			
			
				}
		
		
				// Update is called once per frame
				void Update ()
				{
			
			
			
						//Use is mapping states so no quering keys during gameplay
						if (InputManager.EditMode || !_settingsLoaded)
								return;
			
			
						//Input.GetInput allows combos (combined input actions)
						if (InputManager.GetInputDown ((int)States.Wave)) {// || InputManager.GetInput((int)States.Wave,true))
								// if (InputManager.GetInput((int)States.Wave,false))
								Debug.Log ("Wave Down");
								// animator.Play((int)States.Wave);
								animator.Play (Animator.StringToHash ("Wave"));
						}
			
			
						if (InputManager.GetInputUp ((int)States.MyCustomState)) {
								Debug.Log (States.MyCustomState + "-Up");
								// animator.Play((int)States.Wave);
						}
			
			

			
		
			
			
			
			
			
				}
		
		
		
		

		
		

		
		
		
		
		
		
		
				/// <summary>
				/// DONT FORGET TO CLEAN AFTER YOURSELF
				/// </summary>
				void OnDestroy ()
				{

						InputManager.Dispose ();
				}
		}
}
