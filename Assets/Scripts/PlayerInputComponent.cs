using UnityEngine;
using System.Collections;
using ws.winx.input;
using ws.winx.input.states;

public class PlayerInputComponent : MonoBehaviour
{

		

	public int PlayerIndex;

		// Use this for initialization
		void Start ()
		{
			
		}
	
		// Update is called once per frame
		void Update ()
		{
		    ///
			//InputManager.Settings

		    
			//InputManager.currentPlayerIndex = PlayerIndex;


			if (InputManager.GetInputDown((int)States.Wave)) {
				this.GetComponent<Animator>().Play("Wave");
			}
			
			//	
			//
			if (InputManager.GetInputHold (Animator.StringToHash ("WalkBackward"))) {
				Debug.Log ("WalkBackward-Hold");
			}
			
			
			if (InputManager.GetInputDown (Animator.StringToHash ("WalkBackward"))) {
				Debug.Log ("WalkBackward-Down");
			}
			
			if (InputManager.GetInputUp (Animator.StringToHash ("WalkBackward"))) {
				Debug.Log ("WalkBackward-Up");
			}
		}
}

