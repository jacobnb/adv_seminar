using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touch_joystick_script : MonoBehaviour {
    public float touchRadius = 1.0f;
    Vector2 touchOrigin, touchDirection;
    Vector3 startPosit;
	// Use this for initialization
	void Start () {
        startPosit = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        getTouchInput();
        moveJoystick();
	}

    void moveJoystick()
    {
        transform.position = startPosit + new Vector3(touchDirection.x, touchDirection.y, 0f);
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
                
                touchDirection = (myTouch.position - touchOrigin)/ touchRadius;
                Camera.main.ScreenToWorldPoint(new Vector3(touchDirection.x, touchDirection.y, -Camera.main.transform.position.z));
                if (touchDirection.magnitude > 1)
                {
                    touchDirection.Normalize();
                }
            }
        }
        else
        {
            touchDirection = Vector2.zero;
        }
	}
}
