using UnityEngine;
using System.Collections;
using ws.winx.input;
using ws.winx.input.states;
using System;

public class PlayerInputComponent : MonoBehaviour
{



    public InputPlayer.Player Player;

	//public States state;

    Animator animator;
    int forwardHash;
    int turnHash;


    // Use this for initialization
    void Start()
    {
        animator = this.GetComponent<Animator>();
        forwardHash = Animator.StringToHash("forward");
        turnHash = Animator.StringToHash("turn");


    }


	public void onInputSettingsLoadComplete(){

		Debug.Log ("InputSettings Loadded Received in "+Player);
		manuallyAddStateAndHandlers ();

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
        if (!InputManager.HasInputState("ManualFullAxisMap"))
            InputManager.MapStateToInput("ManualFullAxisMap",Player, InputCode.JoystickAxisX);// InputCode.W.SINGLE, InputCode.C.SINGLE);


        //			InputManager.MapStateToInput ("WalkForward", InputCode.W.SINGLE);
        //			InputManager.MapStateToInput ("WalkForward", 1, InputCode.Joystick1AxisXPositive.SINGLE);
        //			
        //			
        //			InputManager.MapStateToInput ("WalkBackward", InputCode.S.SINGLE);
        //			InputManager.MapStateToInput ("WalkBackward", 1, InputCode.Joystick1AxisYNegative.SINGLE);

    //    UnityEngine.Debug.Log("Log:" + InputManager.Log());




        InputManager.addEventListener((int)States.Wave,Player).UP += onUp;
        InputManager.addEventListener((int)States.Wave,Player).DOWN += onDown;








    }


    void onUp(object o, EventArgs args)
    {
        Debug.Log(Player+">Wave state trigger Up");
    }

    void onDown(object o, EventArgs args)
    {
		Debug.Log(Player+">Wave state trigger Down");
    }



    // Update is called once per frame
    void Update()
    {
       
		//Debug.Log (InputManager.GetInput (Animator.StringToHash ("ManualFullAxisMap"),Player,0.25f));

		//Debug.Log (InputManager.GetInputRaw (Animator.StringToHash ("ManualFullAxisMap"),Player,0.25f));

//		if (InputManager.GetInputUp(Animator.StringToHash ("ManualFullAxisMap"),Player))
//		{
//			Debug.Log (Player+"_Full Axis UP");
//		}

	


        if (InputManager.GetInputDown((int)States.Wave,Player,true))
        {
            animator.Play((int)States.Wave);
        }

        if (InputManager.GetInputDown((int)States.Jump,Player))
        {
            animator.Play((int)States.Jump);
        }


        //
        //Math.Abs prevent code to function diffrently when axis is inverted
        float forward = Math.Abs(InputManager.GetInput((int)States.WalkForward,Player, 0.25f))
            - Math.Abs(InputManager.GetInput((int)States.WalkBackward,Player, 0.25f));

        

        animator.SetFloat(forwardHash, forward);
        //
        //
        float turn = Math.Abs(InputManager.GetInput((int)States.TurnRight,Player, 0.25f))
            - Math.Abs(InputManager.GetInput((int)States.TurnLeft,Player, 0.25f));
       
        animator.SetFloat(turnHash, turn);



       

    }
}

