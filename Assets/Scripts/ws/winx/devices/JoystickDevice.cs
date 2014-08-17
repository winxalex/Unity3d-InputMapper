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
				int _ID = -1;
				string _description;
				JoystickAxisCollection<IAxisDetails> axis_collection;
				JoystickButtonCollection<IButtonDetails> button_collection;
				int _numPOV=0;
		  		int _numButtons;
				int _numAxis;
				//bool _anyKeyDown = false;
				int _VID;
				int _PID;
                bool _isReady=true;
                int _lastFrameNum=-1;

        #endregion

        #region Constructors

				internal JoystickDevice (int id,int pid,int vid,int axes, int buttons,IDriver driver)
				{
						if (axes < 0)
								throw new ArgumentOutOfRangeException ("axes");

						if (buttons < 0)
								throw new ArgumentOutOfRangeException ("buttons");

			        if(axes<8 && numPOV>0){
						throw new Exception("POV X and POV Y are 7th and 8th Axis default driver so min axes is 8");
					}
			     

						_numAxis=axes;
						_numButtons=buttons;

                       

						_ID = id;
                        _VID = vid;
                        _PID = pid;
                        _driver = driver;
						axis_collection = new JoystickAxisCollection<IAxisDetails> (axes);
						button_collection = new JoystickButtonCollection<IButtonDetails> (buttons);
			            
           
				}

        #endregion

        #region Public Members

		#region IDevice implementation


                public bool isReady
                {
                    get
                    {
                        return _isReady;
                    }
                   internal set
                    {
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
			set {
				_PID=value;
			}
		}

		public int VID {
			get {
				return _VID;
			}
			set {
				_VID=value;
			}
		}

     
	
        /// <summary>
        /// ID is value 0 to 15 and given by driver
        /// </summary>
		public int ID {
			get { return _ID; }
			set { _ID = value; }
		}



        public void Update()
        {

//
            if(this.ID==1)
            UnityEngine.Debug.Log("Update"+this.ID+" Current frame:"+Time.frameCount+" lastFrame:"+_lastFrameNum);
           
            //lock is done so no other updates of joystick is done while frame
            //many Joy.GetXXX could be called in same frame so updates would make JosytickDevice values unusable
                if (_lastFrameNum == Time.frameCount)
                {
                    if(this.ID==1)
                   UnityEngine.Debug.Log("skip" + this.ID + " cos its same frame");
                    return;
                }
                else
                {
                   if(this.ID==1)
                    UnityEngine.Debug.Log("Update" + this.ID + " Current frame:" + Time.frameCount + " lastFrame:" + _lastFrameNum);
                 
                    _lastFrameNum = Time.frameCount;
                    _driver.Update(this);// as IDevice<IAxisDetails,IButtonDetails,IDeviceExtension>);
                }
         
          

			//_driver.Update(this);

        }

			
				/// <summary>
				/// Gets the axis value.
				/// </summary>
				/// <returns>The axis.</returns>
				/// <param name="code">Code. joystick(4bits) + axis(4bits) + data(+/- 5bits)</param>
				public float GetAxis (int code)
				{
                        Update();

						JoystickAxis axis = KeyCodeExtension.toAxis(code);
						int data = KeyCodeExtension.toData (code);
						float axisValue = axis_collection [axis].value;

                        // axis_collection [axis].isHat
						if (axis == JoystickAxis.AxisPovX) {
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Left && axisValue < 0)
										return axisValue;
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Right && axisValue > 0)
										return axisValue;
							
								return 0;
							
						}
						
						
						if (axis == JoystickAxis.AxisPovY) {
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Backward && axisValue < 0)
										return axisValue;
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Forward && axisValue > 0)
										return axisValue;
								return 0;
						}
						
						
						
						if (data == (int)JoystickPosition.Negative && axisValue < 0)
								return axisValue;
						if (data == (int)JoystickPosition.Positive && axisValue > 0)
								return axisValue;


						return 0;
				}

				/// <summary>
				/// returns true if the key/axisAsKey is pressed
				/// </summary>
				/// <returns><c>true</c>, if key was gotten, <c>false</c> otherwise.</returns>
				/// <param name="code">Code. joystick(4bits) + axis(4bits) + data(+/- 5bits);</param>
				public virtual bool GetKey (int code)
				{
						Update();



						JoystickAxis axis = KeyCodeExtension.toAxis (code);
						int data = KeyCodeExtension.toData (code);
						float axisValue;



						//if its a button code
						if (axis == JoystickAxis.None) 
								return button_collection [data].buttonState == JoystickButtonState.Hold;// || button_collection[data]==JoystickButtonState.Down;
				 

						axisValue = axis_collection [axis].value;

						if (axis == JoystickAxis.AxisPovX) {
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Left && axisValue < 0)
										return true;
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Right && axisValue > 0)
										return true;


								return false;
				
						}


						if (axis == JoystickAxis.AxisPovY) {
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Backward && axisValue < 0)
										return true;
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Forward && axisValue > 0)
										return true;
								return false;
						}
						
		//	Debug.Log("GetKey axis:"+axis+" JOY:"+KeyCodeExtension.toJoystickID(code)+" ID:"+this.ID+" data:"+data+" axisValue= "+axisValue);

						//if (data == (int)JoystickPosition.Negative && axisAsButton_collection[axis]==JoystickButtonState.Hold)
						if (data == (int)JoystickPosition.Negative && axisValue < 0)
								return true;
						if (data == (int)JoystickPosition.Positive && axisValue > 0)
								return true;


						return false;
				}


				/// <summary>
				/// Gets the key up.
				/// </summary>
				/// <returns><c>true</c>, if key up was gotten, <c>false</c> otherwise.</returns>
				/// <param name="code">Code.</param>
				public virtual  bool GetKeyUp (int code)
				{
						Update();// 

                       // UnityEngine.Debug.Log("GetKeyUP");


						JoystickAxis axis = KeyCodeExtension.toAxis (code);
						int data = KeyCodeExtension.toData (code);
                    
						if (axis == JoystickAxis.None)
								return button_collection [data].buttonState == JoystickButtonState.Up;
								
						IAxisDetails axisDetails=axis_collection [axis];

						if (axis == JoystickAxis.AxisPovX) {
						if ((data + 1) * 9000 == (int)JoystickPovPosition.Left && axisDetails.buttonState == JoystickButtonState.NegToUp)
										return true;
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Right && axisDetails.buttonState == JoystickButtonState.PosToUp)
										return true;
								return false;
						}

						if (axis == JoystickAxis.AxisPovY) {
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Backward && axisDetails.buttonState == JoystickButtonState.NegToUp)
										return true;
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Forward && axisDetails.buttonState == JoystickButtonState.PosToUp)
										return true;
								return false;
						}

						//check if the axis moved in Negative values was relesed
						if (data == (int)JoystickPosition.Negative && axisDetails.buttonState == JoystickButtonState.NegToUp)
								return true;
						if (data == (int)JoystickPosition.Positive && axisDetails.buttonState == JoystickButtonState.PosToUp)
								return true; 

						return axisDetails.buttonState == JoystickButtonState.Up;
				}

				/// <summary>
				/// Gets the key down.
				/// </summary>
				/// <returns><c>true</c>, if key down was gotten, <c>false</c> otherwise.</returns>
				/// <param name="code">Code.</param>
				public virtual bool GetKeyDown (int code)
				{
                    Update();

                   // if (!this.isReady) return false;

                    //UnityEngine.Debug.Log("GetKeyDown Joy"+this.ID);

						JoystickAxis axis = KeyCodeExtension.toAxis (code);
						int data = KeyCodeExtension.toData (code);

                        if (axis == JoystickAxis.None)   //MO data for axis => buttons data
                        {
                            //UnityEngine.Debug.Log("Button state>" + button_collection[data].buttonState);
                         
                            return button_collection[data].buttonState == JoystickButtonState.Down;

                        }
						
							IAxisDetails axisDetails=axis_collection[axis];

						if (axis == JoystickAxis.AxisPovX) {
				if ((data + 1) * 9000 == (int)JoystickPovPosition.Left && axisDetails.buttonState == JoystickButtonState.Down && axisDetails.value < 0)
										return true;
				if ((data + 1) * 9000 == (int)JoystickPovPosition.Right && axisDetails.buttonState == JoystickButtonState.Down && axisDetails.value > 0)
										return true;

								return false;
				
						}


						if (axis == JoystickAxis.AxisPovY) {
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Backward && axisDetails.buttonState == JoystickButtonState.Down && axisDetails.value < 0)
										return true;
								if ((data + 1) * 9000 == (int)JoystickPovPosition.Forward && axisDetails.buttonState == JoystickButtonState.Down && axisDetails.value > 0)
										return true;

								return false;
				
						}


						if (data == (int)JoystickPosition.Negative && axisDetails.buttonState == JoystickButtonState.Down && axisDetails.value < 0)
								return true;
						if (data == (int)JoystickPosition.Positive && axisDetails.buttonState == JoystickButtonState.Down && axisDetails.value > 0)
								return true;
			
			
						return false;
				}


		/// <summary>
		/// Gets the input.
        /// IMPORTANT: If button or axis is already "DOWN" returns 0
		/// </summary>
		/// <returns>The input.</returns>
		public virtual int GetInput(){

            UnityEngine.Debug.Log("GetInput:" + Time.frameCount);

            //there is no  Time.frameCount in Editor (edit mode - editor scripts)
            if(Application.isPlaying) 
            {
                Update();//prevents multiply update in frame
                //if(Application.isWebPlayer && !this.isReady) retr


                
            }
            else  
                _driver.Update(this);
            

          
					int num_axes = _numAxis;
						
					int axis = 0;
					float joyValue=0f;
					float joyValueMax = 0f;
					
					IAxisDetails axisDetails;
					
//            if (axis < num_axes && (axisDetails=axis_collection[axis])!=null ) { 
						
//                        axisDetails=axis_collection[axis];
//                        joyValue = axisDetails.value;
						
						
												
//                        joyValue = joyValue < 0 ? -joyValue : joyValue;//abs
						
//                        if (axisDetails.buttonState == JoystickButtonState.Down) {
//                            joyValueMax = joyValue;
//                            dominantAxis = JoystickAxis.AxisX;
//                        }
						
//                        //UnityEngine.Debug.Log("X "+joyValue+stick.AxisAsButtons [JoystickAxis.AxisY]);
						
						
						
//                        axis++; 
//                    }


////			axisDetails=axis_collection[JoystickAxis.AxisY];
////			joyValue = axisDetails.value;
////			UnityEngine.Debug.Log("joyY"+joyValue+"max"+joyValueMax);
////
////			axisDetails=axis_collection[JoystickAxis.AxisZ];
////			joyValue = axisDetails.value;
////			UnityEngine.Debug.Log("joyZ"+joyValue+"max"+joyValueMax);



					
//            if (axis < num_axes && (axisDetails=axis_collection[axis])!=null ) { 
						
//                        axisDetails=axis_collection[axis];
//                        joyValue = axisDetails.value;

//                        joyValue = joyValue < 0 ? -joyValue : joyValue;//abs
						
//                        if (axisDetails.buttonState == JoystickButtonState.Down)
//                        if (joyValueMax < joyValue) {
//                            joyValueMax = joyValue;
//                            dominantAxis = JoystickAxis.AxisY;
//                        }
						
//                        //UnityEngine.Debug.Log("Y "+joyValue+stick.AxisAsButtons [JoystickAxis.AxisY]);
						
//                        axis++; 
//                    }
					
					
//            if (axis < num_axes && (axisDetails=axis_collection[axis])!=null ) { 
						
//                            axisDetails=axis_collection[axis];
//                            joyValue = axisDetails.value;
						
							
//                        joyValue = joyValue < 0 ? -joyValue : joyValue;//abs
						
//                        if (axisDetails.buttonState == JoystickButtonState.Down)
//                        if (joyValueMax < joyValue) {
//                            joyValueMax = joyValue;
//                            dominantAxis = JoystickAxis.AxisZ;
//                        }
						
						
						
//                        axis++; 
//                    }
					
					
					
//            if (axis < num_axes && (axisDetails=axis_collection[axis])!=null ) { 
						
//                        axisDetails=axis_collection[axis];
//                        joyValue = axisDetails.value;
						
//                        joyValue = joyValue < 0 ? -joyValue : joyValue;//abs
						
//                        if (axisDetails.buttonState == JoystickButtonState.Down)
//                        if (joyValueMax < joyValue) {
//                            joyValueMax = joyValue;
//                            dominantAxis = JoystickAxis.AxisR;
//                        }
						
						
						
//                        axis++; 
//                    }
					
					
//            if (axis < num_axes && (axisDetails=axis_collection[axis])!=null ) { 
						
//                        axisDetails=axis_collection[axis];
//                        joyValue = axisDetails.value;
						
//                        joyValue = joyValue < 0 ? -joyValue : joyValue;//abs
						
//                        if (axisDetails.buttonState == JoystickButtonState.Down)
//                        if (joyValueMax < joyValue) {
//                            joyValueMax = joyValue;
//                            dominantAxis = JoystickAxis.AxisU;
//                        }
						
						
//                        axis++; 
//                    }
					
					
//            if (axis < num_axes && (axisDetails=axis_collection[axis])!=null ) { 
						
//                            axisDetails=axis_collection[axis];
//                            joyValue = axisDetails.value;
									
//                                    joyValue = joyValue < 0 ? -joyValue : joyValue;//abs
									
//                                    if (axisDetails.buttonState == JoystickButtonState.Down)
//                                    if (joyValueMax < joyValue) {
//                                        joyValueMax = joyValue;
//                                        dominantAxis = JoystickAxis.AxisV;
//                                    }
						
						
//                        axis++; 
//                    }

            int dominantAxisInx = -1;
					//TODO loop thru all
            while (axis < num_axes)
            {

                if (axis < num_axes && (axisDetails = axis_collection[axis]) != null)
                {

                    axisDetails = axis_collection[axis];
                    joyValue = axisDetails.value;

                    joyValue = joyValue < 0 ? -joyValue : joyValue;//abs

                    if (axisDetails.buttonState == JoystickButtonState.Down)
                        if (joyValueMax < joyValue)
                        {
                            joyValueMax = joyValue;
                           // dominantAxis = JoystickAxis.AxisV;
                            dominantAxisInx = axis;
                        }


                    
                }

                axis++;
            }


           // UnityEngine.Debug.Log(dominantAxisInx);
					if(dominantAxisInx>-1){
					//if (dominantAxis != JoystickAxis.None) {

                        //UnityEngine.Debug.Log("Count" + axis_collection.Count);
                       // UnityEngine.Debug.Log("dominantAxis "+dominantAxis+" state"+axis_collection[dominantAxis].buttonState);
						
						//stick.AxisAsButtons [dominantAxis] != JoystickButtonState.Hold;

                        // index 6 and 7 are reserved for Pov Axes
                        if (dominantAxisInx ==6) //(int)JoystickAxis.AxisPovX
                        {
                            if (axis_collection[dominantAxisInx].value > 0)
                                return KeyCodeExtension.toCode((Joysticks)ID, dominantAxisInx, JoystickPovPosition.Right);
                            else
                                return KeyCodeExtension.toCode((Joysticks)ID, dominantAxisInx, JoystickPovPosition.Left);
                        }

                        //if (axis_collection[JoystickAxis.AxisPovX].buttonState == JoystickButtonState.Down)
                        //{

                        if (dominantAxisInx == 7)//(int)JoystickAxis.AxisPovY
                        {

                            if (axis_collection[dominantAxisInx].value > 0)
                                return KeyCodeExtension.toCode((Joysticks)ID, dominantAxisInx, JoystickPovPosition.Forward);
                                else
                                return KeyCodeExtension.toCode((Joysticks)ID, dominantAxisInx, JoystickPovPosition.Backward);
                        }
                  
                        if (axis_collection[dominantAxisInx].value > 0)
                            return KeyCodeExtension.toCode((Joysticks)ID, dominantAxisInx, (int)JoystickPosition.Positive);
						else
							return KeyCodeExtension.toCode ((Joysticks)ID, dominantAxisInx, (int)JoystickPosition.Negative);
                        
						
                        //if (axis_collection[dominantAxis].value > 0)
                        //    return KeyCodeExtension.toCode ((Joysticks)ID, dominantAxis, (int)JoystickPosition.Positive);
                        //else
                        //    return KeyCodeExtension.toCode ((Joysticks)ID, dominantAxis, (int)JoystickPosition.Negative);
						
					}
					
					
					
					
					
					
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

                    
                        //if(num_axes<axis)
                        //if (axis_collection[JoystickAxis.AxisPovX].buttonState == JoystickButtonState.Down)
                        //{
                        //    if (axis_collection[JoystickAxis.AxisPovX].value > 0)
                        //        return KeyCodeExtension.toCode((Joysticks)ID, JoystickAxis.AxisPovX, JoystickPovPosition.Right);
                        //    else
                        //        return KeyCodeExtension.toCode((Joysticks)ID, JoystickAxis.AxisPovX, JoystickPovPosition.Left);
                        //}

                        //axis++;

                        //if(num_axes<axis)
                        //if (axis_collection[JoystickAxis.AxisPovY].buttonState == JoystickButtonState.Down)
                        //{
                        //    if (axis_collection[JoystickAxis.AxisPovY].value > 0)
                        //        return KeyCodeExtension.toCode((Joysticks)ID, JoystickAxis.AxisPovY, JoystickPovPosition.Forward);
                        //    else
                        //        return KeyCodeExtension.toCode((Joysticks)ID, JoystickAxis.AxisPovY, JoystickPovPosition.Backward);
                        //}
                    

					
					int button = 0;

                   // UnityEngine.Debug.Log("GetInput took state:"+button_collection[button].buttonState);
                   // if (button_collection[button].buttonState == JoystickButtonState.Down)
                    //            return KeyCodeExtension.toCode ((Joysticks)ID, JoystickAxis.None, button);



                    while (button < _numButtons)
                    {//) {

                        //stick.SetButton (button, (info.Buttons & (1 << button)) != 0);

                        if (button_collection[button].buttonState == JoystickButtonState.Down)
                            return KeyCodeExtension.toCode((Joysticks)ID, JoystickAxis.None, button);

                        // UnityEngine.Debug.Log("AfterbuttonState " + button_collection[0]);

                        button++;

                    }//while buttons
		
			return 0;


		}

				public bool GetAnyKeyDown ()
				{
					return this.GetInput()>0;
						
				}

				

				public override bool Equals (object obj)
				{
                    if (obj == null)
                        return false;

			            JoystickDevice objAsJoystickDevice = obj as JoystickDevice;
						if (objAsJoystickDevice == null)
								return false;
						else
								return this._ID.Equals (objAsJoystickDevice.ID);
				}

				public override int GetHashCode ()
				{
						return this._ID;
				}

			


		 
				/// <summary>
				/// Tos the nearest orto angle.
				/// </summary>
				/// <returns>The nearest orto angle.</returns>
				/// <param name="pov">Pov.</param>
				internal static JoystickPovPosition toNearestOrtoAngle (int pov)
				{
						JoystickPovPosition nearestOrtoAngle;
						int difference;
						int differenceMin;
			
			
			
						//init
						differenceMin = pov - (int)JoystickPovPosition.Right;
						//(x ^ (x >> 31)) - (x >> 31);
						differenceMin = (differenceMin ^ (differenceMin >> 31)) - (differenceMin >> 31);
						nearestOrtoAngle = JoystickPovPosition.Right;
			
			
			
			
						difference = pov - (int)JoystickPovPosition.Backward;
						difference = (difference ^ (difference >> 31)) - (difference >> 31);
			
			
			
			
						if (difference < differenceMin) {
								differenceMin = difference;
								nearestOrtoAngle = JoystickPovPosition.Backward;
						}
			
						if (pov > (int)JoystickPovPosition.Left) {
								difference = pov - (int)JoystickPovPosition.Forward;
								difference = (difference ^ (difference >> 31)) - (difference >> 31);
						} else {
								difference = pov;
						}
			
						if (difference < differenceMin) {
								differenceMin = difference;
								nearestOrtoAngle = JoystickPovPosition.Forward;
						}
			
			
						difference = pov - (int)JoystickPovPosition.Left;
						difference = (difference ^ (difference >> 31)) - (difference >> 31);
			
			
						if (difference < differenceMin) {
								differenceMin = difference;
								nearestOrtoAngle = JoystickPovPosition.Left;
						}
			
						difference = pov - (int)JoystickPovPosition.Forward;
						difference = (difference ^ (difference >> 31)) - (difference >> 31);
			
			
						if (difference < differenceMin) {
								differenceMin = difference;
								nearestOrtoAngle = JoystickPovPosition.Forward;
						}

						return nearestOrtoAngle;
				}

				

				


       #endregion

       

     

      

	
               
                public IDeviceExtension Extension { get; set; }
               

                public JoystickAxisCollection<IAxisDetails> Axis
                {
                    get { return axis_collection; }
                }

                public JoystickButtonCollection<IButtonDetails> Buttons
                {
                    get { return button_collection; }
                }







              
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
				//Joystick=-1,
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
				Joystick15,
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

		public enum JoystickButtonState:ushort
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
				Negative = 0,
				Positive = 1

		}

		public enum JoystickPovPosition : ushort
		{
				Centered = 0xFFFF,/* x */
				Forward = 36000,/* 3 */
				Right = 9000,/* 0 */
				Backward = 18000,/* 1 */
				Left = 27000/* 2 */
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
