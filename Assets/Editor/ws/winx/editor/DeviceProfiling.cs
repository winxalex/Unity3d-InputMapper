

using UnityEditor;
using UnityEngine;
using ws.winx.devices;
using System;
using System.IO;
using ws.winx.input;
using System.Collections.Generic;
using System.Linq;

namespace ws.winx.editor
{
		public class DeviceProfiling : EditorWindow
		{

				
				static DeviceProfiling _instance;
				 RuntimePlatform _platform;
				 bool _nameGiveEdit;
				static bool __wereDevicesEnumerated = false;
	

				//Device
				 int _deviceDisplayIndex;
				 string[] _displayOptions;
				 IDevice _deviceSelected;
				



				//profiles
				 static DeviceProfiles _profiles;
				 int _profileIndexSelected;
				 string _profileName;
				 string _axisButtonName;
				 string _nameGiven;
				 string _profileNameSelected;
				 InputAction _actionSelected;
                 int _deviceCount;

				// Add menu named "Input Mapper" to the Window menu
				[MenuItem ("Window/Input Mapper/DeviceProfiling")]
				static void ShowEditor ()
				{

						// Get existing open window or if none, make a new one:
						if (_instance == null)
						if (!Application.isPlaying && _profiles != null) {
								InputManager.hidInterface.SetProfiles (_profiles);
								InputManager.hidInterface.Enumerate ();
								__wereDevicesEnumerated = true;
								
						}

						_instance = EditorWindow.GetWindow<DeviceProfiling> ();
				}



				/// <summary>
				/// Update this instance.
				/// </summary>
				void Update ()
				{



						if (!Application.isPlaying && _profiles != null && !__wereDevicesEnumerated) {
								InputManager.hidInterface.SetProfiles (_profiles);
								InputManager.hidInterface.Enumerate ();
								__wereDevicesEnumerated = true;
							
						}

						InputAction actionCurrent;

						actionCurrent = InputManager.GetAction (_deviceSelected);
							
							
						if (actionCurrent != null && (actionCurrent.getCode (_deviceSelected) ^ (int)KeyCode.Escape) != 0 && (actionCurrent.getCode (_deviceSelected) ^ (int)KeyCode.Return) != 0) {
								
								
								
								if ((actionCurrent.getCode (_deviceSelected) ^ (int)KeyCode.Backspace) == 0) {
										_nameGiven = String.Empty;
									
								} else {
									
							
									
										actionCurrent.type = InputActionType.SINGLE;
									
										actionCurrent.setCode (InputCode.toCodeAnyDevice (actionCurrent.getCode (_deviceSelected)), null);
									
									
								
										actionCurrent.setCode (InputCode.toCodeAxisFull (actionCurrent.getCode (_deviceSelected)), null);
										
									
									
										_actionSelected = actionCurrent.Clone ();

										//Debug.Log ("Action:" + _actionSelected + " " + _actionSelected.getCode (_deviceSelected) + " type:" + _actionSelected.type);
										this.Repaint ();
										
								}
								
								
								
								//Debug.Log ("Action:" + _action + " " + _action.getCode(_deviceByProfile)+" type:"+_action.type);
						}


				}



				/// <summary>
				/// onGUI
				/// </summary>
				/////////////////    onGUI   ////////////
				void OnGUI ()
				{



						///////////////      DEVICES      ///////////////
						if (_profiles != null) {
								List<IDevice> devicesList = InputManager.GetDevices<IDevice> ();

                               

								if (devicesList.Count > 0) {
										_displayOptions = devicesList.Select (item => item.Name).ToArray ();

										_deviceDisplayIndex = EditorGUILayout.Popup ("Devices:", _deviceDisplayIndex, _displayOptions);

										_deviceSelected = devicesList [_deviceDisplayIndex];


                                        if (!String.IsNullOrEmpty(_profileNameSelected))
                                        {

                                            EditorGUILayout.BeginHorizontal();
                                            if (GUILayout.Button("Assign Profile"))
                                            {

                                                string pidVidKey = _deviceSelected.VID.ToString("X4") + "#" + _deviceSelected.PID.ToString("X4");
                                                _profiles.vidpidProfileNameDict[pidVidKey] = _profileNameSelected;

                                                EditorUtility.SetDirty(_profiles);
                                                AssetDatabase.SaveAssets();
                                            }

                                            if (GUILayout.Button("Remove From Profile"))
                                            {

                                                string pidVidKey = _deviceSelected.VID.ToString("X4") + "#" + _deviceSelected.PID.ToString("X4");

                                                _profiles.vidpidProfileNameDict.Remove(pidVidKey);

                                                EditorUtility.SetDirty(_profiles);
                                                AssetDatabase.SaveAssets();

                                            }

                                            EditorGUILayout.EndHorizontal();

                                        }

                                      
								} else {

										EditorGUILayout.LabelField ("Devices: No attached devices");
								}



                                if (_deviceCount != devicesList.Count)
                                    this.Repaint();

                                _deviceCount = devicesList.Count;
						}



                     


//				if(GUILayout.Button("Proifle2DeviceProfile")){
//						char splitChar = '|';
//
//						string fileBase;
//
//						foreach(var kvp in _profiles.runtimePlatformDeviceProfileDict){
//
//						fileBase=kvp.Key;
//
//						DeviceProfile profile;
//
//						profile=kvp.Value[RuntimePlatform.WindowsPlayer]=new DeviceProfile();
//						profile.Name=kvp.Key;
//						
//						using (StreamReader reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, fileBase + "_win.txt")))
//						{
//							
//							if (!reader.EndOfStream)
//								profile.buttonNaming = reader.ReadLine().Split(splitChar);
//							
//							if (!reader.EndOfStream)
//								profile.axisNaming = reader.ReadLine().Split(splitChar);
//							
//							//rest in future
//							
//							
//							
//						}
//
//						profile=kvp.Value[RuntimePlatform.OSXPlayer]=new DeviceProfile();
//						profile.Name=kvp.Key;
//
//						using (StreamReader reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, fileBase + "_osx.txt")))
//						{
//							
//							if (!reader.EndOfStream)
//								profile.buttonNaming = reader.ReadLine().Split(splitChar);
//							
//							if (!reader.EndOfStream)
//								profile.axisNaming = reader.ReadLine().Split(splitChar);
//							
//							//rest in future
//							
//							
//							
//						}
//
//
//					}
//
//					EditorUtility.SetDirty (_profiles);
//					AssetDatabase.SaveAssets ();
//
//				}


//								if (GUILayout.Button ("Profiles.txt2DeviceProfiles.asset")) {
//										string[] deviceNameProfilePair;
//										char splitChar = '|';
//
//										using (StreamReader reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "profiles.txt"))) {
//					
//					
//												while (!reader.EndOfStream) {
//						
//														deviceNameProfilePair = reader.ReadLine ().Split (splitChar);
//														if (!_profiles.vidpidProfileNameDict.ContainsKey (deviceNameProfilePair [0]))
//																_profiles.vidpidProfileNameDict [deviceNameProfilePair [0]] = deviceNameProfilePair [1];
//
//														if (!_profiles.runtimePlatformDeviceProfileDict.ContainsKey (deviceNameProfilePair [1])) {
//
//																_profiles.runtimePlatformDeviceProfileDict [deviceNameProfilePair [1]] = new Dictionary<RuntimePlatform, DeviceProfile> ();
//														}
//												}
//
//												EditorUtility.SetDirty (_profiles);
//												AssetDatabase.SaveAssets ();
//					
//										}
//								}

							
						
						

						EditorGUILayout.Separator ();



						///////////////  PROFILES /////////////

						_profiles = EditorGUILayout.ObjectField (_profiles, typeof(DeviceProfiles), false) as DeviceProfiles;

						if (_deviceSelected != null && _profiles != null) {
								

								_displayOptions = _profiles.runtimePlatformDeviceProfileDict.Keys.ToArray ();

								if (_displayOptions.Length > 0) {
										_profileIndexSelected = EditorGUILayout.Popup ("Profiles:", _profileIndexSelected, _displayOptions);
										_profileNameSelected = _displayOptions [_profileIndexSelected];
								}

								EditorGUILayout.BeginHorizontal ();

								_profileName = EditorGUILayout.TextField ("Name", _profileName);

								if (GUILayout.Button ("Add profile") && !String.IsNullOrEmpty (_profileName)) {
										

										if (!_profiles.runtimePlatformDeviceProfileDict.ContainsKey (_profileName)) {

											
												_profiles.runtimePlatformDeviceProfileDict [_profileName] = new Dictionary<RuntimePlatform, DeviceProfile> ();
										} else {

												Debug.LogWarning ("Already contain such profile");
										}

										_profileName = String.Empty;

										EditorUtility.SetDirty (_profiles);
										AssetDatabase.SaveAssets ();

								}


								


								EditorGUILayout.EndHorizontal ();

								EditorGUILayout.Separator ();
				

								///////////////// NAMING(buttons,axis..) //////////////
								string actionCodeString = "Click button or Move stick";
								DeviceProfile profile = null;
								JoystickAxis axis = JoystickAxis.None;
								string nameGivenCurrent = "No Name";
			
								if (!String.IsNullOrEmpty (_profileNameSelected)) {
										///// CURRENT ACTION /////
										/// 
										/// 
										EditorGUILayout.BeginHorizontal ();
										EditorGUILayout.LabelField ("ACTION:");

										

										if (_actionSelected != null) {


												actionCodeString = _actionSelected.codeString;
												//find axis button number
												axis = InputCode.toAxis (_actionSelected.getCode (_deviceSelected));

												_platform = Application.platform == RuntimePlatform.OSXEditor ? RuntimePlatform.OSXPlayer : RuntimePlatform.WindowsPlayer;
											
											
											
											
												if (_profiles.runtimePlatformDeviceProfileDict [_profileNameSelected].ContainsKey (_platform)) {
														profile = _profiles.runtimePlatformDeviceProfileDict [_profileNameSelected] [_platform];
												
												
												
												
														if (axis == JoystickAxis.None) {
																nameGivenCurrent = profile.buttonNaming [InputCode.toData (_actionSelected.getCode (_deviceSelected))];
														} else {
																nameGivenCurrent = profile.axisNaming [(int)axis];
														}
												}
										}

										EditorGUILayout.LabelField (actionCodeString);

										EditorGUILayout.EndHorizontal ();

										EditorGUILayout.Separator ();

										///// GIVE NAME ////			
										EditorGUILayout.BeginHorizontal ();



                                        if (String.IsNullOrEmpty(nameGivenCurrent))
                                        {
                                            nameGivenCurrent = "No Name [Click to Edit]";
                                            _nameGiven = String.Empty;
                                        }
                                        else
                                            _nameGiven = nameGivenCurrent;

										if (_actionSelected != null)
										if (!_nameGiveEdit
												&& GUILayout.Button (nameGivenCurrent))
												_nameGiveEdit = true;


										if (_nameGiveEdit) {
												_nameGiven = EditorGUILayout.TextField ("Name", _nameGiven);
				

									

												if (GUILayout.Button ("Give") || (Event.current.isKey && Event.current.keyCode == KeyCode.Return))
												if (!String.IsNullOrEmpty (_profileNameSelected) && !String.IsNullOrEmpty (_nameGiven)) {


									
							
									
														if (profile == null) {
																profile = new DeviceProfile ();
																profile.Name = _profileNameSelected;
																_profiles.runtimePlatformDeviceProfileDict [_profileNameSelected] [_platform] = profile;
										
														}

														if (axis == JoystickAxis.None) {
																profile.buttonNaming [InputCode.toData (_actionSelected.getCode (_deviceSelected))] = _nameGiven;
														} else {
																profile.axisNaming [(int)axis] = _nameGiven;
														}
											
									
														_actionSelected = null;
														_nameGiven = String.Empty;

														EditorUtility.SetDirty (_profiles);

														AssetDatabase.SaveAssets ();


														_nameGiveEdit = false;

														this.Repaint ();
										
												}



										}


										EditorGUILayout	.EndHorizontal ();
								}
						}
						

						///////////////         CREATE ASSET       ////////////////
						if (GUILayout.Button ("Create Assets/Resources/DeviceProfiles.asset")) {

								if (!Directory.Exists (Path.Combine (Application.dataPath, "Resources")))
										AssetDatabase.CreateFolder ("Assets", "Resources");
								
								if (File.Exists (Path.Combine (Path.Combine (Application.dataPath, "Resources"), "DeviceProfiles.asset"))) {
					
										if (EditorUtility.DisplayDialog ("DeviceProfiles Asset Exists!",
					                            "Are you sure you overwrite?", "Yes", "Cancel")) {
												AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<DeviceProfiles> (), "Assets/Resources/DeviceProfiles.asset");
										}
								} else {
										AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<DeviceProfiles> (), "Assets/Resources/DeviceProfiles.asset");
								}
				
				
						}






						//if event is of key or mouse
						if (Event.current.isKey) {
									
								if (Event.current.keyCode == KeyCode.Escape) {
										_nameGiveEdit = false;
										_instance.Repaint ();
								}		
						}
			
			
				}








				///////////////////     ON DESTROY     ////////////////
				void OnDestroy ()
				{

                    __wereDevicesEnumerated = false;
			
						if (!Application.isPlaying) {
				
				
				
								InputManager.Dispose ();
				
						}
				}
	
		
		}
}