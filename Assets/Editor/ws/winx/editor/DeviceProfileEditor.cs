

using UnityEditor;
using UnityEngine;
using ws.winx.devices;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ws.winx.editor
{
		[CustomEditor(typeof(DeviceProfiles))]
		[CanEditMultipleObjects]
		public class DeviceProfileEditor : Editor
		{
				private DeviceProfiles __profiles; //Make an easy shortcut to the Dialogue your editing
				

				string _profileName;
				string _pidVidKey;
				string _profileNameSelected;
				int _profileIndexSelected;
				string[] _displayOptions;
				RuntimePlatform _platformSelected;
			

				void Awake ()
				{
						__profiles = (DeviceProfiles)target;
				}

				void OnEnable ()
				{
						// Setup the SerializedProperties


						//_currentProfile = serializedObject.FindProperty ("currentProfile");

				}
		
				public override void OnInspectorGUI ()
				{



						
						/////////// PROFILES //////////
								
						EditorGUILayout.BeginHorizontal ();
						_profileName = EditorGUILayout.TextField ("Profile Name", _profileName);

						if (GUILayout.Button ("Add") && !String.IsNullOrEmpty (_profileName)) {
								__profiles.runtimePlatformDeviceProfileDict [_profileName] = new Dictionary<RuntimePlatform, DeviceProfile> ();
								EditorUtility.SetDirty (__profiles);
								AssetDatabase.SaveAssets ();

								_profileName = String.Empty;
						}
						EditorGUILayout.EndHorizontal ();


						EditorGUILayout.Separator ();




						//////////////// SELECT PROFILE ///////
						/// 
						_displayOptions = __profiles.runtimePlatformDeviceProfileDict.Keys.ToArray ();
						if (_displayOptions.Length > 0) {
				
								_profileIndexSelected = EditorGUILayout.Popup ("Profiles:", _profileIndexSelected, _displayOptions);
								_profileNameSelected = _displayOptions [_profileIndexSelected];

						}
				

							EditorGUILayout.Separator ();

						/////////// DEVICES //////////
						if (!String.IsNullOrEmpty (_profileNameSelected)) {
								EditorGUILayout.BeginHorizontal ();//,
								EditorGUILayout.LabelField ("Device PID#VID", new GUILayoutOption[]{GUILayout.Width (100)});
								_pidVidKey = EditorGUILayout.TextField (_pidVidKey);
					
				
						

								if (GUILayout.Button ("Assign to Profile") && !String.IsNullOrEmpty (_pidVidKey)) {
										__profiles.pidvidShortTypeNames [_pidVidKey] = _profileNameSelected;
										EditorUtility.SetDirty (__profiles);
										AssetDatabase.SaveAssets ();
								}

								EditorGUILayout.EndHorizontal ();
						}
						


						/////////// PLATFORM //////////
				
						EditorGUILayout.BeginHorizontal ();
			

						_platformSelected = (RuntimePlatform)EditorGUILayout.EnumPopup ("Platform", _platformSelected);

						DeviceProfile profile;

						if (!String.IsNullOrEmpty (_profileNameSelected)) {

								//profile=__profiles.runtimePlatformDeviceProfileDict[_profileNameSelected][_platformSelected];

								if (__profiles.runtimePlatformDeviceProfileDict [_profileNameSelected].ContainsKey (_platformSelected)) {
										profile = __profiles.runtimePlatformDeviceProfileDict [_profileNameSelected] [_platformSelected];

										__profiles.currentProfile = profile;

								} else {
										__profiles.currentProfile = null;

					

										if (GUILayout.Button ("Add Settings")) {
												__profiles.currentProfile = new DeviceProfile ();
												__profiles.currentProfile.Name = _profileNameSelected;
												__profiles.runtimePlatformDeviceProfileDict [_profileNameSelected] [_platformSelected] = __profiles.currentProfile;
												EditorUtility.SetDirty (__profiles);
												AssetDatabase.SaveAssets ();
						
										}
								}
					
				

				


								//__profiles.runtimePlatformDeviceProfileDict [_profileNameSelected].
								//EditorUtility.SetDirty (__profiles);
								//AssetDatabase.SaveAssets ();
						}

						EditorGUILayout.EndHorizontal ();


						//_currentProfile.
						//EditorGUILayout.PropertyField(_currentProfile);

			
						if (__profiles.currentProfile != null)
								this.DrawDefaultInspector ();

						if (GUILayout.Button ("Save")) {
								EditorUtility.SetDirty (__profiles);
								AssetDatabase.SaveAssets ();
				
						}
			
						// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
						//serializedObject.ApplyModifiedProperties ();
				}
		}
}