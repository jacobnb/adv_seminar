using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touch_joystick_script : MonoBehaviour
{
    [SerializeField]
    float touchRadius;
    [SerializeField]
    float joystickRadius;
    Vector3 touchPosition;
    Vector3 startPosit;
    bool validTouch = false;
    float cameraOffset;
    LineRenderer line = null;
    [SerializeField]
    int pointsInCircle;
    int touchIndex = -1;
    Touch testTouch;
    // Use this for initialization
    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        Debug.Assert(line);
        startPosit = transform.position;
        touchPosition = transform.position;
        cameraOffset = -Camera.main.transform.position.z;
        initLine();
        validTouch = false;
    }

    void initLine()
    {
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
            x = Mathf.Cos(theta) * touchRadius + startPosit.x;
            y = Mathf.Sin(theta) * touchRadius + startPosit.y;

            line.SetPosition(i, new Vector3(x, y, z));

            theta += thetaIncrement;
        }


    }

    // Update is called once per frame
    void Update()
    {
        getTouchInput();
        moveJoystick();
    }
    public Vector2 getJoystickLoc()
    {
        return (transform.position - startPosit).normalized;
    }
    void moveJoystick()
    {
        if ((touchPosition - startPosit).magnitude < joystickRadius)
        {
            transform.position = touchPosition;
        }
        else
        {
            transform.position = (touchPosition - startPosit).normalized * joystickRadius + startPosit;
        }
    }
    Vector3 getWorldPosition(Vector2 touchPosit)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(touchPosit.x, touchPosit.y, cameraOffset));
    }

    void getTouchInput()
    {
        if (Input.touchCount > 0)
        { //get a touch inside the joystick area.
            foreach (Touch myTouch in Input.touches)
            {
                touchPosition = getWorldPosition(myTouch.position);
                if ((touchPosition - startPosit).magnitude < touchRadius)
                {//if touch is close enough, return.
                    return;
                }
            }
        }
        //if no valid touches have been found, reset.
        touchPosition = startPosit;
    }
    void oldGetTouchInput()
    {

        if (validTouch)
        {
            //if we have a touch get that position
            //can't just store the touch b/c it won't update.
            //might be better to use fingerID https://docs.unity3d.com/ScriptReference/Touch.html
            touchIndex = testTouch.fingerId;
            if (touchIndex < Input.touchCount)
            {
                touchPosition = getWorldPosition(Input.GetTouch(touchIndex).position);

            }
            else
            {
                validTouch = false;
                touchPosition = startPosit;
            }
        }
        else if (Input.touchCount > 0)
        {
            //Find a touch starting inside the joystick area.
            int index = 0;
            foreach (Touch myTouch in Input.touches)
            {
                if (myTouch.phase == TouchPhase.Began)
                {
                    if ((getWorldPosition(myTouch.position) - startPosit).magnitude < touchRadius)
                    {//if touch is close enough.
                        validTouch = true;
                        touchIndex = myTouch.fingerId;
                        testTouch = myTouch;
                        return;
                    }


                }
                index++;
            }

        }
    }


}
