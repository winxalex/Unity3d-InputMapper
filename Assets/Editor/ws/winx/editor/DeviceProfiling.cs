

using UnityEditor;
using UnityEngine;
using ws.winx.devices;
using System;
using System.IO;


namespace ws.winx.editor
{
		public class DeviceProfiling : EditorWindow
		{

				static DeviceProfiling _instance;

				// Add menu named "Input Mapper" to the Window menu
				[MenuItem ("Window/Input Mapper/DeviceProfiling")]
				static void ShowEditor ()
				{
					
					
					_instance = EditorWindow.GetWindow<DeviceProfiling>();

					
				}


				void OnGUI(){

		

				if (GUILayout.Button ("Create Profiles Asset")) {
						if(!Directory.Exists(Path.Combine(Application.dataPath,"DeviceProfiles")))
								AssetDatabase.CreateFolder ("Assets", "DeviceProfiles");
								
							AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<DeviceProfiles> (), "Assets/DeviceProfiles/DeviceProfiles.asset");
				}
			
			
			}
	
		
		}
}