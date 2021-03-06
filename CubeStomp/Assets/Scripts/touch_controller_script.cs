﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Touchable;

//Manages touches and touch_objects (buttons, joysticks)
public class touch_controller_script : MonoBehaviour {
    [SerializeField]
    touch_object[] touchableObjects;
    float cameraOffset;
    Touch[] touches;

    void Start () {
        cameraOffset = -Camera.main.transform.position.z;

#if UNITY_STANDALONE || UNITY_WEBPLAYER
        //disable all
        foreach(touch_object touchObject in touchableObjects)
        {
            touchObject.deActivate();
        }
        gameObject.SetActive(false);
#endif 

    }


    void Update () {

        getTouches();
        foreach(touch_object touchObject in touchableObjects)
        {
            touchObject.checkForTouching(touches);
        }
	}

    void getTouches()
    {
        touches = Input.touches;
        for(int x = 0; x < touches.Length; x++)
        {
            touches[x].position = getWorldPosition(touches[x].position);
        }
        return;
    }

    Vector3 getWorldPosition(Vector2 touchPosit)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(touchPosit.x, touchPosit.y, cameraOffset));
    }

}
