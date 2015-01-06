using System;
using UnityEngine;
using ws.winx.input;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEditor;
using ws.winx.devices;

namespace ws.winx.gui
{
		public class UserInterfaceWindow : MonoBehaviour
		{





				protected Rect _buttonRect = new Rect (0, 0, 100, 15);
				protected Rect _layerLabelRect = new Rect (0, 0, 100, 15);
				protected Dictionary<int, InputState> _stateInputCombinations;


//        public Dictionary<int, InputState> StateInputCombinations
//        {
//            get { return _stateInputCombinations; }
//            set { _stateInputCombinations = value; }
//        }

				protected static bool _settingsLoaded = false;
				protected static bool _submitButton = false;
				protected static bool _deviceListShow = false;
				protected int _selectedStateHash = 0;
				protected string _combinationSeparator = InputAction.SPACE_DESIGNATOR.ToString ();
				protected int _isPrimary = 0;
				protected string _currentInputString;
				protected string _defaultProfile = "default";
				protected GUILayoutOption[] _inputLabelStyle = new GUILayoutOption[] { GUILayout.Width (200) };
				protected GUILayoutOption[] _stateNameLabelStyle = new GUILayoutOption[] { GUILayout.Width (250) };
#if UNITY_ANDROID || UNITY_IPHONE
                protected GUILayoutOption[] _inputButtonStyle = new GUILayoutOption[] { GUILayout.Height(200) };
                protected GUILayoutOption[] _submitButtonStyle = new GUILayoutOption[] {GUILayout.Height(200), GUILayout.Width(80) };
#else
				protected GUILayoutOption[] _inputButtonStyle = new GUILayoutOption[] { GUILayout.Height (30) };
#endif
				protected InputAction _action;
				protected Vector2 _scrollPosition = Vector2.zero;
				protected InputCombination _previousStateInput = null;


				//Players
				int _playerIndexSelected;
				int _playerIndexSelectedPrev;
				string[] _playerDisplayOptions;
				int[] _playerIndices;
				InputPlayer _playerSelected;

				//Device
				int _deviceDisplayIndex;
				string[] _deviceDisplayOptions;
				int[] _deviceIndices;
				int _deviceDisplayIndexPrev;

				/// <summary>
				/// Path very InputSettings would be saved
				/// </summary>
				/// 


				public string saveURL;
				public InputManager.InputSettings settings;
				public int maxCombosNum = 3;
				public GUISkin guiSkin;
				public TextAsset settingsXML;
				public Rect windowRect = new Rect (0, 0, 600, 430);
				//public bool allowDuplicates=false;

				void Start ()
				{


						if (!_settingsLoaded && settingsXML != null) {
								loadInputSettings ();
								_settingsLoaded = true;
						}



				}


				/// <summary>
				/// Update this instance.
				/// </summary>
				void Update ()
				{



						if (_selectedStateHash != 0) {
								// UnityEngine.Debug.Log("Edit mode true");
								//Use is mapping states so no quering keys during gameplay
								InputManager.EditMode = true;

								

							

			


									_action = InputManager.GetAction (_playerSelected.Device);




								if (_action != null && (_action.code ^ (int)KeyCode.Escape) != 0 && (_action.code ^ (int)KeyCode.Return) != 0) {


										if ((_action.code ^ (int)KeyCode.Backspace) == 0) {
												_stateInputCombinations [_selectedStateHash].combinations [_isPrimary].Clear ();
												_stateInputCombinations [_selectedStateHash].combinations [_isPrimary].Add (new InputAction (KeyCode.None));
										} else {
												toInputCombination (_stateInputCombinations [_selectedStateHash].combinations [_isPrimary], _action);
										}



										// Debug.Log("Action:" + _action + " " + _action.code);
								}


								//Debug.Log ("Action:"+action);
						} else {
								// UnityEngine.Debug.Log("Edit mode false");
								//Continue gameplay
								InputManager.EditMode = false;
						}


				}


				/// <summary>
				/// Saves the input settings.
				/// </summary>
				void saveInputSettings ()
				{

#if UNITY_WEBPLAYER && !UNITY_EDITOR
            if (saveURL == null) throw new Exception("Save path should point to some web service resposable for saving");

                   InputManager.saveSettings(saveURL);
#endif

#if UNITY_WEBPLAYER && UNITY_EDITOR
           
            if(settingsXML!=null)
                InputManager.saveSettings(Path.Combine(Application.streamingAssetsPath, settingsXML.name + ".xml"));
             else 
				InputManager.saveSettings(Path.Combine(Application.streamingAssetsPath, "InputSettings.xml"));


#endif

#if UNITY_ANDROID
                    //Try to save to /storage/sdcard0/Android/data/ws.winx.InputManager/files

                         if (settingsXML != null)
			                  InputManager.saveSettings(Application.persistentDataPath+"/"+settingsXML.name+".xml");
                         else
				              InputManager.saveSettings(Application.persistentDataPath+"/InputSettings.xml");

                         Debug.Log("UI>> Try to save to " + Application.persistentDataPath);
#endif


#if UNITY_STANDALONE
            if (settingsXML != null)
                InputManager.saveSettings(Path.Combine(Application.streamingAssetsPath, settingsXML.name + ".xml"));
            else
                InputManager.saveSettings(Path.Combine(Application.streamingAssetsPath, "InputSettings.xml"));

#endif
				}


				/// <summary>
				/// Loads the input settings.
				/// </summary>
				void loadInputSettings ()
				{

						//UnityEngine.Debug.Log("loadInputSettings");

						//clone(cos maybe some are added manually)
						// _stateInputCombinations = new Dictionary<int, InputState>(InputManager.Settings.stateInputs);

						//load settngs from TextAsset(seem its utf-8 so not need of reading BOM)
						InputManager.loadSettingsFromText (settingsXML.text, false);


						//var stateInputs = InputManager.Settings.stateInputs;

						////concat//concate with priority of keys/items loaded from .xml
						//foreach (var KeyValuePair in _stateInputCombinations)
						//{
						//    if (!stateInputs.ContainsKey(KeyValuePair.Key))
						//        InputManager.Settings.stateInputs.Add(KeyValuePair.Key, KeyValuePair.Value);


						//}

						//clone(cos maybe some are added manually)
						//_stateInputCombinations = new Dictionary<int, InputState>(InputManager.Settings.stateInputs);

						settings = InputManager.Settings;
						_stateInputCombinations = settings.Players [0].DeviceProfileStateInputs ["default"];


				}


				/// <summary>
				/// Tos the input combination.
				/// </summary>
				/// <param name="combos">Combos.</param>
				/// <param name="input">Input.</param>
				void toInputCombination (InputCombination combos, InputAction input)
				{

						if (combos.numActions + 1 > maxCombosNum || (combos.numActions == 1 && combos.GetActionAt (0).code == 0))
								combos.Clear ();

						combos.Add (input);


				}

				/// <summary>
				/// Raises the GU event.
				/// </summary>
				private void OnGUI ()
				{
						GUI.skin = guiSkin;

						GUI.Window (1, windowRect, CreateWindow, new GUIContent ());
						//GUI.Window(1, new Rect(0, 0, Screen.width, Screen.height), CreateWindow,new GUIContent());



						//if event is of key or mouse
						if (Event.current.isKey) {



								if (Event.current.keyCode == KeyCode.Return) {
										_selectedStateHash = 0;
										_previousStateInput = null;
										//this.Repaint ();
								} else if (Event.current.keyCode == KeyCode.Escape) {
										if (_selectedStateHash != 0) {
												_stateInputCombinations [_selectedStateHash].combinations [_isPrimary] = _previousStateInput;
												_previousStateInput = null;
												_selectedStateHash = 0;
										}
								}





						}

						//Approach dependent of GUI so not applicable if you have 3D GUI
						//if (_selectedStateHash != 0)
						//	InputEx.processGUIEvent (Event.current);//process input from keyboard & mouses

				}


				/// <summary>
				/// Creates the window.
				/// </summary>
				/// <param name="windowID">Window I.</param>
				private void CreateWindow (int windowID)
				{

						GUILayout.Label ("http://unity3de.blogspot.com/");

						GUILayout.Label ("InputEx");
						int i = 0;

						if (settings != null) {

								///////// PLAYERS //////////
		 
								if (_playerDisplayOptions == null) {
			
										int numPlayers = settings.Players.Length;
										_playerDisplayOptions = new string[numPlayers];
				

										for (i=0; i<numPlayers; i++) {
												
												_playerDisplayOptions [i] = "Player" + i;
										}
				
				

								}


								_playerIndexSelected = GUILayout.SelectionGrid (_playerIndexSelected, _playerDisplayOptions, _playerDisplayOptions.Length);
			
							_playerSelected=settings.Players[_playerIndexSelected];
				
				// close/hide device list when player index changed
								if (_playerIndexSelectedPrev != _playerIndexSelected) {
										_deviceListShow = false;
										_deviceDisplayIndexPrev = 0;
								}

								_playerIndexSelectedPrev = _playerIndexSelected;
								



							
				
				
				
								///////////// DEVICES /////////
								

								
								List<IDevice> devicesList = InputManager.GetDevices<IDevice> ();





								if (devicesList.Count > 0) {

										//remove devices already assigned except of currentPlayer's Device

										for (i=0; i<settings.Players.Length; i++) {
												IDevice device = settings.Players [i].Device;
												if (device != null && !device.Equals (_playerSelected.Device))
														devicesList.Remove (device);

										}
					                    

										//Fill Device List (first option NoDevice)
										_deviceDisplayOptions = new string[devicesList.Count + 1];
										
										_deviceDisplayOptions [0] = "No Device";
										for (i=1; i<_deviceDisplayOptions.Length;) {
												//if(currentPlayer.Device!=devices [i - 1]){
												_deviceDisplayOptions [i] = "(" + (i - 1) + ")" + devicesList [i - 1].Name;
												i++;
												//}
										

										}
										
										
									
										string deviceButtonLabel = _playerSelected.Device != null ? _playerSelected.Device.Name : "No Device";

										int deviceIndex = -1;

										if (_playerSelected.Device != null) {
												deviceIndex = devicesList.IndexOf (_playerSelected.Device);
												_deviceDisplayIndex = deviceIndex + 1;
										} else {
												_deviceDisplayIndex = 0;
										}

										
										//toggle DeviceList
										if (GUILayout.Button (deviceButtonLabel)) {

												_deviceListShow = !_deviceListShow;
									
										}

										//show Device List
										if (_deviceListShow) {

												//get selection
												_deviceDisplayIndex = GUILayout.SelectionGrid (_deviceDisplayIndex, _deviceDisplayOptions, 1);
												if (_deviceDisplayIndexPrev != _deviceDisplayIndex)
														_deviceListShow = false;
												_deviceDisplayIndexPrev = _deviceDisplayIndex;
								
										}



										//assign/remove device to player
										if (_deviceDisplayIndex == 0)
												_playerSelected.Device = null;
										else if (deviceIndex + 1 != _deviceDisplayIndex)
												_playerSelected.Device = devicesList [_deviceDisplayIndex - 1];

									
								
								}




								

									
								///PROFILE
								//if device is assigned to player and device have profile and there are input settings for that profile
								if (_playerSelected.Device != null && _playerSelected.Device.profile != null && _playerSelected.DeviceProfileStateInputs.ContainsKey (_playerSelected.Device.profile.Name)) {
										
										_stateInputCombinations = _playerSelected.DeviceProfileStateInputs [_playerSelected.Device.profile.Name];
								} else {
										_stateInputCombinations = _playerSelected.DeviceProfileStateInputs [_defaultProfile];
								}
										
									
									



								
								
								
						}

						//////////// INPUT STATES /////////////
						_scrollPosition = GUILayout.BeginScrollView (_scrollPosition, false, true);



						if (_stateInputCombinations != null)
								foreach (var keyValuPair in _stateInputCombinations) {
										//primary,secondary...
										createInputStateGUI (keyValuPair.Key, keyValuPair.Value.name, keyValuPair.Value.combinations);

								}

						GUILayout.EndScrollView ();
						//////////////////////////////////////



						GUILayout.Space (20);

						if (_selectedStateHash == 0 && GUILayout.Button ("Save")) {

								saveInputSettings ();
						}

				}




				/// <summary>
				/// Creates the Input State GUI.
				/// </summary>
				/// <param name="hash">State Hash.</param>
				/// <param name="stateName">State name.</param>
				/// <param name="combinations">State Combinations.</param>
				void createInputStateGUI (int hash, string stateName, InputCombination[] combinations)
				{

						string currentCombinationString;


						GUILayout.BeginHorizontal ();

						//string stateName=((CharacterInputControllerClass.States)hash).ToString();



						//(AnimatorEnum)hash
						//GUILayout.Label(stateName.Remove(0,stateName.IndexOf("Layer")+6).Replace("_"," "),_stateNameLabelStyle);
						GUILayout.Label (stateName, _stateNameLabelStyle);


						if (_selectedStateHash != hash) {


								if (GUILayout.Button (InputCode.toProfiled (_playerSelected.Device, combinations [0]), _inputButtonStyle)) {
										// if (GUILayout.Button(combinations[0].combinationString, _inputButtonStyle))
										_selectedStateHash = hash;
										_previousStateInput = null;
										_isPrimary = 0;
								}

								if (combinations.Length > 1 && combinations [1] != null)
								if (GUILayout.Button (InputCode.toProfiled (_playerSelected.Device, combinations [1]), _inputButtonStyle)) {
										//if (GUILayout.Button(combinations[1].combinationString, _inputButtonStyle))
										_selectedStateHash = hash;
										_previousStateInput = null;
										_isPrimary = 1;
								}


						} else {





								currentCombinationString = InputCode.toProfiled (_playerSelected.Device, combinations [_isPrimary]);
								//currentCombinationString = combinations[_isPrimary].combinationString;

								if (_previousStateInput == null) {
										_previousStateInput = combinations [_isPrimary].Clone ();
								}


								GUILayout.Label (currentCombinationString);//, _inputLabelStyle);

#if UNITY_ANDROID || UNITY_IPHONE
                                if (GUILayout.Button("Submit",_submitButtonStyle))
                                {
                                    _selectedStateHash = 0;
                                    _previousStateInput = null;
                                    return;
                                }
#endif

								//this.Repaint ();
						}



						//Debug.Log ("_selectedStateHash after" + _selectedStateHash);



						GUILayout.EndHorizontal ();



						GUILayout.Space (20);
				}




				/// <summary>
				/// DONT FORGET TO CLEAN AFTER YOURSELF
				/// </summary>
				void OnDestroy ()
				{
						Debug.Log ("onDestroy UserInterfaceWindow");

						_selectedStateHash = 0;

						Debug.Log ("onDestroy End UserInterfaceWindow");
				}



		}
}
