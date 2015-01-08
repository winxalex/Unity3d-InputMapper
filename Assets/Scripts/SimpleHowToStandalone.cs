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
		

		
		
	
		UserInterfaceWindow ui;
	
		
		// Use this for initialization
		void Start()
		{




			


			
		//
		

			
			//supporting devices with custom drivers
			//When you add them add specialized first then XInputDriver  then wide range supporting drivers UnityDriver
			#if (UNITY_STANDALONE_WIN)
            //			InputManager.AddDriver(new ThrustMasterDriver());
            //			InputManager.AddDriver(new WiiDriver());
            			InputManager.AddDriver(new XInputDriver());
			//change default driver
		//InputManager.hidInterface.defaultDriver=new UnityDriver();

//			
			#endif
//			
			#if (UNITY_STANDALONE_OSX)
           			InputManager.AddDriver(new ThrustMasterDriver());
           			InputManager.AddDriver(new XInputDriver());
			//change default driver
			//InputManager.hidInterface.defaultDriver=new UnityDriver();

			#endif
	
		
			InputManager.hidInterface.Enumerate();
				
			
			//if you want to load some states from .xml and add custom manually => first load settings xml then add
			//!!!Application.streamingAssetPath gives "Raw" folder in web player
			
			#if (UNITY_STANDALONE || UNITY_EDITOR ) && !UNITY_WEBPLAYER && !UNITY_ANDROID

			 ui= this.GetComponent<UserInterfaceWindow>();

			
			
			if (ui != null && ui.settingsXML == null)
			{//settingsXML would trigger internal loading mechanism (only for testing)
				
				InputManager.loadSettings(Path.Combine(Application.streamingAssetsPath, "InputSettings.xml"));
				
				
				
				ui.settings = InputManager.Settings;

			}
			
			
			
			
			#endif
			

			
	
			
		}
		
		
	
		

		
	
		void OnGUI(){

			if (ui != null && ui.settings != null && GUI.Button (new Rect (0, 0, 100, 30), "Settings"))
								ui.enabled = !ui.enabled;



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

