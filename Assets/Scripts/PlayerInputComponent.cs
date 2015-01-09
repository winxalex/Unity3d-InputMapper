using UnityEngine;
using System.Collections;
using ws.winx.input;
using ws.winx.input.states;
using System;

public class PlayerInputComponent : MonoBehaviour
{



    public InputPlayer.Player Player;
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
            InputManager.MapStateToInput("ManualFullAxisMap",InputPlayer.Player.Player0, InputCode.Joystick0AxisX);// InputCode.W.SINGLE, InputCode.C.SINGLE);


        //			InputManager.MapStateToInput ("WalkForward", InputCode.W.SINGLE);
        //			InputManager.MapStateToInput ("WalkForward", 1, InputCode.Joystick1AxisXPositive.SINGLE);
        //			
        //			
        //			InputManager.MapStateToInput ("WalkBackward", InputCode.S.SINGLE);
        //			InputManager.MapStateToInput ("WalkBackward", 1, InputCode.Joystick1AxisYNegative.SINGLE);

        UnityEngine.Debug.Log("Log:" + InputManager.Log());




        InputManager.addEventListener((int)States.Wave).UP += onUp;
        InputManager.addEventListener((int)States.Wave).DOWN += onDown;








    }


    void onUp(object o, EventArgs args)
    {
        Debug.Log("Up");
    }

    void onDown(object o, EventArgs args)
    {
        Debug.Log("Down");
    }



    // Update is called once per frame
    void Update()
    {
       


        if (InputManager.GetInputDown((int)States.Wave,Player))
        {
            animator.Play((int)States.Wave);
        }

        if (InputManager.GetInputDown((int)States.Jump))
        {
            animator.Play((int)States.Jump);
        }
        //
        float forward = Math.Abs(InputManager.GetInput((int)States.WalkForward,Player, 0.25f))
            - Math.Abs(InputManager.GetInput((int)States.WalkBackward,Player, 0.25f));

        //Debug.Log (forward);

        animator.SetFloat(forwardHash, forward);
        //
        //
        float turn = Math.Abs(InputManager.GetInput((int)States.TurnRight,Player, 0.25f))
            - Math.Abs(InputManager.GetInput((int)States.TurnLeft,Player, 0.25f));
        //		
        //		//Debug.Log (turn);
        //		
        animator.SetFloat(turnHash, turn);



        InputManager.dispatchEvent();

    }
}

