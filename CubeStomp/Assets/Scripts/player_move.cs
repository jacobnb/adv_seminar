 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_move : MonoBehaviour
{
    public player_move enemy_moveScript;
    public touch_joystick_script joystick_script;
    [SerializeField]
    Touchable.touch_object upButton_script;
    [SerializeField]
    Touchable.touch_object downButton_script;
    public int playerNum;
    public float moveSpeed = 10;
    public float jumpHeight = 10;
    public float smashSpeed = 1200;
    public float frozenTime = 0.5f;
    private float moveDirection;
    public bool shouldJump, canJump, doubleJump, shouldSmash, wallJump;
    //^public to be accessed by the bottom_collider_script
    bool isFrozen;
    private game_controller_script gcs;
    private cube_spitter_script cubeSpitter;
    public float groundDistance = 3.02f;
    private Rigidbody2D rb;
    public LayerMask groundMask;
    public LayerMask playerMask;
    private Transform bottomCollider;
    public bool touching_enemyBottom, touching_enemySide, touching_enemyTop;
    Vector2 touchOrigin = -Vector2.one;

    // Use this for initialization
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        Debug.Assert(rb);
        gcs = game_controller_script.GAME_CONTROLLER;
        Debug.Assert(gcs);
        cubeSpitter = gameObject.GetComponentInChildren<cube_spitter_script>();
        // Debug.Assert(cubeSpitter);
        canJump = true;
        doubleJump = true;
        bottomCollider = transform.Find("Bottom Collider");
        if (gameObject.name == ("Player1"))
        {
            playerNum = 1;
        }
        else if (gameObject.name == "Player2")
        {
            playerNum = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
        getKeyInput();
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        getTouchJoystickInput();
#endif 
        movePlayer();
        checkForDamage();
    }
    public IEnumerator freeze(float frozenTime)
    {
        isFrozen = true;
        yield return new WaitForSeconds(frozenTime);
        isFrozen = false;
    }

    void checkForDamage()
    {
        if (!touching_enemySide && canJump && touching_enemyBottom)
        {
            StartCoroutine(cubeSpitter.spawnCubes());
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - 0.01f, transform.localScale.z);
            bottomCollider.localScale = new Vector3(bottomCollider.localScale.x, bottomCollider.localScale.y + 0.05f, bottomCollider.localScale.z);
            //this is a hack to keep the bottom collider the right size.

            if (transform.localScale.y < 0.01)
            {
                gcs.playerLost(playerNum);
                gameObject.SetActive(false);
            }
        }
    }
    void smash()
    {
        rb.velocity = new Vector2(0.0f, 0.0f);
        rb.AddForce(new Vector2(0f, -smashSpeed));
        //raycast down, if hit player freeze player.
        //example StartCoroutine(Camera.main.GetComponent<myTimer>().Counter());
        Vector2 boxSize = new Vector2(bottomCollider.gameObject.GetComponent<BoxCollider2D>().bounds.size.x, bottomCollider.gameObject.GetComponent<Collider2D>().bounds.size.y);
        RaycastHit2D hitPlayer = Physics2D.BoxCast(bottomCollider.position, boxSize, 0f, Vector2.down, 100f, playerMask);
        if (hitPlayer.collider)
        {
            StartCoroutine(hitPlayer.transform.GetComponent<player_move>().freeze(frozenTime));
        }
        StartCoroutine("freeze", 2 * frozenTime);
    }
    void getTouchJoystickInput()
    {
        Vector2 joystickLocation = joystick_script.getJoystickLoc();
        moveDirection = joystickLocation.x;
        shouldJump = ((touch_button_script)upButton_script).isClicked(); //Is there a better way to do this?
        shouldSmash = ((touch_button_script)downButton_script).isClicked(); ;
    }

    void getKeyInput()
    {
        if (playerNum == 1)
        {
            moveDirection = Input.GetAxis("Horizontal");
            shouldJump = Input.GetKeyDown("up") | Input.GetKeyDown("o");
            shouldSmash = Input.GetKeyDown("down") | Input.GetKeyDown("l");
        }
        else if (playerNum == 2)
        {
            moveDirection = Input.GetAxis("Horizontal2");
            shouldJump = Input.GetKeyDown("w");
            shouldSmash = Input.GetKeyDown("s");
        }
        else
        {
            Debug.Log("Invalid player num");
        }
    }

    void movePlayer()
    {
        if (isFrozen)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        Vector2 velocity = rb.velocity;
        velocity.x = moveDirection * moveSpeed;
        rb.velocity = velocity;
        if (shouldSmash && !canJump)
        {
            smash();
        }
        if (shouldJump)
        {
            if (checkJump())
            {
                jump();
            }
            shouldJump = false;
        }
    }

    bool checkJump()
    {
        if (canJump)
        {
            canJump = false;
            wallJump = false;
            return true;
        }
        else if (wallJump)
        {
            wallJump = false;
            return true;
        }
        else if (doubleJump)
        {
            doubleJump = false;
            return true;
        }
        else
        {
            return false;
        }

    }
    void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0.0f);
        rb.AddForce(new Vector2(0f, jumpHeight));
        if (touching_enemySide || touching_enemyTop)
        {
            enemy_moveScript.jumpedOff();
        }
    }
    public void jumpedOff()
    {
        rb.AddForce(new Vector2(0f, -jumpHeight));
    }
}
