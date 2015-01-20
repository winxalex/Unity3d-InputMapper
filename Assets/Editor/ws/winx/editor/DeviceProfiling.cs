

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
				static RuntimePlatform _platform;
				static bool _nameGiveEdit;

				//Device
				static int _deviceDisplayIndex;
				static string[] _displayOptions;
				static IDevice _deviceSelected;
				


				//profiles
				static DeviceProfiles _profiles;
				static int _profileIndexSelected;
				static string _profileName;
				static string _axisButtonName;
				static string _nameGiven;
				static string _profileNameSelected;
				static InputAction _actionSelected;

				// Add menu named "Input Mapper" to the Window menu
				[MenuItem ("Window/Input Mapper/DeviceProfiling")]
				static void ShowEditor ()
				{

						// Get existing open window or if none, make a new one:
						if (_instance == null)
						if (!Application.isPlaying) {
								InputManager.hidInterface.Enumerate ();
								
						}

						_instance = EditorWindow.GetWindow<DeviceProfiling> ();
				}



				/// <summary>
				/// Update this instance.
				/// </summary>
				void Update ()
				{

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
										_instance.Repaint ();
										
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
						List<IDevice> devicesList = InputManager.GetDevices<IDevice> ();

						if (devicesList.Count > 0) {
								_displayOptions = devicesList.Select (item => item.Name).ToArray ();

								_deviceDisplayIndex = EditorGUILayout.Popup ("Devices:", _deviceDisplayIndex, _displayOptions);

								_deviceSelected = devicesList [_deviceDisplayIndex];
						} else {

								EditorGUILayout.LabelField ("No attached devices");
						}

						

						if (_profiles != null && !String.IsNullOrEmpty (_profileNameSelected)) {

								EditorGUILayout.BeginHorizontal ();
								if (GUILayout.Button ("Assign Profile")) {
				
										string pidVidKey = _deviceSelected.VID.ToString ("X4") + "#" + _deviceSelected.PID.ToString ("X4");
										_profiles.pidvidShortTypeNames [pidVidKey] = _profileNameSelected;

										EditorUtility.SetDirty (_profiles);
										AssetDatabase.SaveAssets ();
								}

								if (GUILayout.Button ("Remove From Profile")) {
				
										string pidVidKey = _deviceSelected.VID.ToString ("X4") + "#" + _deviceSelected.PID.ToString ("X4");

										_profiles.pidvidShortTypeNames.Remove (pidVidKey);

										EditorUtility.SetDirty (_profiles);
										AssetDatabase.SaveAssets ();
				
								}

								EditorGUILayout.EndHorizontal ();
						}
						

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

														_instance.Repaint ();
										
												}



										}


										EditorGUILayout	.EndHorizontal ();
								}
						}
						

						///////////////         CREATE ASSET       ////////////////
						if (GUILayout.Button ("Create Profiles Asset")) {

								if (!Directory.Exists (Path.Combine (Application.dataPath, "DeviceProfiles")))
										AssetDatabase.CreateFolder ("Assets", "DeviceProfiles");
								
								if (File.Exists (Path.Combine (Path.Combine (Application.dataPath, "DeviceProfiles"), "DeviceProfiles.asset"))) {
					
										if (EditorUtility.DisplayDialog ("DeviceProfiles Asset Exists!",
					                            "Are you sure you overwrite?", "Yes", "Cancel")) {
												AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<DeviceProfiles> (), "Assets/DeviceProfiles/DeviceProfiles.asset");
										}
								} else {
										AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<DeviceProfiles> (), "Assets/DeviceProfiles/DeviceProfiles.asset");
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
			
					
			
						if (!Application.isPlaying) {
				
				
				
								InputManager.Dispose ();
				
						}
				}
	
		
		}
}