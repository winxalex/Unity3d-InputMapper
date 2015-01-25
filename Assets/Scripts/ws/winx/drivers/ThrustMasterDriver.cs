using System;
using ws.winx.devices;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using ws.winx.platform;
using System.Timers;
using ws.winx.input;

namespace ws.winx.drivers
{
		public class ThrustMasterDriver:IDriver
		{

				private IHIDInterface __hidInterface;

				public IDevice ResolveDevice (IHIDDevice hidDevice)
				{
						if (hidDevice.PID == 46675 && hidDevice.VID == 1103) {
								this.__hidInterface = hidDevice.hidInterface;

								//check for profile
								DeviceProfile profile = hidDevice.loadProfile();;




								JoystickDevice device = new ThrustmasterRGTFFDDevice (hidDevice.index, hidDevice.PID, hidDevice.VID, hidDevice.ID, 8, 10, this);
								device.Name = hidDevice.Name;
                                device.profile = profile;
								//JoystickDevice joystick = new JoystickDevice(hidDevice.index, hidDevice.PID, hidDevice.VID, 8, 10, this);

               






								int index = 0;
								for (; index < 10; index++) {
										device.Buttons [index] = new ButtonDetails ();

										if (profile != null && profile.buttonNaming.Length > index) {
												device.Buttons [index].name = profile.buttonNaming [index];
										}
								}


								for (index = 0; index < 8; index++) {

										device.Axis [index] = new AxisDetails ();
										if (profile != null && profile.axisNaming.Length > index && device.Axis [index] != null) {
												device.Axis [index].name = profile.axisNaming [index];
						
										}

								}


           

								return device;
						}

						return null;
				}

				public void Update (IDevice device)
				{

						if (device != null && __hidInterface != null && __hidInterface.Contains (device.ID)) {
               
								//  Debug.Log("Update Joy"+device.Index);
								HIDReport data = __hidInterface.ReadBuffered (device.ID);

								if (data != null)
										onRead (data);
                
						}
						// __hidInterface.Generics[device].Read(onRead);
           
				}


				/// <summary>
				/// Move FFD motor of the wheel left or right
				/// </summary>
				/// <param name="forceXY">0xFF - 0xA7(left) and 0x00-0x64(rights) are measurable by feeling </param>
				internal void SetMotor (IDevice device, byte forceX, byte forceY, HIDDevice.WriteCallback callback)
				{


						if (__hidInterface.Generics.ContainsKey (device.ID)) {
								//TODO check if device use sbytes for 0x80 to 0x7f (-128 to 127)
								//Couldn't figure out if forceY doing something

								byte[] data = new byte[5];
								data [0] = 0x40;
								data [1] = forceX;
								data [2] = forceY;
								data [3] = 0x00;
								data [4] = 0x00;

								__hidInterface.Write (data, device.ID, callback);
								//__hidInterface.Generics[device].Write(data, callback);
						}
				}
     
				protected void onRead (object data)
				{
						HIDReport report = data as HIDReport;

           


						// Debug.Log("ThustmasterDriver>>onRead:" + data);

						if (report != null && (report.Status == HIDReport.ReadStatus.Success || report.Status == HIDReport.ReadStatus.Buffered) && report.Data [0] == 0x01) {

               
								// Debug.Log("ThustmasterDriver>>onRead:processRead" + BitConverter.ToString(report.Data));


								JoystickDevice device = InputEx.Devices.GetDeviceAt (report.index) as JoystickDevice;

								//TODO in future use some sofisticate syncronization system as things gone threading unpredicatable
								//have noticed 2 packets sent in same frame
               
                

								//    Debug.Log("ThustmasterDriver>>onRead:Index" + report.index);
								//do something with the data
								//01 00 BC 87 FF FF FF FF                            ..ј‡яяяя
								//01 - is requestID (should be one for bulk data)
								//00 - button clicked info (00100011) button 0,1,5 are clicked
								//0011 1111 1111
								//
								// 87B (10bits) - Wheel axis (max. left -512 - max right 511) 

								// BC (4 bits from 2 to 6th)(min.0 to 7) FF- centered   Hat(POV)

								//FF - trigger left gear shifiter | (Break)
								//FF - trigger right shifiter  | (Gas)
								//FF - trigger (Step Shifter)
								//FF - trigger Dial
              
               

			




               


								if (device != null) {
										//if (!device.isReady) return;

										//((JoystickDevice)device).isReady = false;



										int numButtons = device.Buttons.Count;
										int buttonInx = 0;

										// last 2bits of data2 + 8 bits of data1 
										// 00 BE 
										int buttonInfo = (report.Data [1] | ((report.Data [2] & 0x3) << 8));

										//       UnityEngine.Debug.Log("buttonSequence:"+Convert.ToString(buttonInfo, 2));

										while (buttonInx < numButtons) {
												//stick.SetButton (buttonInx, (info.Buttons & (1 << buttonInx)) != 0);
												// UnityEngine.Debug.Log(Convert.ToString(buttonInfo,2));
                        
												device.Buttons [buttonInx].value = Convert.ToSingle (buttonInfo & (1 << buttonInx));
                      
												buttonInx++;
										}

										//UnityEngine.Debug.Log("but0:" + device.Buttons[0].value + " " + device.Buttons[0].buttonState+" frame "+device.LastFrameNum+"phase:"+report.Status);


										//HAT
										float x = 0f;
										float y = 0f;

										//from 2th bit to 6th bit
										int direction = (report.Data [2] >> 2) & 0xF;

                   
										//TODO use hatToXY function from OSXDRiver as more optimized
										if (direction < 0xf) {
												hatValueToXY (direction, 8, out x, out y);
										}

										//UnityEngine.Debug.Log("Direction:" + direction+"x="+x+" y="+y);
                    
										device.Axis [JoystickAxis.AxisPovX].value = x;
										device.Axis [JoystickAxis.AxisPovY].value = y;

										// UnityEngine.Debug.Log("AxisPovX:" + device.Axis[JoystickAxis.AxisPovY].value + " " + device.Axis[JoystickAxis.AxisPovY].buttonState + " frame " + device.LastFrameNum + "phase:" + report.Status);
										//UnityEngine.Debug.Log("Joy"+device.Index+" AxisPovY:" + device.Axis[JoystickAxis.AxisPovY].value + " " + device.Axis[JoystickAxis.AxisPovY].buttonState + " frame:" + device.LastFrameNum + " phase:" + report.Status);


										//X-Axis (8 bits of Data3 + 2bits of Data2
										device.Axis [0].value = NormalizeAxis ((float)((unchecked((sbyte)report.Data [3]) << 2) | (report.Data [2] >> 6)), -512, 511);
										// UnityEngine.Debug.Log("AxisX:" + device.Axis[0].value);


										//Y-Axis
										device.Axis [1].value = NormalizeTrigger ((float)report.Data [4], 0, 255);
										// UnityEngine.Debug.Log("AxisY:" + device.Axis[1].value + " state: " + device.Axis[1].buttonState);


										//Z-Axis
										device.Axis [2].value = NormalizeTrigger ((float)report.Data [5], 0, 255);

										device.Axis [3].value = NormalizeTrigger ((float)report.Data [6], 0, 255);


										// UnityEngine.Debug.Log("Axis:" + device.Axis[0].value +","+ device.Axis[1].value +","+ device.Axis[2].value+"," + device.Axis[3].value);
                 
								}


             
                
               
						}  
            
           
				}






				//					   0                 
				//					   |
				//				3______|______1
				//					   |
				//					   |
				//					   2



				//				  7    0     1            
				//				   \   |   /
				//				6 _____|______2
				// 					  /|\
				//					/  |  \
				//				   5   4    3


				/// <summary>
				/// Hats the value to X.
				/// </summary>
				/// <param name="value">Value.</param>
				/// <param name="range">Range.</param>
				/// <param name="outX">Out x.</param>
				/// <param name="outY">Out y.</param>
				void hatValueToXY (int value, int range, out float outX, out float outY)
				{

						outX = outY = 0f;
						int rangeHalf = range >> 1;
						int rangeQuat = range >> 2;

						if (value > 0 && value < rangeHalf) {
								outX = 1f;

						} else if (value > rangeHalf) {
								outX = -1f;
						}

						if (value > rangeQuat * 3 || value < rangeQuat) {
								outY = 1f;

						} else if (value > rangeQuat && value < rangeQuat * 3) {
								outY = -1f;
						}

				}





				/// <summary>
				/// Normalize raw axis value to -1 to 1 range.
				/// </summary>
				/// <returns>The offset.</returns>
				/// <param name="pos">Position.</param>
				/// <param name="min">Minimum.</param>
				/// <param name="max">Max.</param>
				/// <param name="dreadZone">Dread zone.</param>
				public float NormalizeAxis (float pos, int min, int max, float dreadZone = 0.001f)
				{
						float value;

						// UnityEngine.Debug.Log(pos);

						if (pos > 0)
								value = pos / max;
						else if (pos < 0)
								value = -pos / min;
						else
								return 0f;
               
            
						if (value > 1)
								return 1;
						else if (value < -1)
								return -1;
						else if (value < dreadZone && value > -dreadZone)
								return 0;
						else
								return value;
				}

				/// <summary>
				///  Normalize raw axis value to 0 - 1 range.
				/// </summary>
				/// <param name="pos">255(released) - 0(full pressed) </param>
				/// <param name="min"></param>
				/// <param name="max"></param>
				/// <param name="dreadZone"></param>
				/// <returns></returns>
				public float NormalizeTrigger (float pos, int min, int max, float dreadZone = 0.001f)
				{
						float value = 1 - pos / (max - min);

						//UnityEngine.Debug.Log("trigger:"+pos+ "value:"+value);

						if (value > 1)
								return 1;

						if (value < dreadZone && value > -dreadZone)
								return 0;

						return value;

				}

				internal void StopMotor (IDevice device)
				{
						byte[] data = new byte[5];
						data [0] = 0x40;
						data [1] = 0x7f;
						data [2] = 0xff;
						data [3] = 0x00;
						data [4] = 0x00;


						this.__hidInterface.Write (data, device.ID);
						//this.__hidInterface.Generics[device].Write(data);
               
				}

				internal void StopMotor (IDevice device, HIDDevice.WriteCallback callback)
				{
						byte[] data = new byte[5];
						data [0] = 0x40;
						data [1] = 0x7f;
						data [2] = 0xff;
						data [3] = 0x00;
						data [4] = 0x00;

						this.__hidInterface.Write (data, device.ID, callback);
						//this.__hidInterface.Generics[device].Write(data, callback);
				}

    
		}
    




    #region ButtonDetails
		public sealed class ButtonDetails : IButtonDetails
		{

        #region Fields

				float _value;
				uint _uid;
				string _name;
				ButtonState _buttonState;

        #region IDeviceDetails implementation

				public string name {
						get {
								return _name;
						}
						set {
								_name = value;
						}
				}

				public uint uid {
						get {
								return _uid;
						}
						set {
								_uid = value;
						}
				}

				public ButtonState buttonState {
						get { return _buttonState; }
				}

				public float value {
						get {
								return _value;
								//return (_buttonState==JoystickButtonState.Hold || _buttonState==JoystickButtonState.Down);
						}
						set {

								_value = value;

								//  UnityEngine.Debug.Log("Value:" + _value);

								if (value > 0) {
										if (_buttonState == ButtonState.None
												|| _buttonState == ButtonState.Up) {

												_buttonState = ButtonState.Down;



										} else {
												//if (buttonState == JoystickButtonState.Down)
												_buttonState = ButtonState.Hold;

										}


								} else { //
										if (_buttonState == ButtonState.Down
												|| _buttonState == ButtonState.Hold) {
												_buttonState = ButtonState.Up;
										} else {//if(buttonState==JoystickButtonState.Up){
												_buttonState = ButtonState.None;
										}

								}
						}
				}
        #endregion
        #endregion

        #region Constructor
				public ButtonDetails (uint uid = 0)
				{
						this.uid = uid;
				}
        #endregion








       
		}

    #endregion

    #region AxisDetails
		public sealed class AxisDetails : IAxisDetails
		{

        #region Fields
				float _value;
				int _uid;
				int _min;
				int _max;
				string _name;
				ButtonState _buttonState = ButtonState.None;
				bool _isNullable;
				bool _isHat;
				bool _isTrigger;


        #region IAxisDetails implementation

				public bool isTrigger {
						get {
								return _isTrigger;
						}
						set {
								_isTrigger = value;
						}
				}

				public int min {
						get {
								return _min;
						}
						set {
								_min = value;
						}
				}

				public int max {
						get {
								return _max;
						}
						set {
								_max = value;
						}
				}

				public bool isNullable {
						get {
								return _isNullable;
						}
						set {
								_isNullable = value;
						}
				}

				public bool isHat {
						get {
								return _isHat;
						}
						set {
								_isHat = value;
						}
				}


        #endregion


        #region IDeviceDetails implementation


				public uint uid {
						get {
								throw new NotImplementedException ();
						}
						set {
								throw new NotImplementedException ();
						}
				}


        #endregion

				public string name {
						get {
								return _name;
						}
						set {
								_name = value;
						}
				}

				public ButtonState buttonState {
						get { return _buttonState; }
				}

				public float value {
						get { return _value; }
						set {
				
								if (value == -1 || value == 1) {
										if (_buttonState == ButtonState.None
					     //|| _buttonState == ButtonState.PosToUp || _buttonState==ButtonState.NegToUp)
					    ) {
						
												_buttonState = ButtonState.Down;
						
												//Debug.Log("val:"+value+"_buttonState:"+_buttonState);
						
										} else {
												_buttonState = ButtonState.Hold;
										}
					
					
								} else {
					
										if (_buttonState == ButtonState.Down
												|| _buttonState == ButtonState.Hold) {
						
												//if previous value was >0 => PosToUp
												if (_value > 0)
														_buttonState = ButtonState.PosToUp;
												else
														_buttonState = ButtonState.NegToUp;
						
												//Debug.Log("val:"+value+"_buttonState:"+_buttonState);
						
										} else {//if(buttonState==JoystickButtonState.Up){
												_buttonState = ButtonState.None;
										}
					
					
								}
				
				
								_value = value;
				
				
				
						}//set
				}


        #endregion



     
		}

    #endregion




   
}
