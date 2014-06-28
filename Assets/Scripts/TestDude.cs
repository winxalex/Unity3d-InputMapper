using UnityEngine;
using System.Collections;
using ws.winx.input;


public class TestDude : MonoBehaviour {

    Animator _animator;

    int WaveState=Animator.StringToHash("Wave");

	// Use this for initialization
	void Start () {
	_animator=this.GetComponent<Animator>();

        //State Wave will be bind to "W" (combination)
		InputManager.AddStateInput(WaveState,"W");

	}
	
	// Update is called once per frame
    void Update()
    {

        //use auto generated States.[state_name] class instead manual Wave creation
        InputManager.PlayStateOnInputDown(_animator, WaveState);
    }
	
}
