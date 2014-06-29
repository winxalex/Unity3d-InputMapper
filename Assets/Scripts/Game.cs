using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using ws.winx.input;
using System.Runtime.Serialization;
using System.Linq;
using ws.winx.platform.windows;
using ws.winx.devices;
using ws.winx.input.states;

namespace ws.winx{
public class Game : MonoBehaviour {

// What if you have 50 states. Would you need to define them manually. => Forget it about this
//		static int idleState = Animator.StringToHash("Base Layer.Idle");	
//		static int locoState = Animator.StringToHash("Base Layer.Locomotion");			// these integers are references to our animator's states
//		static int jumpState = Animator.StringToHash("Base Layer.Jump");				// and are used to check state for various actions to occur
//		static int jumpDownState = Animator.StringToHash("Base Layer.JumpDown");		// within our FixedUpdate() function below
//		static int fallState = Animator.StringToHash("Base Layer.Fall");
//		static int rollState = Animator.StringToHash("Base Layer.Roll");
//		static int waveState = Animator.StringToHash("Layer2.Wave");

		public Animator animator;



	// Use this for initialization
	void Start () {

		
			//supporting devices with custom drivers
			InputManager.AddDriver(new XInputDriver());

			//adding input-states pairs manually
//			InputManager.AddStateInput("My State1",new InputCombination(KeyCodeExtension.toCode(Joysticks.Joystick1,JoystickAxis.AxisPovX,JoystickPovPosition.Forward),(int)KeyCode.Joystick4Button9,(int)KeyCode.P,(int)KeyCode.JoystickButton0));
//			InputManager.AddStateInput("My State2",new InputCombination(KeyCode.Joystick4Button9,KeyCode.P,KeyCode.JoystickButton0));
//			InputManager.AddStateInput("My State3",new InputCombination("A(x2)+Mouse1+JoystickButton31"));
//			InputManager.AddStateInput("My State1",new InputCombination("Mouse1+Joystick12AxisXPositive(x2)+B"));
//			
			//if you want to load some states from .xml and add custom manually first load settings xml
			InputManager.loadSettings(Path.Combine(Application.streamingAssetsPath,"InputSettings.xml"));

            InputManager.AddStateInput("adasdasd", KeyCodeExtension.Backspace.SINGLE, KeyCodeExtension.Joystick1AxisPovXNegative.LONG, KeyCodeExtension.Delete.DOUBLE);

        //Add state and event 
			InputManager.AddStateInput("Click_W+C_State", "W+C");


			//KeyCode.Alpha0.Long,
			//Joysticks.Joystick0.

            InputEvent ev = new InputEvent("Click_W+C_State");
			//InputEvent ev = new InputEvent((int)States.SomeState);
        
			ev.CONT+=new EventHandler(Handle1);
            ev.CONT+= new EventHandler(Handle2);
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
	void Update () {


//			if(InputManager.GetInputDown((int)States.Wave)){
//								Debug.Log("Wave Down");
//			}
		


//			if(InputManager.GetInput((int)States.Walk_Forward,false)){
//
//			}
	

//          if(InputManager.GetInputDown((int)States.Walk_Forward)){
//				Debug.Log("Down");
//			}
//
			//			if(InputManager.GetInputUp((int)States.Walk_Forward)){
//				Debug.Log("Up");
//			}
//
//		
//
//          //using input as analog value
			//float analogValue=InputManager.GetInput((int)States.Walk_Forward,false,0.3f,0.1f,0f);
//			analogValue-=InputManager.GetInput((int)States.Base_Layer_MyState,false,0.3f,0.1f,0f);
//
//			Debug.Log(analogValue);





	}
}
}
