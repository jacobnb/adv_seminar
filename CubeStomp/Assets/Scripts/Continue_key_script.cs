using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//This checks for a series of key presses and calls the given function if one is pressed.
public class Continue_key_script : MonoBehaviour {
    [SerializeField]
    private UnityEvent funcToCall;
    [SerializeField]
    [Tooltip("When one of these is pressed it will trigger the function")]
    private string[] keysToCheck;

	// Update is called once per frame
	void Update () {
        getInput();
	}

    private void getInput()
    {
        foreach(string key in keysToCheck)
        {
            if (Input.GetKeyDown(key))
            {
                funcToCall.Invoke();
                return;
            }
        }
    }
}
