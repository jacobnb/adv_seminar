using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Touchable;

public class touch_joystick_script : touch_object
{
    [SerializeField]
    float joystickRadius;
    LineRenderer line = null;
    [SerializeField]
    int pointsInCircle;
    Vector2 touchPosition;

    private new void Start()
    {
        touchPosition = startPosit;
        base.Start();
        line = gameObject.GetComponent<LineRenderer>();
        Debug.Assert(line);
        initLine();
    }

    void Update()
    {
        moveJoystick();
    }

    protected override void hasBeenTouched()
    {
        touchPosition = myTouch.position;

    }

    protected override void hasNotBeenTouched()
    {
        touchPosition = startPosit;
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
            x = Mathf.Cos(theta) * radius + startPosit.x;
            y = Mathf.Sin(theta) * radius + startPosit.y;

            line.SetPosition(i, new Vector3(x, y, z));

            theta += thetaIncrement;
        }
    }

    public Vector2 getJoystickLoc()
    {
        return ((Vector2)transform.position - startPosit).normalized;
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
}
