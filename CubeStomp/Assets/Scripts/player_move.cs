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
    private ContactFilter2D groundFilter;
	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody2D>();
        canJump = true;
        doubleJump = true;
        groundFilter.SetLayerMask(
            LayerMask.NameToLayer("Ground")
        );
	}
	
	// Update is called once per frame
	void Update () {
        getInput();
        movePlayer();
        checkForGround();
	}

    void getInput() {
        if(playerNum == 1 ){
            moveDirection = Input.GetAxis("Horizontal");
            shouldJump = Input.GetKeyDown("up");
        }
        else if (playerNum == 2){
            moveDirection = Input.GetAxis("Horizontal2");
            shouldJump = Input.GetKeyDown("w");
        }
        else {
            Debug.Log("Invalid player num");
        }
    }

    void movePlayer() {
        //rb.AddForce(new Vector2(moveDirection*moveSpeed, 0f));
        Vector2 velocity = rb.velocity;
        velocity.x = moveDirection*moveSpeed;
        rb.velocity = velocity;

        if(shouldJump){
            if(checkJump()){
                jump();
            }
            shouldJump = false;
        }
    }

    bool checkJump(){
        if(canJump){
            canJump = false;
            return true;
        }
        else if (doubleJump){
            doubleJump = false;
            return true;
        }
        else {
            return false;
        }

    }
    void jump(){
        rb.AddForce(new Vector2(0f, jumpHeight));
    }

    void checkForGround() {
        RaycastHit2D hitGround = Physics2D.raycast //set up raycast mask in start.
    }
}
