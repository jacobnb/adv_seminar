using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_bounds_script : MonoBehaviour {
    /*
     * This class passes the camera bounds from the objects polygon collider 
     * so that it can be copied into the game scene  
     * Access like a singleton so that it's easy to access cross scene.
     */
    PolygonCollider2D bounds;
    public static camera_bounds_script BOUNDS_SCRIPT;
    private void Awake()
    {
        Debug.Log("Setting Bounds Script");
        BOUNDS_SCRIPT = this;
        bounds = GetComponent<PolygonCollider2D>();
    }

    public Vector2[] getBoundsPoints()
    {
        if (bounds)
        {
            return bounds.points;
        }
        else
        {
            return null;
        }
    }
}
