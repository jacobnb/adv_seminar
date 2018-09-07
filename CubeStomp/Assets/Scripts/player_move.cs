using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_move : MonoBehaviour {
    public int playerNum;
    public float moveSpeed = 10;
    public float jumpHeight = 10;
    private float moveDirection;
    bool shouldJump, canJump, doubleJump;
    private Rigidbody2D rb;
	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        getInput();
        movePlayer();
	}

    void getInput() {
        if(playerNum == 1 ){
            moveDirection = Input.GetAxis("Horizontal");
        }
        else if (playerNum == 2){
            moveDirection = Input.GetAxis("Horizontal2");
        }
        else {
            Debug.Log("Invalid player num");
        }
    }

    void movePlayer() {
        rb.AddForce(new Vector2(moveDirection*moveSpeed, 0f));
        if(shouldJump){
            if(checkJump()){
                jump();
                //working on this
            }
        }
    }

    bool checkJump(){
        return true;
    }
    void jump(){
        rb.AddForce(new Vector2(0f, jumpHeight));
    }
}
