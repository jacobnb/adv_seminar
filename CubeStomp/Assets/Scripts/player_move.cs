 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Controls player movement, health, and triggers animations
public class player_move : MonoBehaviour
{
    enum TAGS
    { //these correspond to tags in game.
        NONE = 0,
        GROUND,
        WALL,
        PLAYER
    }

    [Header("Dev Variables")]
    [SerializeField]
    bool enableAI = false;
    [SerializeField]
    TAGS[] collisions = new TAGS[4];

    [Header("Movement")]
    [SerializeField]
    float moveSpeed = 20;
    [SerializeField]
    float jumpHeight = 900;
    [SerializeField]
    float smashSpeed = 1200;
    [SerializeField]
    [Tooltip("How long the enemy is frozen after a successful smash")]
    float frozenTime = 0.5f;
    [SerializeField]
    [Tooltip("Force away from the wall after wall jump")]
    float wallJumpForce = 0; //erased by movement function.
    private float moveDirection;
    [SerializeField]
    bool shouldJump, canJump, doubleJump, shouldSmash, wallJump, hasWallJumped, isSmashing;
    //used in collisions array to get location
    enum CollisionsLoc { botColl, rightColl, leftColl, topColl };
    int prevMoveDirection; //records move direction from a short time ago, used in dash.

    [Header("Touch Controls")]
    public touch_joystick_script joystick_script;
    [SerializeField]
    Touchable.touch_object upButton_script;
    [SerializeField]
    Touchable.touch_object downButton_script;
    
    [Header("References")]
    [SerializeField]
    private player_move enemy_moveScript;
    public LayerMask playerMask;
    private player_anim_script anim_script;
    private game_controller_script gcs;
    private Rigidbody2D rb;

    //Reset and health vars
    float maxHealth = 50;
    Vector2 startPosit;
    Vector3 startSize;
    int playerNum; //set based on name

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        gcs = game_controller_script.GAME_CONTROLLER;
        anim_script = gameObject.GetComponent<player_anim_script>();
        Debug.Assert(rb && gcs && anim_script);
        startPosit = transform.position;
        startSize = transform.localScale;
        canJump = true;
        doubleJump = true;
        hasWallJumped = false;
        if (gameObject.name == ("Player1"))
        {
            playerNum = 1;
        }
        else if (gameObject.name == "Player2")
        {
            playerNum = 2;
        }
    }

    void Update()
    {
        checkIfCanJump();
        setAnimations();

#if UNITY_STANDALONE || UNITY_WEBPLAYER
        getKeyInput();
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        getTouchJoystickInput();
#endif 
        if (enableAI)
        {
            AIMovement();
        }
        movePlayer();
        checkAndDash();
        checkForDamage();
    }
    
    //Set collisions positions and flags
    private void OnCollisionEnter2D(Collision2D coll)
    {
        TAGS collTag = TAGS.NONE;
        string tag = coll.gameObject.tag;
        if (tag == "Ground")
        {
            collTag = TAGS.GROUND;
        }
        else if ( tag == "Wall")
        {
            collTag = TAGS.WALL;
        }
        else if (tag == "Player")
        {
            collTag = TAGS.PLAYER;
            
        }
        else
        { //not a tag we care about so ignore.
            Debug.Log("GameObject does not have recognized tag");
            return;
        }
        if(coll.contactCount <= 0)
        {
            return;
        }
        Vector2 contactPoint = coll.GetContact(coll.contactCount-1).point; //get a contact point
        if(contactPoint.x != coll.GetContact(0).point.x) //if points aren't on the same x-axis
        {//point is on the top / bottom
            if(contactPoint.y < transform.position.y) //on the bottom
            {
                collisions[(int)CollisionsLoc.botColl] = collTag;
            }
            else
            {
                collisions[(int)CollisionsLoc.topColl] = collTag;
            }
        }
        else if (contactPoint.y != coll.GetContact(0).point.y)
        {
            if(contactPoint.x < transform.position.x) // on the left
            {
                collisions[(int)CollisionsLoc.leftColl] = collTag;
            }
            else
            {
                collisions[(int)CollisionsLoc.rightColl] = collTag;
            }
        }
        else
        {
            Debug.Log("Error in collider positioning");
        }

    }

    //Remove collisions positions and flags
    private void OnCollisionExit2D(Collision2D coll)
    {
        TAGS collTag = TAGS.NONE;
        string tag = coll.gameObject.tag;
        if (tag == "Ground")
        {
            collTag = TAGS.GROUND;
        }
        else if (tag == "Wall")
        {
            collTag = TAGS.WALL;
        }
        else if (tag == "Player")
        {
            collTag = TAGS.PLAYER;
        }
        else
        { //not a type we care about so ignore.
            return;
        }
        for(int i = 0; i < collisions.Length; i++)
        {
            if(collisions[i] == collTag)
            {
                collisions[i] = TAGS.NONE;
            }
        }
     }
    
    private void setAnimations()
    {
        if (collisions[(int)CollisionsLoc.botColl] == TAGS.GROUND)
        {
            //if on top of player show dust.
            if (collisions[(int)CollisionsLoc.topColl] == TAGS.PLAYER)
            {
                anim_script.setAllDusts(false, false, true);
            }
            else
            //If touching ground do nothing
            {
                anim_script.setAllDusts(false, false, false);
            }
            return;
            //== THERE IS A RETURN HERE ==//
        }
        else
        {
            anim_script.topDust(false);
        }
        //left side
        if (collisions[(int)CollisionsLoc.leftColl] != TAGS.NONE)
        {
            anim_script.leftDust(true);
        }
        else
        {
            anim_script.leftDust(false);
        }
        //right side
        if (collisions[(int)CollisionsLoc.rightColl] != TAGS.NONE)
        {
            anim_script.rightDust(true);
        }
        else
        {
            anim_script.rightDust(false);
        }
    }

    //Called when a new level is loaded.
    public void GameStarted()
    {
        resetPlayer();
    }

    //Set player health (in menu screen)
    public void setHealth(float newHP)
    {
        maxHealth = newHP;
    }

    //Reset position and health
    /*If this isn't working, make sure that in game scene the UI canvas element is unchecked and everything else under it is enabled*/
    void resetPlayer()
    {
        Debug.Log("Resetting Player " + playerNum);
        rb.velocity = Vector2.zero;
        transform.position = startPosit;
        transform.localScale = startSize;
        for(int i =0; i < collisions.Length; i++)
        {
            collisions[i] = TAGS.NONE;
        }
        gameObject.SetActive(true);

    }

    //Checks through collisions to see if the player can jump
    void checkIfCanJump()
    {
        //if touching ground or on top of enemy
        if(collisions[(int)CollisionsLoc.botColl] == TAGS.GROUND
            || collisions[(int)CollisionsLoc.botColl] == TAGS.PLAYER)
        {
            canJump = true;
            doubleJump = true;
            wallJump = false;
            hasWallJumped = false;
            return;
        }
        else
        {
            canJump = false;
        }
        //if side is touching player or wall
        if(collisions[(int)CollisionsLoc.leftColl] > TAGS.GROUND 
            || collisions[(int)CollisionsLoc.rightColl] > TAGS.GROUND)
        {
            wallJump = true;
        }
        else
        {
            wallJump = false;
        }
    }

    //Make the player move randomly for single player testing. This is bad AI
    void AIMovement()
    {
        moveDirection = Random.Range(-1f, 1f);
        shouldJump = Random.value < 0.1f;
        //shouldSmash = Random.value < 0.5f;
    }

    public IEnumerator smashing(float frozenTime)
    {
        isSmashing = true;
        yield return new WaitForSeconds(frozenTime);
        isSmashing = false;
    }

    void checkForDamage()
    {
        if (collisions[(int)CollisionsLoc.botColl] == TAGS.GROUND && collisions[(int)CollisionsLoc.topColl] == TAGS.PLAYER) 
            //if on the ground and opponent is on top
        {
            //Start damage anim StartCoroutine(cubeSpitter.spawnCubes());
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - 0.5f/maxHealth, transform.localScale.z);
            if (transform.localScale.y < 0.01)
            {
                gcs.playerLost(playerNum);
                gameObject.SetActive(false);
            }
        }
    }

    //Move player down until they hit the floor or opponent.
    void smash()
    {
        rb.velocity = new Vector2(0.0f, 0.0f);
        rb.AddForce(new Vector2(0f, -smashSpeed));
        //raycast down, if hit player freeze player.
        Vector2 boxSize = new Vector2(gameObject.GetComponent<BoxCollider2D>().bounds.size.x, 
                                      gameObject.GetComponent<Collider2D>().bounds.size.y);
        RaycastHit2D hitPlayer = Physics2D.BoxCast(transform.position, boxSize, 0f, Vector2.down, 100f, playerMask);
        if (hitPlayer.collider)
        {
            StartCoroutine(hitPlayer.transform.GetComponent<player_move>().smashing(frozenTime));
        }
        StartCoroutine("smashing", 2 * frozenTime);
    }

    void getTouchJoystickInput()
    {
        Vector2 joystickLocation = joystick_script.getJoystickLoc();
        moveDirection = joystickLocation.x;
        shouldJump = ((touch_button_script)upButton_script).isClicked(); //Is there a better way to do this?
        shouldSmash = ((touch_button_script)downButton_script).isClicked();
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
    private void checkAndDash()
    {
        bool wasMovingRecently = false;
        if (moveDirection > 0)
        {
            prevMoveDirection = 1;
        }
        else if (moveDirection < 0)
        {
            prevMoveDirection = -1;
        }
        else if (moveDirection == 0 && prevMoveDirection != 0)
        {
            wasMovingRecently = true;
        }

        if(wasMovingRecently && moveDirection != 0)
        {
            if(Mathf.Sign(moveDirection) == Mathf.Sign(prevMoveDirection))
            {
                //do dash
            }
            else
            {
                //clear dash.
            }
        }
    }
    void movePlayer()
    {
        if (isSmashing)
        {
            rb.velocity = new Vector2(0f, -smashSpeed);
            anim_script.enableStunAnim();
            return;
        }
        anim_script.disableStunAnim();
        Vector2 velocity = rb.velocity;
        velocity.x = moveDirection * moveSpeed;
        rb.velocity = velocity;
        if (shouldSmash && !canJump)
        {
            smash();
        }
        if (shouldJump)
        {
            checkJump();
            shouldJump = false;
        }
    }

    //If a valid jump variable is true then jump.
    void checkJump()
    {
        if (canJump)
        {
            jump();
            canJump = false;
            wallJump = false;
        }
        else if (wallJump)
        {
            jumpOff();
            wallJump = false;
        }
        else if (doubleJump)
        {
            jump();
            doubleJump = false;
        }
    }

    void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0.0f);
        rb.AddForce(new Vector2(0f, jumpHeight));
        if (collisions[(int)CollisionsLoc.botColl] == TAGS.PLAYER 
            || collisions[(int)CollisionsLoc.leftColl] == TAGS.PLAYER 
            || collisions[(int)CollisionsLoc.rightColl] == TAGS.PLAYER)
        {
            enemy_moveScript.jumpedOff();
        }
    }

    //jump away from wall / player
    void jumpOff()
    {
        //disable multiple wall jumps
        if (hasWallJumped)
        {
            if (doubleJump)
            {
                jump();
                doubleJump = false;
            }
            return;
        }
        else
        {
            hasWallJumped = true;
        }
        //Add impulse off of wall.
        //This gets erased by movement I think
        float wallForce;
        if(collisions[(int)CollisionsLoc.rightColl] == TAGS.WALL)
        {
            wallForce = -wallJumpForce;
        }
        else
        {
            wallForce = wallJumpForce;
        }
        rb.velocity = new Vector2(0.0f, 0.0f);//Zero velocity

        rb.AddForce(new Vector2(wallForce, jumpHeight));
        if (collisions[(int)CollisionsLoc.botColl] == TAGS.PLAYER
            || collisions[(int)CollisionsLoc.leftColl] == TAGS.PLAYER
            || collisions[(int)CollisionsLoc.rightColl] == TAGS.PLAYER)
        {
            enemy_moveScript.jumpedOff();
        }
    }

    //Player has been jumped off of, add downward force.
    public void jumpedOff()
    {
        rb.AddForce(new Vector2(0f, -jumpHeight));
    }
}
