using UnityEngine;
using System.Collections;
using ws.winx.input;
using ws.winx.input.states;
using System;

public class PlayerInputComponent : MonoBehaviour
{

		

	public InputPlayer.Player Player;

		// Use this for initialization
		void Start ()
		{
			
		}



	void manuallyAddStateAndHandlers()
	{
		
		
		
		//   UnityEngine.Debug.Log(InputManager.Log());
		
		//		adding input-states pairs manually
		//			InputManager.MapStateToInput("My State1",new InputCombination(InputCode.toCode(Joysticks.Joystick1,JoystickAxis.AxisPovX,JoystickPovPosition.Forward),(int)KeyCode.Joystick4Button9,(int)KeyCode.P,(int)KeyCode.JoystickButton0));
		//			InputManager.MapStateToInput("My State2",new InputCombination(KeyCode.Joystick4Button9,KeyCode.P,KeyCode.JoystickButton0));
		//			InputManager.MapStateToInput("My State3",new InputCombination("A(x2)+Mouse1+JoystickButton31"));
		//			InputManager.MapStateToInput("My State1",new InputCombination("Mouse1+Joystick12AxisXPositive(x2)+B"));
		
		
		
		////easiest way to map state to combination (ex.of single W and C click)
		if (!InputManager.HasInputState ("ManualFullAxisMap"))
			InputManager.MapStateToInput ("ManualFullAxisMap", InputCode.Joystick0AxisX);// InputCode.W.SINGLE, InputCode.C.SINGLE);
		
		
		//			InputManager.MapStateToInput ("WalkForward", InputCode.W.SINGLE);
		//			InputManager.MapStateToInput ("WalkForward", 1, InputCode.Joystick1AxisXPositive.SINGLE);
		//			
		//			
		//			InputManager.MapStateToInput ("WalkBackward", InputCode.S.SINGLE);
		//			InputManager.MapStateToInput ("WalkBackward", 1, InputCode.Joystick1AxisYNegative.SINGLE);
		
		UnityEngine.Debug.Log("Log:" + InputManager.Log());
		
		
		////Event Based input handling
		InputEvent ev = new InputEvent("ManualFullAxisMap");
		//InputEvent ev = new InputEvent((int)States.SomeState);
		
		
		ev.UP += new EventHandler(onUp);
		ev.DOWN += new EventHandler(onDown);
		
		
		
		
		
	}


	void onUp(object o, EventArgs args)
	{
		Debug.Log("Up");
	}
	
	void onDown(object o, EventArgs args)
	{
		Debug.Log("Down");
	}
	
	void Handle1(object o, EventArgs args)
	{
		Debug.Log("Handle1");
	}
	
	void Handle2(object o, EventArgs args)
	{
		Debug.Log("Handle2");
	}
	
	// Update is called once per frame
		void Update ()
		{
		    ///
			//InputManager.Settings

		    
			//InputManager.currentPlayerIndex = PlayerIndex;
		InputManager.currentPlayerIndex = Player;


			if (InputManager.GetInputDown((int)States.Wave)) {
				this.GetComponent<Animator>().Play("Wave");
			}
			
			//	
			//
//			if (InputManager.GetInputHold (Animator.StringToHash ("WalkBackward"))) {
//				Debug.Log ("WalkBackward-Hold");
//			}
//			
//			
//			if (InputManager.GetInputDown (Animator.StringToHash ("WalkBackward"))) {
//				Debug.Log ("WalkBackward-Down");
//			}
//			
//			if (InputManager.GetInputUp (Animator.StringToHash ("WalkBackward"))) {
//				Debug.Log ("WalkBackward-Up");
//			}


				//Generated value from -1 to 1f
				//			float axisPos = InputManager.GetInputRaw (Animator.StringToHash ("WalkForward"), 0.3f, 0.1f, 0.2f);
				
				//Generated value from 0 to 1f
				//						float axisPos = InputManager.GetInput (Animator.StringToHash ("WalkForward"), 0.3f, 0.1f, 0.2f);
				//			
				//						float axisNeg= InputManager.GetInput (Animator.StringToHash ("WalkBackward"),  0.3f, 0.1f, 0.1f);
				//			
				//						float analogVal=axisPos - axisNeg;
				
				//Debug.Log (analogVal);//would go from  -1 to 1
				//			Debug.Log (axisPos);
				
				
				
				
				
				
				//			float analogVal2= InputManager.GetInput (Animator.StringToHash ("ManualFullAxisMap"));
				//			Debug.Log (analogVal2);





		}
}

