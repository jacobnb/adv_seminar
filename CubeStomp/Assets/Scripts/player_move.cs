 using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class player_move : MonoBehaviour
{
    enum TAGS
    { //these correspond to tags in game.
        NONE = 0,
        WALL,
        GROUND,
        PLAYER
    }
    [SerializeField]
    bool enableAI = false;
    public player_move enemy_moveScript;
    public touch_joystick_script joystick_script;
    private player_anim_script anim_script;
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
    bool shouldJump, canJump, doubleJump, shouldSmash, wallJump;
    [SerializeField]
    TAGS[] collisions = new TAGS[4];
    //used in collisions array to get location
    enum CollisionsLoc {botColl, rightColl, leftColl, topColl}; 
    bool isFrozen;
    float maxHealth = 50;
    private game_controller_script gcs;
    public float groundDistance = 3.02f;
    private Rigidbody2D rb;
    public LayerMask groundMask;
    public LayerMask playerMask;
    Vector2 startPosit;
    Vector3 startSize;

    // Use this for initialization
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        Debug.Assert(rb);
        gcs = game_controller_script.GAME_CONTROLLER;
        Debug.Assert(gcs);
        anim_script = gameObject.GetComponent<player_anim_script>();
        startPosit = transform.position;
        startSize = transform.localScale;
        // Debug.Assert(cubeSpitter);
        canJump = true;
        doubleJump = true;
        if (gameObject.name == ("Player1"))
        {
            playerNum = 1;
        }
        else if (gameObject.name == "Player2")
        {
            playerNum = 2;
        }
    }

    private void setAnimations()
    {
        //If touching ground do nothing
        if(collisions[(int)CollisionsLoc.botColl] == TAGS.GROUND)
        {
            anim_script.setAllDusts(false, false, false);
            return;
        }
        if (collisions[(int)CollisionsLoc.botColl] == TAGS.PLAYER)
        {
            anim_script.setAllDusts(false, false, true);
            return;
        }
        if(collisions[(int)CollisionsLoc.leftColl] != TAGS.NONE)
        {
            anim_script.leftDust(true);
        }
        if(collisions[(int)CollisionsLoc.rightColl] != TAGS.NONE)
        {
            anim_script.rightDust(true);
        }
    }
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
        { //not a type we care about so ignore.
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
    public void GameStarted()
    {
        resetPlayer();
    }
    public void setHealth(float newHP)
    {
        maxHealth = newHP;
    }
    void resetPlayer()
    {
        rb.velocity = Vector2.zero;
        transform.position = startPosit;
        transform.localScale = startSize;
        for(int i =0; i < collisions.Length; i++)
        {
            collisions[i] = TAGS.NONE;
        }
        gameObject.SetActive(true);

    }
    void checkForGround()
    {
        bool touchingWallOrPlayer = false; //also touching player
        foreach(TAGS touching in collisions)
        {
            if(touching == TAGS.GROUND) {
                canJump = true;
                doubleJump = true;
                wallJump = false;
                return; //if player is touching ground, we don't care about the rest.
            }
            if (touching == TAGS.PLAYER || touching == TAGS.WALL)
            {
                touchingWallOrPlayer = true; //Or should we enable double jump if it's not enabled?
            } 
        }
        wallJump = touchingWallOrPlayer;
    }
    // Update is called once per frame
    void Update()
    {
        checkForGround();
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
        checkForDamage();
    }
    void AIMovement()
    {
        moveDirection = Random.Range(-1f, 1f);
        shouldJump = Random.value < 0.1f;
        //shouldSmash = Random.value < 0.5f;
    }
    public IEnumerator freeze(float frozenTime)
    {
        isFrozen = true;
        yield return new WaitForSeconds(frozenTime);
        isFrozen = false;
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
    void smash()
    {
        rb.velocity = new Vector2(0.0f, 0.0f);
        rb.AddForce(new Vector2(0f, -smashSpeed));
        //raycast down, if hit player freeze player.
        //example StartCoroutine(Camera.main.GetComponent<myTimer>().Counter());
        Vector2 boxSize = new Vector2(gameObject.GetComponent<BoxCollider2D>().bounds.size.x, gameObject.GetComponent<Collider2D>().bounds.size.y);
        RaycastHit2D hitPlayer = Physics2D.BoxCast(transform.position, boxSize, 0f, Vector2.down, 100f, playerMask);
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
        if (collisions[(int)CollisionsLoc.botColl] == TAGS.PLAYER 
            || collisions[(int)CollisionsLoc.leftColl] == TAGS.PLAYER 
            || collisions[(int)CollisionsLoc.rightColl] == TAGS.PLAYER)
        {
            enemy_moveScript.jumpedOff();
        }
    }
    public void jumpedOff()
    {
        rb.AddForce(new Vector2(0f, -jumpHeight));
    }
}
