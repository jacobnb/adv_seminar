using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Touchable
{
    public class touch_object : MonoBehaviour
    {
        [SerializeField]
        float radius;
        Touch myTouch;
        
        public float getRadius()
        {
            return radius;
        }
        public void setTouch(Touch newTouch)
        {
            myTouch = newTouch;
        }
    }
}

