using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class disable_cinemachine : MonoBehaviour {
    CinemachineBrain cBrain;
	
    //Disables cinemachine for the start screen.
	void Start () {
        cBrain = gameObject.GetComponent<CinemachineBrain>();
        Debug.Assert(cBrain);
        enableCinemachine(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void enableCinemachine(bool enabled)
    {
        if (enabled)
        {
            cBrain.enabled = true;
        }
        else
        {
            cBrain.enabled = false;
        }
    }
}
