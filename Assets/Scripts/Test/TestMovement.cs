using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    [SerializeField] public PlayerData data;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;



    #region Variables
    public Rigidbody2D rb;
    private Collision coll;

    public Vector2 moveInput;
    public bool isFacingRight;
    public bool canMove;

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
    public float lastOnGroundTime;
    public float onGroundTime;
    public float lastPressedJumpTime;
    public bool isJumping;
    public bool isJumpCut;
    public bool inAir;
    public bool isFalling;


    [Space]
    [Header("Wall Mechanics")]
    public float lastOnWallTime;
    public float lastOnWallRightTime;
    public float lastOnWallLeftTime;
    public bool isWallSliding;
    public bool isWallJumping;
    public float wallJumpStartTime;
    public int lastWallJumpDir;
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
        canMove = true;
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
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;
        lastOnWallRightTime -= Time.deltaTime;
        lastOnWallLeftTime -= Time.deltaTime;

        #endregion

        #region INPUT HANDLER
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (!isWallJumping && !isWallGrabbing && !isWallClimbing)
        {
            if (moveInput.x != 0)
            {
                CheckDirectionToFace(moveInput.x > 0);
            }
                
        }
            

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.J))
        {
            OnJumpUpInput();
        }

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K))
        {
            //OnDashInput();
        }
        #endregion

        #region COLLISION CHECKS
        CollisionCheck();

        if (isGrounded)
        {
            lastOnGroundTime = data.coyoteTime;
        }

        if (isOnWall)
        {
            lastOnWallTime = data.coyoteTime;
        }
        #endregion

        #region JUMP CHECKS

        if (isJumping && rb.velocity.y < 0 && (!isWallGrabbing && !isWallClimbing))
        {
            isJumping = false;

            if (!isWallJumping)
                isFalling = true;
        }

        /*if (isWallJumping && Time.time - wallJumpStartTime > data.wallJumpingTime)
        {
            isWallJumping = false;
        }*/

        if (lastOnGroundTime > 0 && !isJumping && !isWallJumping && (!isWallGrabbing && !isWallClimbing))
        {
            isJumpCut = false;

            if (!isJumping)
                isFalling = false;
        }

        if (!isDashing)
        {
            //Jump
            if (CanJump() && lastPressedJumpTime > 0)
            {
                isJumping = true;
                isWallJumping = false;
                isJumpCut = false;
                isFalling = false;
                Jump();

                //AnimHandler.startedJumping = true;
            }

            //WALL JUMP
            /*else if (CanWallJump() && lastPressedJumpTime > 0)
            {
                isWallJumping = true;
                isJumping = false;
                isJumpCut = false;
                isFalling = false;

                wallJumpStartTime = Time.time;            

                WallJump();
            }*/
        }
        #endregion

        LedgeCorrect();

        #region WALL CHECKS


        //WallJump();
        if (CanWallJump())
        {
            /*if (!canMove)
                return;*/
               
            /*isWallSliding = false;
            isWallGrabbing = false;
            isWallJumping = true;
            isWallClimbing = false;*/

            PerformWallJump();
        }
        else
        {
            //WallJump();
            isWallJumping = false;
        }
        if (CanWallClimb())
        {
            if (!canMove)
                return;
                
            isWallSliding = false;
            isWallGrabbing = false;
            isWallJumping = false;
            isWallClimbing = true;
            PerformWallClimb();
        }
        else
        {
            isWallClimbing = false;
        }
        if (CanWallGrab())
        {
            if (!canMove)
                return;
            isWallSliding = false;
            isWallGrabbing = true;
            isWallJumping = false;
            isWallClimbing = false;
            PerformWallGrab();
        }
        else
        {
            isWallGrabbing = false;
        }
        if (CanWallSlide())
        {
            if (!canMove)
                return;
            isWallSliding = true;
            isWallGrabbing = false;
            isWallJumping = false;
            isWallClimbing = false;
            PerformWallSlide();
        }
        else
        {
            isWallSliding = false;
        }



        #endregion

        #region GRAVITY
        if (!isWallGrabbing && !isDashing && !isWallClimbing)
        {
            if (isJumpCut)
            {
                //Higher gravity if jump button released
                SetGravityScale(data.fallGravityScale);
            }
            else if (inWater)
            {
                SetGravityScale(data.waterGravityScale);
            }
            else if ((isJumping || isWallJumping || isFalling) && Mathf.Abs(rb.velocity.y) < data.jumpHangTimeThreshold)
            {
                SetGravityScale(data.gravityScale * data.jumpHangGravityMult);
            }
            /*else if ((isFalling) && (rb.velocity.y < 0f))
            {
                SetGravityScale(data.fallGravityScale);
            }*/
            else
            {
                SetGravityScale(data.gravityScale);
            }
        }
        else
        {
            SetGravityScale(0);
        }
        
        #endregion

    }

    private void FixedUpdate()
    {

        if (!isWallClimbing && !isWallGrabbing) // move player horizontally if not wall jumping
        {
            // move player horizontally
            Run();
        }
    }

    #region WALL MECHANICS

    #region WALL SLIDE
    private void PerformWallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -data.wallSlidingSpeed, float.MaxValue));
    }
    #endregion

    #region WALL GRAB
    private void PerformWallGrab()
    {
        if (isWallGrabbing && lastPressedJumpTime > 0)
        {
            PerformWallJump();
        }
        StickToWall();
        rb.velocity = new Vector2(rb.velocity.x, 0);
        //rb.velocity = new Vector2(0, 0);
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

    #region WALL CLIMB
    private void PerformWallClimb()
    {
        if (isWallClimbing && lastPressedJumpTime > 0)
        {
            PerformWallJump();
        }
        // wall climbing
        float speedModifier = moveInput.y > 0 ? data.wallClimbingSpeedUp : data.wallClimbingSpeedDown;
        rb.velocity = new Vector2(rb.velocity.x, moveInput.y * speedModifier);
    }
    #endregion


    #region WALL JUMP

    private void PerformWallJump()
    {
        isWallSliding = false;
        isWallGrabbing = false;
        isWallJumping = true;
        isWallClimbing = false;
        data.wallJumpingDirection = -transform.localScale.x;

        if ((transform.localScale.x == 1 && isFacingRight) || (transform.localScale.x == -1 && !isFacingRight))
        {
            PerformFlip();
        }
        

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        rb.velocity = new Vector2(data.wallJumpingPower.x * data.wallJumpingDirection, data.wallJumpingPower.y);

    }
    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
    #endregion

    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        //Debug.Log("Jump");
        //Ensures we can't call Jump multiple times from one press
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        #region Perform Jump
        rb.AddForce(Vector2.up * data.jumpPower, ForceMode2D.Impulse);
        #endregion
    }
    #endregion


    #region COLLISION CHECKS
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
                lastOnWallRightTime = data.coyoteTime;
            }
            else
            {
                onRightWall = false;
                onLeftWall = true;
                lastOnWallLeftTime = data.coyoteTime;
            }
        }
        else
        {
            onRightWall = false; onLeftWall = false;
        }
    }

    private void LedgeCollisionCheck()
    {
        if (!coll.canLedge && coll.onWall && (isWallGrabbing || isWallSliding || isWallClimbing) && !inAir && !isWallJumping && !isJumpCut)
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
        if (coll.canCornerCorrect && !isOnWall)
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


    #region RUN METHODS
    private void Run()
    {
        if (!canMove)
            return;
        rb.velocity = new Vector2(moveInput.x * data.speed, rb.velocity.y);
    }
    #endregion

    #region LEDGE CORRECT
    private void LedgeCorrect()
    {
        if (!canMove) return;
        if (canLedgeCorrect)
        {
            StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(0.3f));
            Debug.Log("ledge corrected");
            //SetGravityScale(data.gravityScale);
            isWallGrabbing = false;
            isWallSliding = false;
            isOnWall = false;
            canLedgeCorrect = false;
            if (moveInput.x == 0)
            {
                //rb.velocity = new Vector2(rb.velocity.x, data.wallJumpingPower.y / 1.857f); // DO NOT CHANGE THIS
                rb.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);
                //rb.AddForce(Vector2.up * 4f, ForceMode2D.Impulse);
                StartCoroutine(AddRight());
            }
            else if (moveInput.x != 0)
            {
                //rb.velocity = new Vector2(rb.velocity.x, 14f);
                //rb.velocity = new Vector2(rb.velocity.x, data.wallJumpingPower.y / 1.857f);
                rb.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);
                StartCoroutine(AddRight());
                //rb.AddForce(Vector2.up * 4f, ForceMode2D.Impulse);

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
        rb.AddForce(Vector2.right * data.wallJumpingPower.x * 8f * transform.localScale.x);
        Debug.Log("add right");
        //rb.AddForce(Vector2.right * data.wallJumpingPower.x * 2f * transform.localScale.x);
    }

    #endregion


    #region CHECK METHODS

    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping;
    }
    private bool CanWallJump()
    {
        return lastPressedJumpTime > 0 && lastOnWallTime > 0 && lastOnGroundTime <= 0 && !isWallJumping;
    }
    private bool CanJumpCut()
    {
        return isJumping && rb.velocity.y > 0;
    }

    private bool CanWallSlide()
    {
        return lastOnWallTime > 0 && (!isGrounded) && moveInput.x != 0f && !isWallGrabbing && !isWallClimbing && !isWallJumping;
    }
    
    private bool CanWallGrab()
    {
        return lastOnWallTime > 0 && Input.GetKey(KeyCode.LeftShift) && !isWallClimbing && !isWallJumping;
    }

    private bool CanWallClimb()
    {
        //return lastOnWallTime > 0 && (moveInput.y != 0) && Input.GetKey(KeyCode.LeftShift);
        if (lastOnWallTime > 0 && !isWallJumping)
        {
            if (Input.GetKey(KeyCode.LeftShift) && ((isFacingRight && lastOnWallRightTime > 0) || (!isFacingRight && lastOnWallLeftTime > 0)))
            {
                if (moveInput.y != 0 && moveInput.x != 0)
                {
                    return true;
                }
                if (moveInput.y != 0 && moveInput.x == 0)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }
    #endregion


    #region INPUT CALLBACKS
    //Methods which whandle input detected in Update()
    public void OnJumpInput()
    {
        lastPressedJumpTime = data.jumpBufferTime;
    }

    public void OnJumpUpInput()
    {
        isJumpCut = true;
    }

    private bool CanWallJumpCut()
    {
        return isWallJumping && rb.velocity.y > 0;
    }

    /*public void OnDashInput()
    {
        LastPressedDashTime = Data.dashInputBufferTime;
    }*/
    #endregion



    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    private void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
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
    #endregion


}
