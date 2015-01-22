

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
		public class DeviceProfilesEditor : Editor
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

								if (GUILayout.Button ("Remove") && !String.IsNullOrEmpty (_profileNameSelected)) {
					
										if (EditorUtility.DisplayDialog ("Remove the profile",
					                                 "Are you sure remove profile and all devices's map to it?", "Yes", "Cancel")) {
						
												if (__profiles.runtimePlatformDeviceProfileDict.ContainsKey (_profileNameSelected)) {
														__profiles.runtimePlatformDeviceProfileDict.Remove (_profileNameSelected);
														List<string> pidVidMappingsToBeRemoved = new List<string> ();
							
														foreach (var kvp in __profiles.vidpidProfileNameDict) {
																pidVidMappingsToBeRemoved.Add (kvp.Key);
														}
							
														foreach (var key in pidVidMappingsToBeRemoved) {
																__profiles.vidpidProfileNameDict.Remove (key);
														}

                                                        _profileNameSelected = String.Empty;
                                                        _profileIndexSelected = 0;
												}
										}
					
								}

						}
				

						EditorGUILayout.Separator ();

						/////////// DEVICES //////////

						if (!String.IsNullOrEmpty (_profileNameSelected)) {
								EditorGUILayout.BeginHorizontal ();//,
								EditorGUILayout.LabelField ("Device PID#VID or Name(UnityDriver)", new GUILayoutOption[]{GUILayout.Width (130)});
								_pidVidKey = EditorGUILayout.TextField (_pidVidKey);
					
				
						

								if (GUILayout.Button ("Assign to Profile") && !String.IsNullOrEmpty (_pidVidKey)) {
										__profiles.vidpidProfileNameDict [_pidVidKey] = _profileNameSelected;
										EditorUtility.SetDirty (__profiles);
										AssetDatabase.SaveAssets ();
										_pidVidKey=String.Empty;
										this.Repaint();
								}
								
								if (GUILayout.Button ("Remove") && !String.IsNullOrEmpty (_pidVidKey)) {

										if (__profiles.vidpidProfileNameDict.ContainsKey (_pidVidKey)) {
												__profiles.vidpidProfileNameDict.Remove (_pidVidKey);
												EditorUtility.SetDirty (__profiles);
												AssetDatabase.SaveAssets ();
										}
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