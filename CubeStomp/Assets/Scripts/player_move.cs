using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_move : MonoBehaviour {
    public int playerNum;
    public float moveSpeed = 10;
    public float jumpHeight = 10;
    private float moveDirection;
    public bool shouldJump, canJump, doubleJump; 
    //^public to be accessed by the bottom_collider_script
    public float groundDistance = 3.02f;
    private Rigidbody2D rb;
    public LayerMask groundMask;
    private Transform bottomCollider;
    public bool touching_enemyBottom, touching_enemySide, touching_enemyTop;
	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody2D>();
        canJump = true;
        doubleJump = true;
        bottomCollider = transform.Find("Bottom Collider");
	}
	
	// Update is called once per frame
	void Update () {
        getInput();
        movePlayer();
        checkForGround();
        checkForDamage();
	}

    void checkForDamage()
    {
        if(!touching_enemySide && canJump && touching_enemyBottom)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - 0.01f, transform.localScale.z);
            if(transform.localScale.y < 0.01)
            {
                gameObject.SetActive(false);
            }
        }
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
        rb.velocity = new Vector2(rb.velocity.x, 0.0f);
        rb.AddForce(new Vector2(0f, jumpHeight));
    }

    void checkForGround() {
        Vector2 boxSize = new Vector2(bottomCollider.gameObject.GetComponent<BoxCollider2D>().bounds.size.x, bottomCollider.gameObject.GetComponent<Collider2D>().bounds.size.y);
        RaycastHit2D hitGround = Physics2D.BoxCast(bottomCollider.position, boxSize, 0f, Vector2.down, groundDistance, groundMask); //set up raycast mask in start.
        Debug.DrawRay(bottomCollider.position, Vector3.down*groundDistance);
        //RaycastHit2D hitGround = Physics2D.Raycast(bottomCollider.position, Vector2.down, groundDistance, groundMask); //set up raycast mask in start.
        if(hitGround.collider){
            //canJump = true;
            //doubleJump = true;
        }
    }
    void OnColliderEnter(Collider col){
		Debug.Log("NOT Bottom Collider 2");
	}

}
