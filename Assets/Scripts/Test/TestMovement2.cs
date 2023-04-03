using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement2 : MonoBehaviour
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
    private bool changingDirection;

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
        data.maxFallTimer = 0;
        data.jumpBoostTimer = 0;
        data.moveSpeedTimer = 0;


        SetGravityScale(data.gravityScale);
    }

    private void Update()
    {
        #region INPUT HANDLER
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Jump") || (Input.GetKeyDown(KeyCode.J)))
        {
            data.jumpBufferTimeCounter = data.jumpBufferTime;
        }
        else
        {
            data.jumpBufferTimeCounter -= Time.deltaTime;
        }

        changingDirection = (rb.velocity.x > 0f && moveInput.x < 0f) || (rb.velocity.x < 0f && moveInput.x > 0f);
        if ((moveInput.x < 0f && isFacingRight || moveInput.x > 0f && !isFacingRight) && !CanWallGrab() && !CanWallSlide())
        {
            Flip();
        }

        #endregion
    }

    private void FixedUpdate()
    {
        CollisionCheck();
        if (canDash)
        {

        }
        if (!isDashing)
        {
            if (canMove)
            {
                MoveCharacter();
            }
            else
            {
                rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(moveInput.x * data.runMaxSpeed, rb.velocity.y)), .5f * Time.deltaTime);
            }

            if (isGrounded)
            {
                ApplyGroundLinearDrag();
                data.hangTimeCounter = data.jumpHangGravityMult;
                dashPressed = false;
            }
            else
            {
                ApplyAirLinearDrag();
                FallMultiplier();
                data.hangTimeCounter -= Time.fixedDeltaTime;
                //if (!isOnWall || rb.velocity.y < 0f || _wallRun) _isJumping = false;
                if (!isOnWall || rb.velocity.y < 0f) isJumping = false;
            }

            if (CanJump())
            {
                if (isOnWall && !isGrounded)
                {
                    /*if (!_wallRun && (_onRightWall && _horizontalDirection > 0f || !_onRightWall && _horizontalDirection < 0f))
                    {
                        StartCoroutine(NeutralWallJump());
                    }
                    else
                    {
                        WallJump();
                    }*/
                    if (data.stamina != data.staminaMin)
                        WallJump();

                }
                else
                {
                    Jump(Vector2.up);
                }
            }

            if (!isJumping)
            {
                if (CanWallSlide())
                {
                    WallSlide();
                }
                if (CanWallGrab())
                {
                    WallGrab();
                }
                if (CanWallClimb())
                {
                    WallClimb();
                }
                else
                {
                    isWallClimbing = false;
                }

                /*if (CanWallGrab()) WallGrab();
                if (CanWallClimb()) WallRun();
                if (isOnWall) StickToWall();*/
                if (isOnWall)
                {
                    StickToWall();
                }
            }

        }

        #region STAMINA
        // regen stamina if grounded ONLY
        if (isGrounded && !inWater && data.stamina >= data.staminaMin && data.stamina < data.staminaMax)
        {
            //data.stamina += data.staminaRegen;
            data.stamina = data.staminaMax;
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

    #region WALL MECHANICS

    private void WallClimb()
    {
        data.stamina -= data.wallClimbStaminaDrain * Time.deltaTime;

        float speedModifier = moveInput.y > 0 ? data.wallClimbingSpeedUp : data.wallClimbingSpeedDown;
        rb.velocity = new Vector2(rb.velocity.x, moveInput.y * speedModifier);
        isWallClimbing = true;

    }
    private void WallGrab()
    {
        data.stamina -= data.wallClimbStaminaDrain * Time.deltaTime;

        SetGravityScale(0);
        rb.velocity = Vector2.zero;
    }
    private void WallSlide()
    {
        //rb.velocity = new Vector2(rb.velocity.x, -data.runMaxSpeed * data.wallSlidingSpeed);

        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -data.wallSlidingSpeed, float.MaxValue));
    }

    private void WallJump()
    {
        data.stamina -= data.wallJumpStaminaDrain;

        //Debug.Log("true");
        Vector2 jumpDirection = onRightWall ? Vector2.left : Vector2.right;
        //Jump(Vector2.up + jumpDirection);

        Vector2 direction = Vector2.up + jumpDirection;

        //Debug.Log(direction);

        ApplyAirLinearDrag();
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        if (isWallClimbing)
        {
            rb.AddForce(new Vector2(0, data.wallJumpingPower.y), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(direction * data.wallJumpingPower, ForceMode2D.Impulse);
            Flip();
        }

        data.hangTimeCounter = 0f;
        data.jumpBufferTimeCounter = 0f;
        isJumping = true;
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
            Flip();
        }
        else if (onLeftWall && isFacingRight)
        {
            Flip();
        }
    }
    #endregion



    #region MOVE METHODS

    private void MoveCharacter()
    {
        rb.AddForce(new Vector2(moveInput.x, 0f) * data.runAcceleration);

        if (Mathf.Abs(rb.velocity.x) > data.runMaxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * data.runMaxSpeed, rb.velocity.y);
    }

    private void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(moveInput.x) < 0.4f || changingDirection)
        {
            rb.drag = data.groundLinearDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private void ApplyAirLinearDrag()
    {
        rb.drag = data.airLinearDrag;
    }

    private void FallMultiplier()
    {
        if (moveInput.y < 0f)
        {
            //rb.gravityScale = data.fallGravityScale;
            SetGravityScale(data.forcedFallGravityScale);
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                data.maxFallTimer += Time.deltaTime;
                //rb.gravityScale = data.fallGravityScale;
                // vary the gravity when falling for x amount of time
                if (data.maxFallTimer >= data.maxFallTimerCap - 1f && data.maxFallTimer < data.maxFallTimerCap)
                {
                    SetGravityScale(data.minFallGravityScale);
                }
                else if (data.maxFallTimer >= data.maxFallTimerCap)
                {
                    SetGravityScale(data.maxFallGravityScale);
                }
                else
                {
                    SetGravityScale(data.fallGravityScale);
                }

            }
            // instant fall gravity
            else if (rb.velocity.y > 0 && !(Input.GetButton("Jump") || Input.GetKey(KeyCode.J)))
            {
                //rb.gravityScale = data.fallGravityScale;
                SetGravityScale(data.fallGravityScale);
            }
            // jump gravity
            else
            {
                //rb.gravityScale = 3f;
                SetGravityScale(data.jumpGravityScale);
            }
        }
    }

    #endregion

    #region JUMP METHODS
    private void Jump(Vector2 direction)
    {
        /*if (!isGrounded && !isOnWall)
            _extraJumpsValue--;*/

        ApplyAirLinearDrag();
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(direction * data.jumpPower, ForceMode2D.Impulse);
        data.hangTimeCounter = 0f;
        data.jumpBufferTimeCounter = 0f;
        data.maxFallTimer = 0f;
        isJumping = true;
    }


    #endregion

    #region CHECK METHODS
    private bool CanJump()
    {
        //return lastOnGroundTime > 0 && !isJumping;

        return data.jumpBufferTimeCounter > 0f && (data.hangTimeCounter > 0f || isOnWall);
    }
    private bool CanWallSlide()
    {
        return isOnWall && (data.stamina != data.staminaMin) && !isGrounded && !Input.GetKeyDown(KeyCode.LeftShift) && rb.velocity.y < 0f && ((moveInput.x > 0 && isFacingRight) || (moveInput.x < 0 && !isFacingRight));
    }

    private bool CanWallGrab()
    {
        return isOnWall && (data.stamina != data.staminaMin) && Input.GetKey(KeyCode.LeftShift);
    }

    private bool CanWallClimb()
    {
        return isOnWall && (data.stamina != data.staminaMin) && Input.GetKey(KeyCode.LeftShift) && moveInput.y != 0;
    }

    /*private bool CanWallClimb()
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
    }*/

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

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        //transform.Rotate(0f, 180f, 0f);
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    #endregion
}
