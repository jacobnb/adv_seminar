using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stunAnimScript : MonoBehaviour {
    [SerializeField]
    float rotationSpeed = 500;
    [SerializeField]
    [Tooltip("How far offset from the player this gameobject is")]
    Vector3 offsetPosition;
    Transform player;
    // Use this for initialization
    void Start () {
    }

    public void init(Transform playerToFollow)
    {
        player = playerToFollow;
    }
    // Update is called once per frame
    void Update () {
        Vector3 rotation = transform.localEulerAngles;
        rotation.z += rotationSpeed * Time.deltaTime;

        transform.localEulerAngles = rotation;
        transform.position = player.position + offsetPosition;
	}

}
