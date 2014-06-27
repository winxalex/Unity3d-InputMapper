using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization;
using ws.winx.devices;

namespace ws.winx.input
{

	[DataContract]
    public class InputCombination
    {


		public delegate bool InputDelegate(InputAction action);
		
		
		
		protected List<InputAction> _actionsList;
		protected String _combinationString;
		protected List<InputAction>.Enumerator _pointer;
		protected float _analogValue=0f;
		protected float _timeDelta;
		protected bool _isActive=false;


		private InputAction __currentInputAction;
		private float __lastInputTime;
		private float __range;
		private KeyCode __lastCode;


		[DataMember(Name = "InputActions")]
		public List<InputAction> actions{
			get{return _actionsList;}
			set{ 
				_actionsList=value; 
				_combinationString=ToString(value);
				initPointer();

			}
		}

		public int numActions{
			get{
				return _actionsList.Count;
			}
		}
		
		public bool isActive
		{
			get{ return _isActive; }
			set {  _isActive =value; }
		}
		
		
		public String combinationString
		{
			get{ 
				if(_combinationString==null) _combinationString=ToString(_actionsList);
				return _combinationString; 
			}
			set {  _combinationString =value; 
					_actionsList.Clear();
					parse(combinationString);
			}
		}



		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.input.InputCombination"/> class.
		/// </summary>
		/// <param name="codes">Codes.</param>
		public  InputCombination(params KeyCode[] codes){
			_actionsList=new List<InputAction>();
			for(int i=0;i<codes.Length;i++)
				_actionsList.Add(new InputAction(codes[i]));
			
			initPointer();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.input.InputCombination"/> class.
		/// </summary>
		/// <param name="codes">Codes ex. (int)KeyCode.P,(int)Joystick2Button12,InputCodes.toCode (Joysticks joy,JoystickAxis axis,JoystickPosition pos),...</param>
		public  InputCombination(params int[] codes){
			//_actionsList=codes.Select(entry => new InputAction(entry)).ToList();

			_actionsList=new List<InputAction>();
			for(int i=0;i<codes.Length;i++)
				_actionsList.Add(new InputAction(codes[i]));
			
			initPointer();
			
			
		}



		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.input.InputCombination"/> class.
		/// </summary>
		/// <param name="combinations">Combinations.</param>
		public  InputCombination(List<InputAction> combinations){
			_actionsList=combinations;


			initPointer();


		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.input.InputCombination"/> class.
		/// </summary>
		/// <param name="combinationString">Combination string.</param>
		public  InputCombination(String combinationString){
			_actionsList=new List<InputAction>();

			this.combinationString=combinationString;


			initPointer();

		}


		public InputAction GetActionAt(int inx){
			return _actionsList.ElementAt(inx);
		}


		public void Add(InputAction action){
			_actionsList.Add(action);
			_combinationString=ToString(_actionsList);
		}

		public void Clear(){
			_actionsList.Clear();
			_combinationString=String.Empty;
		}

		public bool Pop(){
			return _actionsList.Remove(_actionsList.Last());
		}

	

		
		
		protected bool GetInputBase(InputDelegate keyCallback,bool atOnce=false){
			if(__currentInputAction.type==InputActionType.SINGLE && _actionsList.Count==1){
				
				return keyCallback(__currentInputAction);
				
			}else{
				if(atOnce)
					return GetCombinationInput(keyCallback);
				else
					return GetCombinationInput();
			}

		}
		
		
		
		internal bool GetInput(bool atOnce){
				 return GetInputBase(InputEx.GetKey,atOnce); 
    	}
		
		
		
		internal bool GetInputUp(bool atOnce){

			return GetInputBase(InputEx.GetKeyUp,atOnce);

		}
		
		internal bool GetInputDown(bool atOnce){
		      return GetInputBase(InputEx.GetKeyDown,atOnce);
		}
		

		internal bool GetCombinationInput(InputDelegate keyCallback){
			_pointer=_actionsList.GetEnumerator();
			_pointer.MoveNext();
			
			do{
				if(keyCallback(_pointer.Current)==false) return false;
				
			}while(_pointer.MoveNext()!=false);
			
			
			
			return true;
		}
		
		internal bool GetCombinationInput()
		{


							bool isDown=InputEx.GetKeyDown(_pointer.Current);
							InputAction action;
							int code=0;


							if(isDown) //return false;
							{
								code=_pointer.Current.code;
								//UnityEngine.Debug.Log("Code:"+code);
							}

							
				//UnityEngine.Debug.Log("Code:"+code);

							//action= InputEx.GetInput();//
				//if this was last code in combination


				    //if current action code isDown and has type=single
				if((isDown && _pointer.Current.type==InputActionType.SINGLE)
				   //process code ("Q(-)","W(x2)"...same with joys.. and if code and type are ok go in
				   || ((action=InputEx.processInput(code,Time.time))!=null
							&& action.code==_pointer.Current.code && action.type==_pointer.Current.type)){
	//				UnityEngine.Debug.Log("CODE:"+_pointer.Current.codeString+" Action:"+action);

							   //save time when action happened if not saved or reseted
								if(__lastInputTime==0){__lastInputTime=Time.time;}


						        
								
								if(Time.time<__lastInputTime+InputAction.COMBINATION_CLICK_SENSITIVITY){

								   //get the time when current action of combination happened
									__lastInputTime=Time.time;

									//just move to next if possible or reset if couldn't
									if(!_pointer.MoveNext()){
										_pointer=_actionsList.GetEnumerator();//Reset pointer
										_pointer.MoveNext();//start from beginin
										return true;
									}
								}else{//reset cos time has passed for next action
									_pointer=_actionsList.GetEnumerator();//Reset pointer
									_pointer.MoveNext();//start from beginin
									__lastInputTime=0;

	//					UnityEngine.Debug.Log("Reset Time Cos Time Pass:"+Time.time+" Time Allowed:"+(__lastInputTime+InputAction.COMBINATION_CLICK_SENSITIVITY));

								}


//								UnityEngine.Debug.Log("CodeAfter:"+_pointer.Current.codeString);

					return false;
							}
						//UnityEngine.Debug.Log("CodeAfter New Between Code or not same type:"+_pointer.Current.codeString);
			
						if((__lastInputTime>0 && Time.time>__lastInputTime+InputAction.COMBINATION_CLICK_SENSITIVITY)
				   ||  (InputEx.anyKeyDown && code==0)){

//						UnityEngine.Debug.Log("Reset in Idle or another key pressed"+Time.time+" Time Allowed:"+(__lastInputTime+InputAction.COMBINATION_CLICK_SENSITIVITY)+"code:"+code);

						_pointer=_actionsList.GetEnumerator();//Reset pointer
						_pointer.MoveNext();//start from beginin
						__lastInputTime=0;
				}


		
			
			return false;
			
		}


		internal float GetAxis(float sensitivity,float dreadzone,float gravity){
			if(_actionsList.Count>1) return 0;

			if(__currentInputAction.code<KeyCodeExtension.MAX_KEY_CODE || KeyCodeExtension.toAxis(__currentInputAction.code)==JoystickAxis.None){//if keys are used as axis
				return GetVirtualAxis(sensitivity,dreadzone,gravity);
			}

			//TODO made dreadzone rounding to axis
			return InputEx.GetAxis(__currentInputAction);

		}
		
		
		internal float GetVirtualAxis(float sensitivity,float dreadzone,float gravity){
			

			if (InputEx.GetKey(__currentInputAction))
			{
				_isActive=true;
				_timeDelta += Time.deltaTime * sensitivity;
				
				//timeDelta need to go from 0 to 1  (which mean from 0 to 100% of range difference)
				_analogValue = Mathf.Lerp(0, 1, Mathf.Clamp01(_timeDelta));
				
				
				if(_analogValue<dreadzone)
				{
					_analogValue=0;
				}
				
			
				
			}else{ //on KeyUp reset _timeDelta
				if(InputEx.GetKeyUp(__currentInputAction)) 
				{
					_isActive=false;
					_timeDelta = 0f;//reset
					
					if(!(gravity>0))_analogValue=0;
					
					return _analogValue;
				}

				
				//effect of gravity
				if(_analogValue != 0){
					
					_timeDelta += Time.deltaTime * gravity;
					_analogValue = Mathf.Lerp(_analogValue, 0, Mathf.Clamp01(_timeDelta));
				}
			}
			
			
			
			
			return _analogValue;
			
			
		}



		public void reset(){
			_timeDelta=0f;
			_analogValue=0f;
			_isActive=false;
		}
		
		
		public InputCombination Clone ()
		{
			return new InputCombination(_combinationString);
		}


		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="ws.winx.input.InputCombination"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="ws.winx.input.InputCombination"/>.</returns>
		public override string ToString ()
		{
			return combinationString;
		}
		
		
		/// <summary>
		/// Inits the pointer.
		/// </summary>
		protected void initPointer(){
			_pointer=_actionsList.GetEnumerator();
			
			_pointer.MoveNext();
			__currentInputAction=_pointer.Current;
		}


		/// <summary>
		/// Tos the string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="list">List.</param>
		protected string ToString(List<InputAction> list){
			StringBuilder strBuilder=new StringBuilder();
			List<InputAction>.Enumerator ptr=list.GetEnumerator();//Reset pointer
			
			
			while(ptr.MoveNext()){
				strBuilder.Append(ptr.Current.ToString()+InputAction.SPACE_DESIGNATOR);
			}
			
			strBuilder.Remove(strBuilder.Length-1,1);
			
			return strBuilder.ToString();
		}
		
		/// <summary>
		/// Parse the specified combinationString.
		/// </summary>
		/// <param name="combinationString">Combination string.</param>
		protected void parse(String combinationString){
			
			int len=combinationString.Length;
			int inx=0;
			int lastInx=0;
			
			if((inx=combinationString.IndexOf(InputAction.SPACE_DESIGNATOR,inx))>-1){
				
				_actionsList.Add(new InputAction(combinationString.Substring(lastInx,inx-lastInx)));
				lastInx=++inx;
				
				while(inx <len)
				{
					if((inx=combinationString.IndexOf(InputAction.SPACE_DESIGNATOR,inx))>-1){
						_actionsList.Add(new InputAction(combinationString.Substring(lastInx,inx-lastInx)));
						lastInx=++inx;
					}else{
						_actionsList.Add(new InputAction(combinationString.Substring(lastInx,len-lastInx)));
						break;
					}
					
				}
			}
			else{
				_actionsList.Add(new InputAction(combinationString));
			}
			
			

			
		}

       
    }
}
