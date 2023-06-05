using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SugboMovement : MonoBehaviour, IDataPersistence
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Slider staminaWheel;
    [SerializeField] private Slider usageWheel;
    private BodySpriteSetter bodySpriteSetter;
    public Rigidbody2D rb;
    private Collision coll;
    public PlayerDeath death;
    public Animator animator;

    #region Variables

    public Vector2 moveInput;
    public bool isFacingRight;
    public static bool canMove;
    public static bool isDead;
    private bool changingDirection;

    [Space]
    [Header("Collision")]
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isOnPlatform;
    [HideInInspector] public bool isOnWall;
    [HideInInspector] public bool onRightWall;
    [HideInInspector] public bool onLeftWall;
    [HideInInspector] public bool canLedgeCorrect;
    [HideInInspector] public bool inWater;

    [Space]
    [Header("Jump")]
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isJumpCut;
    [HideInInspector] public bool inAir;
    [HideInInspector] public bool isFalling;

    [Space]
    [Header("Wall Mechanics")]
    [HideInInspector] public float lastOnWallTime;
    [HideInInspector] public bool isWallSliding;
    [HideInInspector] public bool isWallJumping;
    [HideInInspector] public bool isWallClimbing;
    [HideInInspector] public bool isWallGrabbing;

    [Space]
    [Header("Power Ups")]
    [HideInInspector] public bool isMoveSpeed;
    [HideInInspector] private bool moveSpeedInit;
    [HideInInspector] public bool isJumpBoost;
    [HideInInspector] public bool canDoubleJump;
    [HideInInspector] public bool doubleJumpPressed;
    [HideInInspector] public bool canDash;
    [HideInInspector] public bool dashPressed;
    [HideInInspector] public bool isDashing;

    #endregion

    #region Player Data
    [Space(20)]
    [Header("Player Data")]
    [Space(10)]
    [Header("Stamina")]
    public float stamina;
    public float wallGrabStaminaDrain;
    public float wallJumpStaminaDrain;
    public float wallClimbStaminaDrain;
    public float waterStaminaDrain;
    public float staminaRegen;
    public float staminaMax;
    public float staminaMin;


    [Header("Gravity")]
    public float gravityScale;
    public float jumpGravityScale;
    public float fallGravityScale;
    public float minFallGravityScale;
    public float maxFallGravityScale;
    public float maxFallTimer;
    [Range(0f, 3)] public float maxFallTimerCap;
    public float forcedFallGravityScale;
    public float waterGravityScale;

    [Header("Run")]
    public float defaultMoveSpeed;
    public float runMaxSpeed;
    public float runAcceleration;
    public float groundLinearDrag;


    [Space]
    [Header("Jump")]
    public float defaultJumpPower;
    public float jumpPower;
    public float jumpCutPower;
    public float hangTimeCounter;
    public float jumpHangTimeThreshold;
    [Range(0f, 1)] public float jumpHangGravityMult;
    public float airLinearDrag;
    public float fallMultiplier;

    [Space]
    [Header("Wall Mechanics")]
    public float wallSlidingSpeed;
    public Vector2 wallJumpingPower;
    // wall climb
    [Range(0.01f, 10f)] public float wallClimbingSpeedUp;
    [Range(0.01f, 10f)] public float wallClimbingSpeedDown;

    [Space]
    [Header("Power Ups")]
    // Move Speed
    public float moveSpeedTimer;
    public float moveSpeedTimerCap;
    public float moveSpeedIncrease;
    // Jump Boost
    public float jumpBoostTimer;
    public float jumpBoostTimerCap;
    public float jumpBoostIncrease;
    // Dash
    public float dashPower;
    public float dashTime;

    [Space]
    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
    public float coyoteTimeCounter;
    [Range(0.01f, 0.5f)] public float jumpBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.
    public float jumpBufferTimeCounter;
    #endregion

    private void Awake()
    {
        bodySpriteSetter = GetComponent<BodySpriteSetter>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        death = GetComponent<PlayerDeath>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        isFacingRight = true;
        canMove = true;
        isDead = false;
        runMaxSpeed = defaultMoveSpeed;
        jumpPower = defaultJumpPower;
        stamina = staminaMax;
        maxFallTimer = 0;
        jumpBoostTimer = 0;
        moveSpeedTimer = 0;

        SetGravityScale(gravityScale);
        GameObject baseRespawn = GameObject.FindGameObjectWithTag("BaseRespawn");
        if (NewDataPersistenceManager.instance.gameData.newGame)
        {
            Debug.Log("New Game");    

            if (baseRespawn != null)
            {
                Debug.Log($"{baseRespawn} || {baseRespawn.transform.position}");
                NewDataPersistenceManager.instance.gameData.respawnPoint = baseRespawn.transform.position;
                NewDataPersistenceManager.instance.gameData.position = baseRespawn.transform.position;

                // transform.position = NewDataPersistenceManager.instance.gameData.position;
            }
        }
        /*else if (NewDataPersistenceManager.instance.gameData.previousSceneIndex < SceneManager.GetActiveScene().buildIndex)
        {
            if (baseRespawn != null)
            {
                NewDataPersistenceManager.instance.gameData.respawnPoint = baseRespawn.transform.position;
                NewDataPersistenceManager.instance.gameData.position = baseRespawn.transform.position;

                // transform.position = NewDataPersistenceManager.instance.gameData.position;
            }
        }*/
        else
        {
            Debug.Log("Not New Game");
            NewDataPersistenceManager.instance.LoadGame();
        }
       
        transform.position = NewDataPersistenceManager.instance.gameData.position;
    }

    private void Update()
    {

        UpdateAnimation();

        #region TIMERS
        lastOnWallTime += Time.deltaTime;
        #endregion

        if (canMove && !isDead)
        {
            #region INPUT HANDLER
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Jump") || (Input.GetKeyDown(KeyCode.J)))
            {
                jumpBufferTimeCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferTimeCounter -= Time.deltaTime;
            }

            changingDirection = (rb.velocity.x > 0f && moveInput.x < 0f) || (rb.velocity.x < 0f && moveInput.x > 0f);
            if ((moveInput.x < 0f && isFacingRight || moveInput.x > 0f && !isFacingRight) && !CanWallGrab() && !CanWallSlide())
            {
                Flip();
            }

            #endregion
        }
        CollisionCheck();
        LedgeCollisionCheck();
        UpdatePowerUps();

        if (canLedgeCorrect) LedgeCorrect();

        #region AIR STUFF
        if ((isGrounded || isOnWall || isWallSliding || isWallClimbing || isWallGrabbing) && !inWater) // can jump anytime when not in water
        {
            inAir = false;
            coyoteTimeCounter = coyoteTime;
        }
        else if (isGrounded && inWater) // can only jump when grounded in water
        {
            inAir = false;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            inAir = true;
            coyoteTimeCounter -= Time.deltaTime;
        }
        #endregion

        if (CanJump())
        {
            if (isOnWall && !isGrounded)
            {
                if (stamina != staminaMin)
                {
                    if ((moveInput.x > 0 && isFacingRight && onRightWall) || (moveInput.x < 0 && !isFacingRight && onLeftWall))
                    {
                        NeutralWallJump();
                    }
                    else
                    {
                        WallJump();
                    }
                }
            }
            else
            {
                if (inWater && !isGrounded)
                {
                    return;
                }
                else
                {
                    if (isGrounded || canDoubleJump || coyoteTimeCounter > 0f)
                        Jump(Vector2.up);
                }
            }
        }

        if (!isGrounded && isOnWall && !isWallJumping)
        {
            if (CanWallJump())
            {
                if ((moveInput.x > 0 && isFacingRight && onRightWall) || (moveInput.x < 0 && !isFacingRight && onLeftWall))
                {
                    NeutralWallJump();
                }
                else
                {
                    WallJump();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            if (canMove && !isDead)
            {
                MoveCharacter();
            }
            else if (!canMove && !isDead)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(moveInput.x * runMaxSpeed, rb.velocity.y)), .5f * Time.deltaTime);
            }

            if (isGrounded)
            {
                ApplyGroundLinearDrag();
                hangTimeCounter = jumpHangGravityMult;
            }
            else
            {
                if (!inWater)
                {
                    ApplyAirLinearDrag();
                    FallMultiplier();
                    hangTimeCounter -= Time.fixedDeltaTime;
                    if (!isOnWall || rb.velocity.y < 0f) isJumping = false;
                }
            }

            if (inWater)
            {
                if (CanWaterJump())
                {
                    stamina -= waterStaminaDrain * 2;
                    Jump(Vector2.up);
                }
                if (moveInput.y < 0f)
                {
                    SetGravityScale(3);
                }
            }

            if (!isJumping && !inWater)
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

                if (isOnWall && !isGrounded)
                {
                    StickToWall();
                }
            }
        }

        UpdateStamina();
    }

    private void UpdateStamina()
    {
        if (stamina >= staminaMax)
        {
            staminaWheel.gameObject.SetActive(false);
            usageWheel.gameObject.SetActive(false);
        }
        else
        {
            staminaWheel.gameObject.SetActive(true);
            usageWheel.gameObject.SetActive(true);
        }
        // regen stamina if grounded and not in water ONLY
        if (isGrounded && !inWater && stamina >= staminaMin && stamina < staminaMax)
        {
            stamina += staminaRegen * Time.deltaTime;
            // stamina = staminaMax;
        }
        // cap stamina at min and max
        if (stamina >= staminaMax)
        {
            stamina = staminaMax;
        }
        if (stamina <= staminaMin)
        {
            stamina = staminaMin;
        }

        if (inWater && stamina <= staminaMin)
        {
            StartCoroutine(death.StartRespawn());
        }

        staminaWheel.value = (stamina / staminaMax);
        usageWheel.value = (stamina / staminaMax) + 0.1f;
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
        animator.SetFloat("Vertical", Mathf.Abs(moveInput.y));

        // jumping while on wall
        if (isJumping && rb.velocity.y > 0.01f && !isGrounded && isOnWall)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Idling", false);
            animator.SetBool("Climbing", false);
            animator.SetBool("Grabbing", false);
            animator.SetBool("Falling", false);
            animator.SetBool("Jumping", true);
            animator.SetBool("DoubleJumping", false);
        }
        // sliding
        if (CanWallSlide())
        {
            animator.SetBool("Running", false);
            animator.SetBool("Idling", false);
            animator.SetBool("Climbing", false);
            animator.SetBool("Grabbing", true);
            animator.SetBool("Falling", false);
            animator.SetBool("Jumping", false);
            animator.SetBool("DoubleJumping", false);
        }
        // swimming
        if (inWater)
        {
            if (isGrounded)
            {
                animator.SetBool("Swimming", false);
                animator.SetBool("Climbing", false);
                animator.SetBool("Grabbing", false);
                animator.SetBool("Falling", false);
                animator.SetBool("Jumping", false);
                animator.SetBool("DoubleJumping", false);
                if (moveInput.x != 0)
                {
                    animator.SetBool("Running", true);
                    animator.SetBool("Idling", false);
                }
                else
                {
                    animator.SetBool("Running", false);
                    animator.SetBool("Idling", true);
                }
            }
            else
            {
                animator.SetBool("Running", false);
                animator.SetBool("Idling", false);
                animator.SetBool("Swimming", true);
                animator.SetBool("Climbing", false);
                animator.SetBool("Grabbing", false);
                animator.SetBool("Falling", false);
                animator.SetBool("Jumping", false);
                animator.SetBool("DoubleJumping", false);
            }
        }
        else
        {
            animator.SetBool("Swimming", false);
        }
        // grabbing and climbing
        if (CanWallGrab() && !CanWallSlide() && !CanJump() && lastOnWallTime < 0.01f)
        {
            if (CanWallClimb())
            {
                animator.SetBool("Running", false);
                animator.SetBool("Idling", false);
                animator.SetBool("Climbing", true);
                animator.SetBool("Grabbing", false);
                animator.SetBool("Falling", false);
                animator.SetBool("Jumping", false);
                animator.SetBool("DoubleJumping", false);
            }
            else
            {
                animator.SetBool("Running", false);
                animator.SetBool("Idling", false);
                animator.SetBool("Climbing", false);
                animator.SetBool("Grabbing", true);
                animator.SetBool("Falling", false);
                animator.SetBool("Jumping", false);
                animator.SetBool("DoubleJumping", false);
            }
        }
        // running
        if (canMove && !isDead && isGrounded && moveInput.x != 0)
        {
            // animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
            animator.SetBool("Running", true);
            animator.SetBool("Idling", false);
            animator.SetBool("Climbing", false);
            animator.SetBool("Grabbing", false);
            animator.SetBool("Falling", false);
            animator.SetBool("Jumping", false);
            animator.SetBool("DoubleJumping", false);
        }
        // idling 
        if (moveInput.x == 0 && !CanWallGrab() && !CanWallClimb() && !CanWallSlide() && !isJumping && isGrounded)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Idling", true);
            animator.SetBool("Climbing", false);
            animator.SetBool("Grabbing", false);
            animator.SetBool("Falling", false);
            animator.SetBool("Jumping", false);
            animator.SetBool("DoubleJumping", false);
        }
        // jumping
        if (isJumping && rb.velocity.y > 0.01f && !isGrounded && !isOnWall && lastOnWallTime > 0.01f)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Idling", false);
            animator.SetBool("Climbing", false);
            animator.SetBool("Grabbing", false);
            animator.SetBool("Jumping", true);
            animator.SetBool("Falling", false);
            animator.SetBool("DoubleJumping", false);
        }
        // falling
        if (!isJumping && rb.velocity.y < 0.01f && !isGrounded && !isOnWall && !CanWallSlide())
        {
            animator.SetBool("Running", false);
            animator.SetBool("Idling", false);
            animator.SetBool("Climbing", false);
            animator.SetBool("Grabbing", false);
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", true);
            animator.SetBool("DoubleJumping", false);
        }
        // double jump
        if (doubleJumpPressed && !isGrounded && !isOnWall)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Idling", false);
            animator.SetBool("Climbing", false);
            animator.SetBool("Grabbing", false);
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
            animator.SetBool("DoubleJumping", true);
        }
    }

    // Data Persistence
    #region SAVE STUFF
    public void LoadData(GameData data)
    {
        Debug.Log($"LOADDATA {data.position}");
        transform.position = data.position;
        if (bodySpriteSetter != null) bodySpriteSetter.SetPlayerSprites();
    }

    public void SaveData(GameData data)
    {
        Debug.Log("SAVE DATA");
        if (PlayerDeath.currentRespawn != null)
        {
            data.respawnPoint = PlayerDeath.currentRespawn.transform.position;
            data.position = data.respawnPoint;
            data.sceneIndex = SceneManager.GetActiveScene().buildIndex;

            // HARD CODED CHANGE THIS IF NECESSARY
            // 5 FOR CITY 1, 6 FOR CITY 2, 7 FOR CITY 3, etc.
            if (data.sceneIndex >= 5)
            {
                data.newGame = false;
            }
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            Debug.Log("New Boundary");
            NewDataPersistenceManager.instance.SaveGame();
        }
    }

    #endregion

    #region WALL MECHANICS

    #region LEDGE CORRECT
    private void LedgeCorrect()
    {
        if (canLedgeCorrect)
        {
            canMove = false;
            isWallGrabbing = false;
            isWallSliding = false;
            isOnWall = false;
            canLedgeCorrect = false;
            if (moveInput.x == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 16);
                StartCoroutine(AddRight());

            }
            else if (moveInput.x != 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, wallJumpingPower.y / 1.857f);
                canMove = true;
            }
        }
    }
    IEnumerator AddRight()
    {
        yield return null;
        rb.velocity = new Vector2(transform.localScale.x * 20f, rb.velocity.y);
        canMove = true;
    }
    #endregion

    private void WallClimb()
    {
        stamina -= wallClimbStaminaDrain * Time.deltaTime;
        float speedModifier = moveInput.y > 0 ? wallClimbingSpeedUp : wallClimbingSpeedDown;
        rb.velocity = new Vector2(rb.velocity.x, moveInput.y * speedModifier);
        isWallClimbing = true;
    }
    
    private void WallGrab()
    {
        stamina -= wallClimbStaminaDrain * Time.deltaTime;
        SetGravityScale(0);
        rb.velocity = Vector2.zero;
    }
    
    private void WallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
    }

    private void NeutralWallJump()
    {
        stamina -= wallJumpStaminaDrain;
        ApplyAirLinearDrag();
        rb.velocity = new Vector2(0f, wallJumpingPower.y + 2f);
        hangTimeCounter = 0f;
        jumpBufferTimeCounter = 0f;
        isJumping = true;
    }

    private void WallJump()
    {
        stamina -= wallJumpStaminaDrain;
        Vector2 jumpDirection = onRightWall ? Vector2.left : Vector2.right;
        ApplyAirLinearDrag();
        if (isWallClimbing)
        {
            rb.velocity = new Vector2(0f, wallJumpingPower.y);
        }
        else
        {
            rb.velocity = new Vector2(wallJumpingPower.x * jumpDirection.x, wallJumpingPower.y);
            Flip();
        }

        hangTimeCounter = 0f;
        jumpBufferTimeCounter = 0f;
        isJumping = true;

        animator.SetBool("Running", false);
        animator.SetBool("Idling", false);
        animator.SetBool("Climbing", false);
        animator.SetBool("Grabbing", false);
        animator.SetBool("Falling", false);
        animator.SetBool("Jumping", true);
        animator.SetBool("DoubleJumping", false);
    }
    private void StickToWall()
    {
        //Push player torwards wall
        if (onRightWall && transform.localScale.x >= 0f)
        {
            rb.velocity = new Vector2(5f, rb.velocity.y);
        }
        else if (onLeftWall && transform.localScale.x <= 0f)
        {
            rb.velocity = new Vector2(-5f, rb.velocity.y);
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

    #region POWERUPS
    private void UpdatePowerUps()
    {
        #region MOVESPEED
        if (isMoveSpeed)
        {
            // Initial move speed increase
            if (!moveSpeedInit)
            {
                runMaxSpeed = moveSpeedIncrease;
                moveSpeedInit = true;
            }

            moveSpeedTimer += Time.deltaTime;

            runMaxSpeed -= Time.deltaTime; // decrement move speed over time
            if (moveSpeedTimer >= moveSpeedTimerCap)
            {
                runMaxSpeed = defaultMoveSpeed;
                moveSpeedTimer = 0f;
                moveSpeedInit = false;
                isMoveSpeed = false;
            }
        }
        #endregion

        #region DOUBLE JUMP
        if (canDoubleJump)
        {
            if (jumpBufferTimeCounter > 0f) // replace with jump buffer
            {
                Jump(Vector2.up);
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
            jumpPower = jumpBoostIncrease;
            jumpBoostTimer += Time.deltaTime;
            if (jumpBoostTimer >= jumpBoostTimerCap)
            {
                jumpPower = defaultJumpPower;
                jumpBoostTimer = 0f;
                isJumpBoost = false;
            }
        }
        #endregion

        #region DASH

        if (canDash)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(_Dash());
                if (isGrounded)
                    dashPressed = true;
                else if (!isGrounded && !isDashing)
                    dashPressed = false;
            }
            if (dashPressed && isGrounded)
            {
                canDash = false;
            }
            // can dash if picked dash powerup from the ground
            else if (!dashPressed && isGrounded && !inAir)
            {
                canDash = true;
            }
            // cant dash anymore when picking a dash powerup while midair AND landing on ground not using the dash
            else if (!dashPressed && isGrounded && inAir)
            {
                canDash = false;
            }
            // cant dash anymore when picking a dash powerup while midair AND wallsliding not using the dash
            else if (!dashPressed && isWallSliding)
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
        canMove = false;
        SetGravityScale(0f); // set gravity to 0 while dashing
        PerformDash(); // perform dash
        yield return new WaitForSeconds(dashTime); // time while dashing
        SetGravityScale(gravityScale); // set gravity back to normal after dashing
        StopDash(); // stop isDashing bool
        canMove = true;
    }

    private void PerformDash()
    {
        dashPressed = true;
        canDash = false;
        isDashing = true;
        rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
    }

    private void StopDash()
    {
        isDashing = false;
    }

    #endregion

    #region MOVE METHODS
    private void MoveCharacter()
    {
        rb.AddForce(new Vector2(moveInput.x, 0f) * runAcceleration);

        if (Mathf.Abs(rb.velocity.x) > runMaxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * runMaxSpeed, rb.velocity.y);
    }

    private void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(moveInput.x) < 0.4f || changingDirection)
        {
            rb.drag = groundLinearDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private void ApplyAirLinearDrag()
    {
        rb.drag = airLinearDrag;
    }

    private void FallMultiplier()
    {
        if (moveInput.y < 0f)
        {
            SetGravityScale(forcedFallGravityScale);
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                maxFallTimer += Time.deltaTime;
                // vary the gravity when falling for x amount of time
                if (maxFallTimer >= maxFallTimerCap - 1f && maxFallTimer < maxFallTimerCap)
                {
                    SetGravityScale(minFallGravityScale);
                }
                else if (maxFallTimer >= maxFallTimerCap)
                {
                    SetGravityScale(maxFallGravityScale);
                }
                else
                {
                    SetGravityScale(fallGravityScale);
                }
            }
            // instant fall gravity
            else if (rb.velocity.y > 0 && !(Input.GetButton("Jump") || Input.GetKey(KeyCode.J)))
            {
                SetGravityScale(fallGravityScale);
                coyoteTimeCounter = 0;
            }
            // jump gravity
            else
            {
                SetGravityScale(jumpGravityScale);
            }
        }
    }

    #endregion

    #region JUMP METHODS
    private void Jump(Vector2 direction)
    {
        ApplyAirLinearDrag();
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(direction * jumpPower, ForceMode2D.Impulse);
        hangTimeCounter = 0f;
        jumpBufferTimeCounter = 0f;
        maxFallTimer = 0f;
        isJumping = true;
    }


    #endregion

    #region CHECK METHODS
    private bool CanJump()
    {
        return jumpBufferTimeCounter > 0f && (hangTimeCounter > 0f || isOnWall) && coyoteTimeCounter > 0f;
    }

    private bool CanWallJump()
    {
        return jumpBufferTimeCounter > 0f && !isGrounded && (stamina != staminaMin) && !inWater;
    }

    private bool CanWaterJump()
    {
        return jumpBufferTimeCounter > 0f && inWater;
    }
    private bool CanWallSlide()
    {
        return isOnWall && (stamina != staminaMin) && !isGrounded && !Input.GetKeyDown(KeyCode.LeftShift) && rb.velocity.y < 0f && ((moveInput.x > 0 && isFacingRight) || (moveInput.x < 0 && !isFacingRight));
    }

    private bool CanWallGrab()
    {
        return isOnWall && (stamina != staminaMin) && Input.GetKey(KeyCode.LeftShift);
    }

    private bool CanWallClimb()
    {
        return isOnWall && (stamina != staminaMin) && Input.GetKey(KeyCode.LeftShift) && moveInput.y != 0;
    }

    #endregion

    #region COLLISION CHECK

    private void CollisionCheck()
    {
        GroundCollisionCheck();
        PlatformCollisionCheck();
        WallCollisionCheck();
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
        if (coll.onWall) 
        {
            isOnWall = true;
            lastOnWallTime = 0f;
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

    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    #endregion
}
