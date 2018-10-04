using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Touchable
{
    public abstract class touch_object : MonoBehaviour
    {
        [SerializeField]
        protected float radius;
        protected Touch myTouch;
        protected Vector2 startPosit;

        protected void Start()
        {
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
            hasNotBeenTouched();
        }

        //do something when touched / not touched.
        protected abstract void hasBeenTouched();
        protected abstract void hasNotBeenTouched();
    }
}

