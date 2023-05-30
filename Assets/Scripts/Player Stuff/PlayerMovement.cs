using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public PlayerData data;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public Transform wallCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private AudioSource jumpSFX;



    #region Variables
    public Rigidbody2D rb;
    private Collision coll;

    public Vector2 moveInput;
    public bool isFacingRight;

    [Space]
    [Header("Collision")]
    public bool isGrounded;
    public bool isOnPlatform;
    public bool isOnWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool canCornerCorrect;
    public bool canLedgeCorrect;
    public bool inWater;

    [Space]
    [Header("Jump")]
    public float lastOnAirTime;
    public float onGroundTime;
    public bool inAir;
    public bool isFalling;

    [Space]
    [Header("Wall Mechanics")]
    public bool isWallSliding;
    public bool isWallJumping;
    public bool isWallClimbing;
    public bool isWallGrabbing;




    [Space]
    [Header("Power Ups")]
    public bool isMoveSpeed;
    private bool moveSpeedInit;
    public bool isJumpBoost;
    public bool canDoubleJump;
    public bool doubleJumpPressed;
    public bool canDash;
    public bool dashPressed;
    public bool isDashing;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
    }
    // Start is called before the first frame update
    void Start()
    {
        isFacingRight = true;
        data.speed = data.defaultMoveSpeed;
        data.jumpPower = data.defaultJumpPower;
        data.stamina = data.staminaMax;

        SetGravityScale(data.gravityScale);
    }

    // Update is called once per frame
    void Update()
    {
        #region TIMERS
        // lastOnAirTime -= Time.deltaTime;
        // onGroundTime += Time.deltaTime;

        #endregion

        if (isDashing) // don't allow the player to move while dashing
        {
            return;
        }


        #region INPUT HANDLERS
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (!isOnWall) 
        {
            if (moveInput.x != 0)
                CheckDirectionToFace(moveInput.x > 0);
        }  
        

        #endregion

        #region TIMER AND BOOL CHECKS

        #region COYOTE TIMER
        if ((isGrounded || isOnWall || isWallSliding || isWallClimbing || isWallGrabbing) && !inWater) // can jump anytime when not in water
        {
            inAir = false;
            lastOnAirTime -= Time.deltaTime;
            data.coyoteTimeCounter = data.coyoteTime;
        }
        else if (isGrounded && inWater) // can only jump when grounded in water
        {
            inAir = false;
            lastOnAirTime -= Time.deltaTime;
            data.coyoteTimeCounter = data.coyoteTime;
        }
        else
        {
            inAir = true;
            onGroundTime = 0f;
            lastOnAirTime = 0f;
            data.coyoteTimeCounter -= Time.deltaTime;
        }

        if (rb.velocity.y > 0.1f || isGrounded)
        {
            isFalling = false;
        }
        else if (rb.velocity.y < -0.1f && inAir)
        {
            isFalling = true;
        }

        #endregion

        #region JUMP BUFFER
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.J))
        {
            data.jumpBufferTimeCounter = data.jumpBufferTime;
        }
        else
        {
            data.jumpBufferTimeCounter -= Time.deltaTime;
        }


        #endregion

        #endregion

        if (!inWater)
        {
            // Check if wall sliding
            WallSlide();
            // Check if can wall jump
            WallJump();
            // Check if can wall grab
            WallGrab();
            // Check if can wall climb
            WallClimb();
        }

        if (isWallGrabbing || isWallClimbing)
        {
            SetGravityScale(0);
        }
        else if (!inWater)
        {
            SetGravityScale(data.gravityScale);
        }

        // Checks collision detected by collision checkers
        CollisionCheck();

        // Check if can ledge correct
        LedgeCorrect();


        if (!isWallJumping && !isWallGrabbing && !isWallClimbing)
        {
            // Checks if sprite needs to be flipped depending on direction faced
            Flip();
        }


        // Check powerups

        UpdatePowerUps();

    }


    private void FixedUpdate()
    {
        if (isDashing) // don't allow the player to move while dashing
        {
            return;
        }

        #region JUMP 

        //code history for jump
        //if (Input.GetButtonDown("Jump") && isGrounded)
        //if (Input.GetButtonDown("Jump") && data.coyoteTimeCounter > 0f) // implement coyote timer
        if (data.jumpBufferTimeCounter > 0f && data.coyoteTimeCounter > 0f) // implement jump buffer
        {
            rb.velocity = new Vector2(rb.velocity.x, data.jumpPower);

            data.jumpBufferTimeCounter = 0f; // reset to 0 as soon as we jump.
        }
        if (Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.J) && rb.velocity.y > 0f)
        {
            jumpSFX.Play();
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * data.jumpCutPower);

            data.coyoteTimeCounter = 0f; // reset to 0 once jump button is realesed.
        }
        #endregion




        if (!isWallJumping && !isWallGrabbing && !isWallClimbing) // move player horizontally if not wall jumping
        {
            // move player horizontally
            Run();
        }

        #region STAMINA
        // regen stamina if grounded ONLY
        if (isGrounded && !inWater && data.stamina >= data.staminaMin && data.stamina < data.staminaMax)
        {
            data.stamina += data.staminaRegen;
        }
        // cap stamina at min and max
        if (data.stamina >= data.staminaMax)
        {
            data.stamina = data.staminaMax;
        }
        if (data.stamina <= data.staminaMin)
        {
            data.stamina = data.staminaMin;
        }

        #endregion



    }

    private void Run()
    {
        rb.velocity = new Vector2(moveInput.x * data.speed, rb.velocity.y);
    }





    #region WALL MECHANICS

    #region WALL GRAB
    private void WallGrab()
    {
        //if (!isWallJumping && isOnWall && (Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.LeftShift)))
        if (!isWallJumping && isOnWall && Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.J)) // can WJ while WG
            {
                StopWallGrab();
                PerformWallJump();
            }
            else
            {
                PerformWallGrab();
            }
        }
        /*else if (isWallGrabbing && Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopWallGrab();
        }
        
        if (!inWater && !isWallGrabbing)
        {
            StopWallGrab();
        }*/
        else
        {
            StopWallGrab();
        }
    }

    private void StopWallGrab()
    {
        isWallGrabbing = false;
        // SetGravityScale(data.gravityScale);
    }

    private void PerformWallGrab()
    {
        if (data.stamina != data.staminaMin)
        {
            isWallGrabbing = true;
            // drain stamina overtime
            data.stamina -= data.wallGrabStaminaDrain * Time.deltaTime;

            // stick to wall
            // call StickToWall() in case a bug occurs where player is wallgrabbing but slightly away from the wall
            StickToWall();

            // don't make the player move when only grabbing
            // SetGravityScale(0);

            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        else
        {
            StopWallGrab();
        }
    }



    #endregion

    #region WALL CLIMB
    private void WallClimb()
    {
        if (!isWallJumping && isOnWall && moveInput.y != 0 && isWallGrabbing)
        {
            /*isWallGrabbing = false;
            isWallClimbing = true;*/
            PerformWallClimb();
        }
        else
        {
            StopWallClimb();
        }
    }

    private void PerformWallClimb()
    {
        if (data.stamina != data.staminaMin)
        {
            isWallGrabbing = false;
            isWallClimbing = true;
            // drain stamina overtime
            data.stamina -= data.wallClimbStaminaDrain * Time.deltaTime;
            // don't make the player move when only grabbing

            // SetGravityScale(0);

            // wall climbing
            float speedModifier = moveInput.y > 0 ? data.wallClimbingSpeedUp : data.wallClimbingSpeedDown;
            rb.velocity = new Vector2(rb.velocity.x, moveInput.y * speedModifier);
        }
        else
        {
            StopWallClimb();
        }

    }

    private void StopWallClimb()
    {
        isWallClimbing = false;
    }
    #endregion

    #region WALL SLIDE
    private void WallSlide()
    {
        if (isOnWall && !isGrounded && moveInput.x != 0f && !isWallGrabbing)
        {

            PerformWallSlide();
        }
        else
        {
            StopWallSlide();
        }
    }

    private void PerformWallSlide()
    {
        isWallSliding = true;
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -data.wallSlidingSpeed, float.MaxValue));
    }

    private void StopWallSlide()
    {
        isWallSliding = false;
    }

    #endregion

    #region WALL JUMP
    private void WallJump()
    {
        if (isWallSliding || isWallGrabbing || isWallClimbing)
        {
            isWallJumping = false;
            data.wallJumpingDirection = -transform.localScale.x;
            data.wallJumpingCounter = data.wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            data.wallJumpingCounter -= Time.deltaTime;
        }

        //if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.J)) && data.wallJumpingCounter > 0f)
        if (data.jumpBufferTimeCounter > 0f && data.wallJumpingCounter > 0f)
        {
            PerformWallJump();
        }

        // allow the player to move in the air up until the peak of the wall jump height
        if (isWallJumping && inAir && !isFalling && moveInput.x != 0)
        {
            //rb.velocity = new Vector2(moveInput.x * data.wallJumpingPower.x / 2f, rb.velocity.y);
            Run();
        }
    }

    private void PerformWallJump()
    {
        if (data.stamina != data.staminaMin)
        {
            isWallJumping = true;
            data.stamina -= data.wallJumpStaminaDrain;

            isOnWall = false;
            /*isWallSliding = false;
            isWallGrabbing = false;*/
            data.wallJumpingCounter = 0f;
            if (moveInput.x == 0 || (onRightWall && moveInput.x == 1) || (onLeftWall && moveInput.x == -1))
            {
                //rb.velocity = new Vector2(0f, data.wallJumpingPower.y);
                if (moveInput.y != 0) // might change later
                {
                    //rb.velocity = new Vector2(rb.velocity.x * transform.localScale.x, data.wallJumpingPower.y + 2f);
                    rb.velocity = new Vector2(0f, data.wallJumpingPower.y + 2f);
                }
                else
                {
                    rb.velocity = new Vector2(0f, data.wallJumpingPower.y);
                }

            }
            else
            {
                rb.velocity = new Vector2(data.wallJumpingPower.x * data.wallJumpingDirection, data.wallJumpingPower.y);
                if (transform.localScale.x != data.wallJumpingDirection)
                {
                    PerformFlip();
                }
            }

            Invoke(nameof(StopWallJumping), data.wallJumpingDuration);

        }
        else
        {
            isWallJumping = false;
            data.wallJumpingDirection = -transform.localScale.x;
            data.wallJumpingCounter = data.wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));

        }

    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    #endregion

    #region LEDGE CORRECT
    private void LedgeCorrect()
    {
        if (canLedgeCorrect)
        {
            //SetGravityScale(data.gravityScale);
            isWallGrabbing = false;
            isWallSliding = false;
            isOnWall = false;
            canLedgeCorrect = false;
            if (moveInput.x == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, data.wallJumpingPower.y / 1.857f); // DO NOT CHANGE THIS
                StartCoroutine(AddRight());
            }
            else if (moveInput.x != 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, data.wallJumpingPower.y / 1.857f);
                //rb.velocity = new Vector2(rb.velocity.x, data.wallJumpingPower.y / 2f);
                /*isWallGrabbing = false;
                isWallSliding = false;
                isOnWall = false;
                canLedgeCorrect = false;*/
            }

            //transform.position = new Vector2(transform.position.x + (0.5f * transform.localScale.x), transform.position.y + 0.4f);
        }
    }

    IEnumerator AddRight()
    {
        yield return new WaitForSeconds(0.1f);
        rb.AddForce(Vector2.right * data.wallJumpingPower.x * 5f * transform.localScale.x);
        //rb.AddForce(Vector2.right * data.wallJumpingPower.x * 2f * transform.localScale.x);
    }

    #endregion

    #endregion


    #region COLLISION CHECK

    private void CollisionCheck()
    {
        GroundCollisionCheck();
        PlatformCollisionCheck();
        WallCollisionCheck();
        LedgeCollisionCheck();
        CornerCorrectCheck(); // TODO - have to fix this
        WaterCollisionCheck();
    }

    private void WaterCollisionCheck()
    {
        if (coll.inWater)
        {
            inWater = true;
        }
        else
        {
            inWater = false;
        }
    }

    private void GroundCollisionCheck()
    {
        //Ground Check
        //if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer)) //checks if set box overlaps with ground
        if (coll.onGround)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void PlatformCollisionCheck()
    {
        if (coll.onPlatform)
        {
            isOnPlatform = true;
            isGrounded = true;
        }
        else
        {
            isOnPlatform = false;
        }
    }

    private void WallCollisionCheck()
    {
        //if (Physics2D.OverlapCircle(wallCheck.position, 0.25f, groundLayer))
        if (coll.onWall) //this is bugged
        {
            isOnWall = true;
        }
        else
        {
            isOnWall = false;
        }

        if (isOnWall)
        {
            if (isFacingRight)
            {
                onRightWall = true;
                onLeftWall = false;
            }
            else
            {
                onRightWall = false;
                onLeftWall = true;
            }
        }
        else
        {
            onRightWall = false; onLeftWall = false;
        }
    }

    private void LedgeCollisionCheck()
    {
        if (!coll.canLedge && coll.onWall && (isWallGrabbing || isWallSliding || isWallClimbing) && !inAir)
        {
            canLedgeCorrect = true;
        }
        else
        {
            canLedgeCorrect = false;
        }
    }

    private void CornerCorrectCheck()
    {
        if (coll.canCornerCorrect)
        {
            canCornerCorrect = true;
        }
        else
        {
            canCornerCorrect = false;
        }

        if (canCornerCorrect && !isGrounded && !isOnWall && !isFalling)
        {
            CornerCorrect(rb.velocity.y);
        }
    }

    private void CornerCorrect(float yVelocity)
    {
        // Push player to the right
        // RaycastHit2D hit = Physics2D.Raycast(transform.position - data.innerRayCastOffset + Vector3.up * data.topRayCastLength, Vector3.left, data.topRayCastLength, groundLayer);
        RaycastHit2D hit = Physics2D.Raycast(transform.position - data.innerRayCastOffset + Vector3.up * data.topRayCastLength, Vector3.left, data.topRayCastLength, wallLayer);
        if (hit.collider != null)
        {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * data.topRayCastLength,
                transform.position - data.edgeRayCastOffset + Vector3.up * data.topRayCastLength);
            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, yVelocity);
            return;
        }

        // Push player to the left
        hit = Physics2D.Raycast(transform.position + data.innerRayCastOffset + Vector3.up * data.topRayCastLength, Vector3.right, data.topRayCastLength, groundLayer);
        if (hit.collider != null)
        {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * data.topRayCastLength,
                transform.position + data.edgeRayCastOffset + Vector3.up * data.topRayCastLength);
            transform.position = new Vector3(transform.position.x - newPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, yVelocity);
            return;
        }
    }

    #endregion


    #region POWER UPS
    private void UpdatePowerUps()
    {
        #region MOVESPEED
        if (isMoveSpeed)
        {
            // Initial move speed increase
            if (!moveSpeedInit)
            {
                data.speed = data.moveSpeedIncrease;
                moveSpeedInit = true;
            }

            data.moveSpeedTimer += Time.deltaTime;

            data.speed -= 0.002f; // decrement move speed over time
            if (data.moveSpeedTimer >= data.moveSpeedTimerCap)
            {
                data.speed = data.defaultMoveSpeed;
                data.moveSpeedTimer = 0f;
                moveSpeedInit = false;
                isMoveSpeed = false;
            }
        }
        #endregion

        #region DOUBLE JUMP
        if (canDoubleJump)
        {
            //if (Input.GetButtonDown("Jump"))
            if (data.jumpBufferTimeCounter > 0f) // replace with jump buffer
            {
                rb.velocity = new Vector2(rb.velocity.x, data.jumpPower);
                if (isGrounded)
                    doubleJumpPressed = false;
                else if (!isGrounded && !isWallJumping)
                    doubleJumpPressed = true;
            }

            // can DJ if picked DJ powerup from the ground
            if (!doubleJumpPressed && isGrounded && !inAir)
            {
                canDoubleJump = true;
            }
            // cant DJ anymore when picking a DJ powerup while midair AND landing on ground not using the DJ
            if (!doubleJumpPressed && isGrounded && inAir)
            {
                canDoubleJump = false;
            }
            // cant DJ anymore when picking a DJ powerup while midair AND wallsliding not using the DJ
            if (!doubleJumpPressed && isWallSliding)
            {
                canDoubleJump = false;
            }

        }

        if (doubleJumpPressed && !isGrounded)
        {
            canDoubleJump = false;
            if (isGrounded)
            {
                doubleJumpPressed = false;
            }
        }

        if (isGrounded && !canDoubleJump)
        {
            doubleJumpPressed = false;
        }
        #endregion

        #region JUMP BOOST
        if (isJumpBoost)
        {
            data.jumpPower = data.jumpBoostIncrease;
            data.jumpBoostTimer += Time.deltaTime;
            if (data.jumpBoostTimer >= data.jumpBoostTimerCap)
            {
                data.jumpPower = data.defaultJumpPower;
                data.jumpBoostTimer = 0f;
                isJumpBoost = false;
            }
        }
        #endregion

        #region DASH
        if (canDash) // moves the player 3.6 units in the x axis (if dash power is 24f)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                //dashPressed = true;
                //StartCoroutine(Dash());   
                StartCoroutine(_Dash());
                if (isGrounded)
                    dashPressed = false;
                else if (!isGrounded && !isDashing)
                    dashPressed = true;
            }


            // can dash if picked dash powerup from the ground
            if (!dashPressed && isGrounded && !inAir)
            {
                canDash = true;
            }
            // cant dash anymore when picking a dash powerup while midair AND landing on ground not using the dash
            if (!dashPressed && isGrounded && inAir)
            {
                canDash = false;
            }
            // cant dash anymore when picking a dash powerup while midair AND wallsliding not using the dash
            if (!dashPressed && isWallSliding)
            {
                canDash = false;
            }
        }

        if (dashPressed && !isGrounded)
        {
            canDash = false;
            if (isGrounded)
            {
                dashPressed = false;
            }
        }

        if (isGrounded && !canDash)
        {
            dashPressed = false;
        }

        #endregion
    }

    private IEnumerator _Dash()
    {
        canDash = false;
        SetGravityScale(0f); // set gravity to 0 while dashing
        PerformDash(); // perform dash
        yield return new WaitForSeconds(data.dashTime); // time while dashing
        SetGravityScale(data.gravityScale); // set gravity back to normal after dashing
        StopDash(); // stop isDashing bool
    }
    /*private IEnumerator Dash()
    {
        SetGravityScale(0f); // set gravity to 0 while dashing
        PerformDash(); // perform dash
        yield return new WaitForSeconds(data.dashTime); // time while dashing
        SetGravityScale(data.gravityScale); // set gravity back to normal after dashing
        StopDash(); // stop isDashing bool
        yield return new WaitForSeconds(data.dashCooldown); // dash cooldown

    }*/

    private void PerformDash()
    {
        isDashing = true;
        rb.velocity = new Vector2(transform.localScale.x * data.dashPower, 0f);
    }

    private void StopDash()
    {
        isDashing = false;
    }
    #endregion


    #region GENERAL METHODS
    private void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
        {
            PerformFlip();
        }
    }
    private void Flip()
    {
        if (isFacingRight && moveInput.x < 0f || !isFacingRight && moveInput.x > 0f)
        {
            PerformFlip();
        }
    }

    private void PerformFlip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    private void StickToWall()
    {
        //Push player torwards wall
        if (onRightWall && transform.localScale.x >= 0f)
        {
            rb.velocity = new Vector2(15f, rb.velocity.y);
        }
        else if (onLeftWall && transform.localScale.x <= 0f)
        {
            rb.velocity = new Vector2(-15f, rb.velocity.y);
        }

        //Face correct direction
        if (onRightWall && !isFacingRight)
        {
            PerformFlip();
        }
        else if (onLeftWall && isFacingRight)
        {
            PerformFlip();
        }
    }
    #endregion



    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(wallCheck.position, 0.25f);
        /*Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        Gizmos.color = Color.blue;*/
        /*Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);*/
    }
    #endregion
}
