using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Touchable;
public class touch_controller_script : MonoBehaviour {
    [SerializeField]
    touch_object[] touchableObjects;
    float[] object_radii; 
    //not sure if it makes sense to cache the radii like this or not.


	void Start () {
        cacheRadii();
    }
	
    
	void Update () {
	    //Iterate through touches	
	}

    void cacheRadii()
    {
        int index = 0;
        foreach (touch_object touchObject in touchableObjects)
        {
            object_radii[index] = touchObject.getRadius();
            index++;
        }
    }
}
