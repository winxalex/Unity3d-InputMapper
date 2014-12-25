using System;
using System.Collections.Generic;
using UnityEngine;
using ws.winx.platform;
using ws.winx.input;

namespace ws.winx.devices
{
		/// <summary>
		/// Represents a joythis device and provides methods to query its status.
		/// </summary>
		public /*abstract*/ class JoystickDevice//<TAxisDetails,TButtonDetails,TDeviceExtension> 
        :IDevice
//		: IDevice<TAxisDetails,TButtonDetails,TDeviceExtension>
				//where TAxisDetails:IAxisDetails
				//where TButtonDetails:IButtonDetails
				//where TDeviceExtension:IDeviceExtension
		{
        #region Fields
				

		

				
				IDriver _driver;
				int _index = -1;
				string _description;
				JoystickAxisCollection<IAxisDetails> axis_collection;
				JoystickButtonCollection<IButtonDetails> button_collection;
				int _numPOV = 0;
				int _numButtons;
				int _numAxis;
				//bool _anyKeyDown = false;
				int _VID;
				int _PID;
				bool _isReady = true;
				int _lastFrameNum = -1;
				//int axisDominantPrevInx = -1;

				public int LastFrameNum {
						get { return _lastFrameNum; }
						set { _lastFrameNum = value; }
				}

        #endregion

        #region Constructors

				internal JoystickDevice (int index, int pid, int vid, int axes, int buttons, IDriver driver)
				{
						if (axes < 0)
								throw new ArgumentOutOfRangeException ("axes");

						if (buttons < 0)
								throw new ArgumentOutOfRangeException ("buttons");

			        
			     

						_numAxis = axes;
						_numButtons = buttons;

                       

						_index = index;
						_VID = vid;
						_PID = pid;
						_driver = driver;
						axis_collection = new JoystickAxisCollection<IAxisDetails> (axes);
						button_collection = new JoystickButtonCollection<IButtonDetails> (buttons);
			            
           
				}

        #endregion

        #region Public Members

		#region IDevice implementation


				public bool isReady {
						get {
								return _isReady;
						}
						internal set {
								_isReady = value;
						}
				}

				public int numPOV {
						get {
								return _numPOV;
						}
						set {
								_numPOV = value;
						}
				}

				public string Name {
						get {
								return _description;
						}
						set {
								_description = value;
						}
				}

				public IDriver driver {
						get {
								return _driver;
						}
		
				}

				public int PID {
						get {
								return _PID;
						}
						internal set {
								_PID = value;
						}
				}

				public int VID {
						get {
								return _VID;
						}
						internal set {
								_VID = value;
						}
				}

     
	
				/// <summary>
				/// index is value 0 to 15 and given by driver
				/// </summary>
				public int Index {
						get { return _index; }
						internal set { _index = value; }
				}

				public virtual void Update ()
				{

//
						// if(this.ID==1)
						//UnityEngine.Debug.Log("Update"+this.ID+" Current frame:"+Time.frameCount+" lastFrame:"+_lastFrameNum);
           
						//lock is done so no other updates of joystick is done while frame
						//many Joy.GetXXX could be called in same frame so updates would make JosytickDevice values unusable
						if (_lastFrameNum == Time.frameCount) {
								// if(this.ID==1)
								// UnityEngine.Debug.Log("skip" + this.ID + " cos its same frame"+Time.frameCount);
                    
						} else {
								// if(this.ID==1)
								// UnityEngine.Debug.Log("Update" + this.ID + " Current frame:" + Time.frameCount + " lastFrame:" + _lastFrameNum);
                 
								_lastFrameNum = Time.frameCount;
								_driver.Update (this);// as IDevice<IAxisDetails,IButtonDetails,IDeviceExtension>);

                   
						}
         
          

						//_driver.Update(this);

				}

			
				/// <summary>
				/// Gets the axis value.
				/// </summary>
				/// <returns>The axis.</returns>
				/// <param name="code">Code. joystick(4bits) + axis(4bits) + data(+/- 5bits)</param>
				public float GetInput (int code)
				{
						Update ();

						JoystickAxis axis = InputCode.toAxis (code);
						int data = InputCode.toData (code);
						float axisValue = axis_collection [axis].value;


						// axis_collection [axis].isHat
						if (axis == JoystickAxis.AxisPovX) {
								if (data == (int)JoystickPovPosition.Left && axisValue < 0)
										return axisValue;
								if (data == (int)JoystickPovPosition.Right && axisValue > 0)
										return axisValue;
							
								return 0;
							
						}
						
						
						if (axis == JoystickAxis.AxisPovY) {
								if (data == (int)JoystickPovPosition.Backward && axisValue < 0)
										return axisValue;
								if (data == (int)JoystickPovPosition.Forward && axisValue > 0)
										return axisValue;
								return 0;
						}
						
						
						if (data == (int)JoystickPosition.Full)
								return axisValue;
						
						if (data == (int)JoystickPosition.Negative && axisValue < 0)
								return axisValue;
						if (data == (int)JoystickPosition.Positive && axisValue > 0)
								return axisValue;


						return 0;
				}




				/// <summary>
				/// Gets the key up.
				/// </summary>
				/// <returns><c>true</c>, if key up was gotten, <c>false</c> otherwise.</returns>
				/// <param name="code">Code.</param>
				public virtual  bool GetInputUp (int code)
				{
						Update ();// 

						// UnityEngine.Debug.Log("GetKeyUP");


						JoystickAxis axis = InputCode.toAxis (code);
						int data = InputCode.toData (code);
                    
						if (axis == JoystickAxis.None) {

								//previous mapping might be to device with less or more buttons
								//at same device index
								if (button_collection.Count > data)
										return button_collection [data].buttonState == ButtonState.Up;
								else
										return false;
						}

			//previous mapping might be to device with less or more axess
			//at same device index
			if (axis_collection.Count <= (int)axis)
								return false;

			IAxisDetails axisDetails= axis_collection [axis];

			if (axisDetails == null)
								return false;

						if (axis == JoystickAxis.AxisPovX) {
								if (data == (int)JoystickPovPosition.Left && axisDetails.buttonState == ButtonState.NegToUp)
										return true;
								if (data == (int)JoystickPovPosition.Right && axisDetails.buttonState == ButtonState.PosToUp)
										return true;
								return false;
						}

						if (axis == JoystickAxis.AxisPovY) {
								if (data == (int)JoystickPovPosition.Backward && axisDetails.buttonState == ButtonState.NegToUp)
										return true;
								if (data == (int)JoystickPovPosition.Forward && axisDetails.buttonState == ButtonState.PosToUp)
										return true;
								return false;
						}

						//check if the axis moved in Negative values was relesed
						if (data == (int)JoystickPosition.Negative && axisDetails.buttonState == ButtonState.NegToUp)
								return true;
						if (data == (int)JoystickPosition.Positive && axisDetails.buttonState == ButtonState.PosToUp)
								return true; 

						return false;
				}




				public virtual bool GetInputBase (int code, ButtonState buttonState)
				{

			//UnityEngine.Debug.Log("Button state>" + buttonState+" code:"+code);

						if (ButtonState.Up == buttonState)
								return GetInputUp (code);



						Update ();
							
						// if (!this.isReady) return false;
							
						//UnityEngine.Debug.Log("GetKeyDown Joy"+this.ID);
							
						JoystickAxis axis = InputCode.toAxis (code);
						int data = InputCode.toData (code);
							
						if (axis == JoystickAxis.None) {   //MO data for axis => buttons data
								//UnityEngine.Debug.Log("Button state>" + button_collection[data].buttonState);

								//previous mapping might be to device with less or more buttons
								//at same device index
								if(button_collection.Count>data)
									return button_collection [data].buttonState == buttonState;
								else
									return false;
								
						}

						

			//previous mapping might be to device with less or more axess
			//at same device index
			if (axis_collection.Count <= (int)axis)
				return false;
			
			IAxisDetails axisDetails= axis_collection [axis];

			if (axisDetails == null)
				return false;
			
			bool isEqualToButtonState = axisDetails.buttonState == buttonState;
							
							
							
						if (axis == JoystickAxis.AxisPovX) {


								if (data == (int)JoystickPovPosition.Left && isEqualToButtonState && axisDetails.value < 0)
										return true;
								if (data == (int)JoystickPovPosition.Right && isEqualToButtonState && axisDetails.value > 0)
										return true;
								
								return false;
								
						}
							
							
						if (axis == JoystickAxis.AxisPovY) {
								if (data == (int)JoystickPovPosition.Backward && isEqualToButtonState && axisDetails.value < 0)
										return true;
								if (data == (int)JoystickPovPosition.Forward && isEqualToButtonState && axisDetails.value > 0)
										return true;
								
								return false;
								
						}
							
							
						if (data == (int)JoystickPosition.Negative && isEqualToButtonState && axisDetails.value < 0)
								return true;


						if (data == (int)JoystickPosition.Positive && isEqualToButtonState && axisDetails.value > 0) {

							//	UnityEngine.Debug.Log("data:" + data+"buttotnState:"+buttonState+" equal"+isEqualToButtonState+" value:"+axisDetails.value);
								return true;
						}
							
							
						return false;

				}


			


				/// <summary>
				/// Gets the input.
				/// IMPORTANT: If button or axis is already "DOWN" returns 0
				/// GetInput is called in "edit mode" (when mapping in UI) and anykeydown for devices
				/// during combos process
				/// </summary>
				/// <returns>The input.</returns>
				public virtual int GetInputCode ()
				{

						// UnityEngine.Debug.Log("GetInput:" + Time.frameCount);

						//there is no  Time.frameCount in Editor (Edit mode - Editor scripts)
						if (Application.isPlaying) {
								//prevents multiply update in frame
								Update ();      
						} else  
								_driver.Update (this);
            

          
						int num_axes = _numAxis;
						
						int axis = 0;
					
					
						IAxisDetails axisDetails;


                        //UnityEngine.Debug.Log("Code" + axis_collection[6].buttonState);
						//UnityEngine.Debug.Log("Count" + axis_collection.Count);
						// if(this.Index==0)
						//     UnityEngine.Debug.Log("GetInput>Joy" + this.Index + "AxisPovY state:" + axis_collection[JoystickAxis.AxisPovY].buttonState + " frame:" + this.LastFrameNum);


						/////////////////////////////////   HANDLE AXIS in DIGITAL MANNER //////////////////////////////

						


						//only if there is no last dominant axis or  when last axis have been released you can search for new dominant axis
						//not reconginzed trigger axis can hangout this kind of soliving never go to Up
                    //if (axisDominantPrevInx < 0 || (axisDominantPrevInx > -1 && ((axisDetails = axis_collection [axisDominantPrevInx]).buttonState == ButtonState.PosToUp || axisDetails.buttonState == ButtonState.NegToUp))) {
								
								

								while (axis < num_axes) {

										if (axis < num_axes && (axisDetails = axis_collection [axis]) != null) {

												axisDetails = axis_collection [axis];
											

												if (axisDetails.buttonState == ButtonState.Down) {
														

														
							
														// index 6 and 7 are reserved for Pov Axes
														if (axis == 6) { //(int)JoystickAxis.AxisPovX
																if (axis_collection [axis].value > 0)
																		return InputCode.toCode ((Joysticks)Index, axis, JoystickPovPosition.Right);
																else
																		return InputCode.toCode ((Joysticks)Index, axis, JoystickPovPosition.Left);
														}
							
													
							
														if (axis == 7) {//(int)JoystickAxis.AxisPovY
								
																if (axis_collection [axis].value > 0)
																		return InputCode.toCode ((Joysticks)Index, axis, JoystickPovPosition.Forward);
																else
																		return InputCode.toCode ((Joysticks)Index, axis, JoystickPovPosition.Backward);
														}
							
														if (axis_collection [axis].value > 0)
																return InputCode.toCode ((Joysticks)Index, axis, (int)JoystickPosition.Positive);
														else
																return InputCode.toCode ((Joysticks)Index, axis, (int)JoystickPosition.Negative);







														
												}




										}

										axis++;
								
								}//end while

								

						//}
					
					
					
					
					
					
						// A discrete POV returns specific values for left, right, etc.
						// A continuous POV returns an integer indicating an angle in degrees * 100, e.g. 18000 == 180.00 degrees.
						// The vast majority of joysticks have discrete POVs, so we'll treat all of them as discrete for simplicity.
						//					           0(Forward)
						//						         |
						//							     |
						//				 (Left)27000----- ----9000(Right)
						//						         |
						//							     |
						//							   18000(Backward)
						
						
						
						/////TODO make possible any axes to be POV (add isHatFirstAxis)
						// axis_collection [JoystickAxis.AxisPovX].isHat


						///////////////////////////////////   HANDLE BUTTONS  //////////////////////////////

					
						int button = 0;

						////DEBUG
						//if (this is WiimoteDevice)
						//{
						//    UnityEngine.Debug.Log("GetInput took state:" + button_collection[button].buttonState);
						//    if (button_collection[button].buttonState == JoystickButtonState.Down)
						//        return KeyCodeExtension.toCode((Joysticks)ID, JoystickAxis.None, button);
						//}

						//UnityEngine.Debug.Log ("GetInput from J" + this.Index + " took state:" + button_collection [0].buttonState);


						while (button < _numButtons) {

                   

								if (button_collection [button].buttonState == ButtonState.Down)
										return InputCode.toCode ((Joysticks)Index, JoystickAxis.None, button);

								// UnityEngine.Debug.Log("AfterbuttonState " + button_collection[0]);

								button++;

						}//while buttons
		
						return 0;


				}

				public bool GetAnyInputDown ()
				{
						return this.GetInputCode () > 0;
						
				}

				public override bool Equals (object obj)
				{
						if (obj == null)
								return false;

						JoystickDevice objAsJoystickDevice = obj as JoystickDevice;
						if (objAsJoystickDevice == null)
								return false;
						else
								return this._index.Equals (objAsJoystickDevice.Index);
				}

				public override int GetHashCode ()
				{
						return this._index;
				}

			


		 
			
       

     

      

	
               
				public IDeviceExtension Extension { get; set; }

				public JoystickAxisCollection<IAxisDetails> Axis {
						get { return axis_collection; }
				}

				public JoystickButtonCollection<IButtonDetails> Buttons {
						get { return button_collection; }
				}

		#endregion





              
		}




    #region JoystickDevice<TExtension> : JoystickDevice

		// Provides platform-specific information about the relevant JoystickDevice.
//	public sealed class JoystickDevice<TAxisDetails,TButtonsDetails,TExtension>  : JoystickDevice<TAxisDetails,TButtonsDetails> 
//		where TExtension:IDeviceExtension
//			where TAxisDetails:IAxisDetails
//			where TButtonsDetails:IButtonDetails
//		{
//			internal JoystickDevice (int id, int axes, int buttons)
//            : base(id, axes, buttons)
//				{
//			       
//					
//				}
//
//				public TExtension Extension;
//
//				
//		}

    #endregion

		public enum Joysticks
		{
				
				Joystick0=0,
				Joystick1,
				Joystick2,
				Joystick3,
				Joystick4,
				Joystick5,
				Joystick6,
				Joystick7,
				Joystick8,
				Joystick9,
				Joystick10,
				Joystick11,
				Joystick12,
				Joystick13,
				Joystick14,
				//Joystick15,
				Joystick
		}




    

  
    



    #region JoystickAxis

		/// <summary>
		/// Defines available JoystickDevice axes.
		/// </summary>
		public enum JoystickAxis
		{
				/// <summary>The first axis of the JoystickDevice.</summary>
				AxisX=0,
				/// <summary>The second axis of the JoystickDevice.</summary>
				AxisY,
				/// <summary>The third axis of the JoystickDevice.</summary>
				AxisZ,
				/// <summary>The fourth axis of the JoystickDevice.</summary>
				AxisR,
				/// <summary>The fifth axis of the JoystickDevice.</summary>
				AxisU,
				/// <summary>The sixth axis of the JoystickDevice.</summary>
				AxisV,
				/// <summary>The seventh axis of the JoystickDevice.</summary>
				AxisPovX,
				/// <summary>The eighth axis of the JoystickDevice.</summary>
				AxisPovY,
				/// <summary>The nineth axis of the JoystickDevice.</summary>
				AxisAccX,
				/// <summary>The tenth axis of the JoystickDevice.</summary>
				AxisAccY,
				/// <summary>The eleventh axis of the JoystickDevice.</summary>
				AxisAccZ,
				/// <summary>The twelveth axis of the JoystickDevice.</summary>
				AxisAccR,
				/// <summary>The 13th axis of the JoystickDevice.</summary>
				AxisAccU,
				/// <summary>The 14th axis of the JoystickDevice.</summary>
				AxisAccV,
				None
        


       
		}

		public enum ButtonState:ushort
		{
				None=0,
				Up,
				PosToUp,
				NegToUp,
				Down,
				Hold
		}

		public enum JoystickPosition : ushort
		{
				Full = 0,
				Negative = 1,
				Positive = 2,
				

		}


		//TODO remove that 9000 factor shit

		public enum JoystickPovPosition : ushort
		{
				//Centered = 0xFFFF,/* x *///0xFF
				Forward = 3,//36000,/* 3 *///0x
				Right = 0,//9000,/* 0 */ // 0x04
				Backward =1,// 18000,/* 1 *///0x08
				Left = 2//27000/* 2 */ //0x40
		}

    #endregion


	#region JoystickButtonCollection
	
		/// <summary>
		/// Defines a collection of JoystickButtons.
		/// </summary>
		public sealed class JoystickButtonCollection<TButtonDetails> where TButtonDetails:IButtonDetails
		{
		#region Fields
		
				TButtonDetails[] details;
		
		#endregion
		
		#region Constructors
		
				internal JoystickButtonCollection (int numButtons)
				{
						details = new TButtonDetails[numButtons];
				}
		
		#endregion
		
		#region Public Members
		
				/// <summary>
				/// Gets a System.Boolean indicating whether the JoystickButton with the specified index is pressed.
				/// </summary>
				/// <param name="index">The index of the JoystickAxisAsButton to check.</param>
				/// <returns>True if the JoystickAxisAsButton is pressed; false otherwise.</returns>
				public TButtonDetails this [int index] {
						get { return details [index]; }
						internal set { details [index] = value; }
				}
		
		
		
				/// <summary>
				/// Gets a System.Int32 indicating the available amount of JoystickButton.
				/// </summary>
				public int Count {
						get { return details.Length; }
				}
		
		#endregion
		}
	
	#endregion



	


    #region JoystickAxisCollection

		/// <summary>
		/// Defines a collection of JoystickAxes.
		/// </summary>
		public sealed class JoystickAxisCollection<TAxisDetails> where TAxisDetails:IAxisDetails
		{
        #region Fields

				TAxisDetails[] details;

        #endregion

        #region Constructors

				internal JoystickAxisCollection (int numAxes)
				{
						if (numAxes < 0)
								throw new ArgumentOutOfRangeException ("numAxes");

						details = new TAxisDetails[numAxes];
				}


        #endregion

        #region Public Members

				
				public TAxisDetails this [int index] {
						get { return details [index]; }
						internal set { details [index] = value; }
				}
				
				public TAxisDetails this [JoystickAxis axis] {
						get { 
								return details [(int)axis]; 
						}
						internal set { details [(int)axis] = value; }
				}






				/// <summary>
				/// Gets a System.Int32 indicating the available amount of JoystickAxes.
				/// </summary>
				public int Count {
						get { return details.Length; }
				}

        #endregion


		}
	#endregion
	



		


	

   
}
#endregion
