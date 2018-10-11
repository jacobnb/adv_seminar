using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Touchable
{
    public abstract class touch_object : MonoBehaviour
    {
        [SerializeField]
        protected float radius;
        [SerializeField]
        bool shouldDrawCircle;

        protected Touch myTouch;
        protected Vector2 startPosit;

        protected void Start()
        {
            startPosit = transform.position;
            if (shouldDrawCircle)
            {
                drawRadius();
            }
        }
        public void deActivate()
        {
            gameObject.SetActive(false);
        }
        private void drawRadius()
        {
            int pointsInCircle = 30;
            gameObject.AddComponent<LineRenderer>();
            LineRenderer line = gameObject.GetComponent<LineRenderer>();
            if (!line)
            {
                return;
            }
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.positionCount = pointsInCircle;
            line.useWorldSpace = true;
            line.loop = true;
            float x;
            float y;
            float z = 0;
            float theta = 0;
            float thetaIncrement = 2 * Mathf.PI / pointsInCircle;
            for (int i = 0; i < (pointsInCircle); i++)
            {
                x = Mathf.Cos(theta) * radius + startPosit.x;
                y = Mathf.Sin(theta) * radius + startPosit.y;

                line.SetPosition(i, new Vector3(x, y, z));

                theta += thetaIncrement;
            }
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

