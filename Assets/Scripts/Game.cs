using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using ws.winx.input;
using ws.winx.input.states;
using System.Runtime.Serialization;
using System.Linq;
using ws.winx.platform.windows;
using ws.winx.devices;

namespace ws.winx{
public class Game : MonoBehaviour {

		static int idleState = Animator.StringToHash("Base Layer.Idle");	
		static int locoState = Animator.StringToHash("Base Layer.Locomotion");			// these integers are references to our animator's states
		static int jumpState = Animator.StringToHash("Base Layer.Jump");				// and are used to check state for various actions to occur
		static int jumpDownState = Animator.StringToHash("Base Layer.JumpDown");		// within our FixedUpdate() function below
		static int fallState = Animator.StringToHash("Base Layer.Fall");
		static int rollState = Animator.StringToHash("Base Layer.Roll");
		static int waveState = Animator.StringToHash("Layer2.Wave");

		//XInputDriver driver;


//		public static UnityEngine.KeyCode KeyCode(){
//			return new UnityEngine.KeyCode();
//		}

		//static UnityEngine.KeyCode KeyCode=new KeyCode();//UnityEngine.KeyCode.A;

	// Use this for initialization
	void Start () {
//        UnityEngine.KeyCode KeyCode=new KeyCode();//UnityEngine.KeyCode.A;
//
//			//UnityEngine.KeyCode KeyCode=new KeyCode();
//            JoystickAxis axis =KeyCode.toAxis(2333);
//            KeyCode.toAxis(333);
//            KeyCode.toAxis(23343);
//       // KeyCode.toAxis
//            var a = KeyCode.A;
//            KeyCode.toAxis(23);
//      
//            KeyCode.A.toAxis(23);
        

			//KeyCode.toAxis(
        //KeyCode.toC
            // KeyCode.toCode()
			//KeyCode.toCode(Joysticks.Joystick1,JoystickAxis.AxisPovX,0);

			//KeyCode.A.toCode(
			//driver = new XInputDriver ();
			//KeyCode a=new UnityEngine.KeyCode();
			//KeyCode.toCode();

			InputManager.AddDriver(new XInputDriver());

			InputManager.AddStateInput("My State1",new InputCombination(KeyCodeExtension.toCode(Joysticks.Joystick1,JoystickAxis.AxisPovX,JoystickPovPosition.Forward),(int)KeyCode.Joystick4Button9,(int)KeyCode.P,(int)KeyCode.JoystickButton0));
//			InputManager.AddStateInput("My State2",new InputCombination(KeyCode.Joystick4Button9,KeyCode.P,KeyCode.JoystickButton0));
//			InputManager.AddStateInput("My State3",new InputCombination("A(x2)+Mouse1+JoystickButton31"));
//			InputManager.AddStateInput("My State1",new InputCombination("Mouse1+Joystick12AxisXPositive(x2)+B"));
//			InputManager.AddStateInput("My State2","Joystick10AxisPovXForward(-)+C(x2)");
			//InputManager.saveSettings(Path.Combine(Application.dataPath,"InputSettings.xml"));

			Debug.Log(InputManager.Log());




	}

	
	
	// Update is called once per frame
	void Update () {
			//driver.GetInput();
		
//			if(InputManager.GetInput((int)CharacterInputControllerClass.States.Base_Layer_Walk_Forward,false,false)){
//				Debug.Log("Hold down");
//			}

//			if(InputManager.GetInputDown((int)CharacterInputControllerClass.States.Base_Layer_Walk_Forward)){
//				Debug.Log("Down");
//			}
//
//			if(InputManager.GetInputUp((int)CharacterInputControllerClass.States.Base_Layer_Walk_Forward)){
//				Debug.Log("Up");
//			}
//
//		
//
//
//			float analogValue=InputManager.GetInput((int)CharacterInputControllerClass.States.Base_Layer_Walk_Forward,false,0.3f,0.1f,0f);
//			analogValue-=InputManager.GetInput((int)CharacterInputControllerClass.States.Base_Layer_MyState,false,0.3f,0.1f,0f);
//
//			Debug.Log(analogValue);





	}
}
}
