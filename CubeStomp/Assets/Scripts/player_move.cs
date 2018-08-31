using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_move : MonoBehaviour {
    public float moveSpeed;
    public float jumpHeight;
    private float moveDirection;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        getInput();
	}

    void getInput()
    {
        moveDirection = Input.GetAxis("Horizontal");
    }

    void movePlayer()
    {

    }
}
