using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Touchable
{
    public abstract class touch_object : MonoBehaviour
    {
        [SerializeField]
        float radius;
        Touch myTouch;
        Vector2 startPosit;

        protected void Start()
        {
            Debug.Log("Parent Start");
            startPosit = transform.position;
        }

        public virtual void checkForTouching(Touch[] newTouches)
        { //can be overriden with override
            //Will only get the first touch in it's radius.
            foreach(Touch tempTouch in newTouches)
            {
                if ((tempTouch.position - startPosit).magnitude < radius)
                {
                    myTouch = tempTouch;
                    hasBeenTouched();
                    return;
                }
            }

            //check if a touch is touching.
        }

        //do something when touched.
        protected abstract void hasBeenTouched();
    }
}

