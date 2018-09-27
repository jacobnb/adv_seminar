using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touch_joystick_script : MonoBehaviour {
    Vector2 touchOrigin, touchDirection;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 100);
	}

	void getTouchInput(){
		if (Input.touchCount > 0){
			Touch myTouch = Input.touches[0];
			if(myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            else
            {
                touchDirection = (myTouch.position - touchOrigin).normalized;
            }
        }
	}
}
