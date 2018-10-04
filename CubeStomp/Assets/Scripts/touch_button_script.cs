using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Touchable;

public class touch_button_script : touch_object {
    [SerializeField]
    bool isTouched = false;

    public bool isClicked()
    {
        if (isTouched)
        {
            isTouched = false;
            return true;
        }
        return isTouched;
    }

    public override void checkForTouching(Touch[] newTouches)
    {
        foreach (Touch tempTouch in newTouches)
        {
            if (tempTouch.phase == TouchPhase.Began && (tempTouch.position - startPosit).magnitude < radius)
            {
                myTouch = tempTouch;
                hasBeenTouched();
                return;
            }
        }
        hasNotBeenTouched();
    }
    protected override void hasBeenTouched()
    {
        isTouched = true;
    }

    protected override void hasNotBeenTouched()
    {
        isTouched = false;
    }
}
