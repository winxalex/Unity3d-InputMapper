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
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(UserInterfaceWindow))]
public class InputComponent : MonoBehaviour
{

		

		//
		// Fields
		//



		public TextAsset profileList;

		public TextAsset[] profiles;

		public string settingsFileName="InputSettings.xml";

		

		[FormerlySerializedAs ("onLoad"), UnityEngine.SerializeField]
		private InputComponentEvent m_onLoad = new InputComponentEvent ();


		//
		// Nested Types
		//
		[Serializable]
		public class InputComponentEvent : UnityEvent
		{
		}
		
	
	//
	// Properties
	//
	public InputComponentEvent onLoad
	{
		get
		{
			return this.m_onLoad;
		}
		set
		{
			this.m_onLoad = value;
		}
	}


		UserInterfaceWindow ui;

		// Use this for initialization
		void Start ()
		{


				   if (String.IsNullOrEmpty (settingsFileName)) {

						Debug.LogError("Add Settings FileName from StreamingAssets");
						return;
					}


		ui= this.GetComponent<UserInterfaceWindow>();

		//supporting devices with custom drivers
		//When you add them add specialized first then XInputDriver  then wide range supporting drivers like WinMM or OSXDriver
		//supporting devices with custom drivers
		//When you add them add specialized first then XInputDriver  then wide range supporting drivers WinMM or OSXDriver
		#if (UNITY_STANDALONE_WIN)
		InputManager.AddDriver(new ThrustMasterDriver());
		//InputManager.AddDriver(new WiiDriver());
		InputManager.AddDriver(new XInputDriver());
		//change default driver
		//InputManager.hidInterface.defaultDriver=new UnityDriver();
		#endif
		#if (UNITY_STANDALONE_OSX)
		InputManager.AddDriver(new ThrustMasterDriver());
		InputManager.AddDriver(new XInputDriver());
		//change default driver
		//InputManager.hidInterface.defaultDriver=new UnityDriver();
		
		#endif
		
		#if (UNITY_STANDALONE_ANDROID)
		InputManager.AddDriver(new ThrustMasterDriver());
		InputManager.AddDriver(new XInputDriver());
		#endif
		
		

		InputManager.hidInterface.Enumerate();
		
		
		

		
		//if you want to load some states from .xml and add custom manually first load settings xml
		//!!!Application.streamingAssetPath gives "Raw" folder in web player
		
		#if (UNITY_STANDALONE || UNITY_EDITOR ) && !UNITY_WEBPLAYER && !UNITY_ANDROID
		//UnityEngine.Debug.Log("Standalone");

		
		
		if (ui != null && !String.IsNullOrEmpty (settingsFileName))
		{//settingsXML would trigger internal loading mechanism (only for testing)

			//load settings from external file
			ui.settings=InputManager.loadSettings(Path.Combine(Application.streamingAssetsPath, settingsFileName));

			//ui.settings=InputManager.loadSettingsFromText(settingsXML.text);



	




			//dispatch Event 
			this.m_onLoad.Invoke();

		}
		
		

		
		#endif
		
		#region Load InputSettings.xml Android
		#if UNITY_ANDROID

		
		
		Loader request = new Loader();
		
		
		if (Application.platform == RuntimePlatform.Android)
		{
			if (File.Exists(Application.persistentDataPath + "/" + settingsXML.name+".xml"))
			{
				
				if (ui != null)
				{
					Debug.Log("Game>> Try to load from " + Application.persistentDataPath);
					InputManager.loadSettings(Application.persistentDataPath + "/" + settingsXML.name+".xml");
					ui.settings = InputManager.Settings;
					manuallyAddStateAndHandlers();
					return;
					
				}
			}
			else
			{// content of StreamingAssets get packed inside .APK and need to be load with WWW
				request.Add(Path.Combine(Application.streamingAssetsPath, "InputSettings.xml"));
				request.Add(Path.Combine(Application.streamingAssetsPath, "profiles.txt"));
				request.Add(Path.Combine(Application.streamingAssetsPath, "xbox360_drd.txt"));
				//....

				//unpack everything in presistentDataPath
			}
			
			
			request.LoadComplete += new EventHandler<LoaderEvtArgs<List<WWW>>>(onLoadComplete);
			request.Error += new EventHandler<LoaderEvtArgs<String>>(onLoadError);
			request.LoadItemComplete += new EventHandler<LoaderEvtArgs<WWW>>(onLoadItemComplete);
			request.load();
		}
		else //TARGET=ANDROID but playing in EDITOR => use Standalone setup
		{
			if (ui != nulll)
			{//settingsXML would trigger internal loading mechanism (only for testing)
				
				InputManager.loadSettings(Path.Combine(Application.streamingAssetsPath, settingsXML.name+".xml"));
				
				
				
				ui.settings = InputManager.Settings;
			}
			
			

			
		}
		
		
		
		#endif
		#endregion
		
		#if(UNITY_WEBPLAYER || UNITY_EDITOR) && !UNITY_STANDALONE && !UNITY_ANDROID
		Loader request = new Loader();
		
		//UNITY_WEBPLAYER: Application.dataPath "http://localhost/appfolder/"
		request.Add(Application.dataPath+"/StreamingAssets/"+settingsXML.name+".xml");
		
		
		request.LoadComplete += new EventHandler<LoaderEvtArgs<List<WWW>>>(onLoadComplete);
		request.Error += new EventHandler<LoaderEvtArgs<String>>(onLoadError);
		request.LoadItemComplete += new EventHandler<LoaderEvtArgs<WWW>>(onLoadItemComplete);
		request.load();
		#endif
		}



			#if (UNITY_WEBPLAYER || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_STANDALONE
			void onLoadComplete(object sender, LoaderEvtArgs<List<WWW>> args)
			{
				// Debug.Log(((List<WWW>)args.data).ElementAt(0).text);
				
				if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1) return;
				
				
				//UnityEngine.Debug.Log("WebPlayer " + Path.Combine(Path.Combine(Application.dataPath, "StreamingAssets"), "InputSettings.xml"));
				
				
				
				
				
				
				if (ui != null)//without settingsXML defined =>load them manually and attach them
				{
					InputManager.loadSettingsFromText(args.data.ElementAt(0).text);
					ui.settings = InputManager.Settings;
				}
				
				
			   //dispatch load complete
				
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

	
		// Update is called once per frame
		void Update ()
		{
			InputManager.dispatchEvent();
		}


		void OnGUI(){

			if (ui != null && ui.settings != null && GUI.Button (new Rect (0, 0, 100, 30), "Settings"))
								ui.enabled = !ui.enabled;



		}
}

