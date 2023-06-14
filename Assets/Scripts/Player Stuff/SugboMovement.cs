using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SugboMovement : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Slider staminaWheel;
    [SerializeField] private Slider usageWheel;
    [SerializeField] private Slider jumpBoostBuff;
    [SerializeField] private Slider speedBoostBuff;
    [SerializeField] private Slider doubleJumpBuff;
    [SerializeField] private Slider dashBuff;
    [SerializeField] private ParticleManager particleManager;

    [SerializeField] private List<Slider> sliders;

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
    public bool inTransition;
    private bool changingDirection;
    private enum MovementState { idling, running, jumping, doubleJumping, falling, swimming, grabbing, climbing, dying }

    [Space]
    [Header("Collision")]
    public float lastOnGroundTime;
    public bool isGrounded;
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
    public float hangTimeCounter;
    [Range(0f, 1)] public float jumpHangGravityMult;
    public float airLinearDrag;

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
        DeactivateAllSliders();

        SetGravityScale(gravityScale);
        GameObject baseRespawn = GameObject.FindGameObjectWithTag("BaseRespawn");
        if (NewDataPersistenceManager.instance.gameData.newGame)
        {
            if (baseRespawn != null)
            {
                NewDataPersistenceManager.instance.gameData.respawnPoint = baseRespawn.transform.position;
                NewDataPersistenceManager.instance.gameData.position = baseRespawn.transform.position;
            }
        }
        else
        {
            NewDataPersistenceManager.instance.LoadGame();
        }
       
        transform.position = NewDataPersistenceManager.instance.gameData.position;
    }

    private void Update()
    {
        
        
        if (inTransition) return;
        
        if (isDead)
        {
            ResetPowerUp();
            return;
        }
        if (!canMove) return;
        UpdateAnimation();
        #region TIMERS
        lastOnWallTime += Time.deltaTime;
        lastOnGroundTime += Time.deltaTime;
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
        WaterGravity();

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
                        NeutralWallJump(baseJump: true);
                    }
                    else
                    {
                        WallJump(baseJump: true);
                    }
                }
            }
            else
            {
                if (!isGrounded)
                {
                    return;
                }
                else if (inWater)
                {
                    GroundWaterJump(baseJump: true);
                }
                else
                {
                    if (isGrounded || canDoubleJump || coyoteTimeCounter > 0f)
                        Jump(Vector2.up, baseJump: true);
                }
            }
        }

        if (!isGrounded && isOnWall && !isWallJumping)
        {
            if (CanWallJump())
            {
                if ((moveInput.x > 0 && isFacingRight && onRightWall) || (moveInput.x < 0 && !isFacingRight && onLeftWall))
                {
                    NeutralWallJump(baseJump: true);
                }
                else
                {
                    WallJump(baseJump: true);
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
                    stamina -= waterStaminaDrain;
                    // Jump(Vector2.up, baseJump: true);
                    WaterJump(baseJump: true);
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

                if (isOnWall && !isGrounded && !isDead)
                {
                    StickToWall();
                }
            }
        }

        UpdateStamina();
        UpdatePowerUpBuffSlider();
        
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

        if (inWater && stamina <= staminaMin && !isDead)
        {
            death.HandleDeath();
        }

        staminaWheel.value = (stamina / staminaMax);
        usageWheel.value = (stamina / staminaMax) + 0.1f;
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (isDead) return;

        if (isGrounded && CanWallGrab() && lastOnWallTime < 0.01f)
        {
            state = MovementState.grabbing;
        }
        else if (CanWallClimb() && lastOnWallTime < 0.01f)
        {
            state = MovementState.climbing;
        }
        else if (!isGrounded && CanWallGrab() && lastOnWallTime < 0.01f)
        {
            state = MovementState.grabbing;
        }
        else if (!isGrounded && CanWallSlide() && lastOnWallTime < 0.01f)
        {
            state = MovementState.grabbing;
        }
        
        // water running/swimming
        else if (moveInput.x > 0f && inWater && isGrounded)
        {
            state = MovementState.running;
        }
        else if (moveInput.x < 0f && inWater && isGrounded)
        {
            state = MovementState.running;
        }
        else if (moveInput.x > 0f && inWater && !isGrounded)
        {
            state = MovementState.swimming;
        }
        else if (moveInput.x < 0f && inWater && !isGrounded)
        {
            state = MovementState.swimming;
        }
        // non water running
        else if (moveInput.x > 0f && !inWater && isGrounded && !isOnWall)
        {
            state = MovementState.running;
        }
        else if (moveInput.x < 0f && !inWater && isGrounded && !isOnWall)
        {
            state = MovementState.running;
        }
        else
        {
            state = MovementState.idling;
        }


        if (rb.velocity.y > 0.1f && doubleJumpPressed && !inWater)
        {
            state = MovementState.doubleJumping;
        }
        else if (rb.velocity.y > 0.1f && !doubleJumpPressed && !inWater && lastOnWallTime > 0.1f)
        {
            state = MovementState.jumping;
        }
        // jumping while on wall
        else if (rb.velocity.y > 0.1f && !doubleJumpPressed && !inWater && lastOnWallTime < 0.1f && !CanWallClimb())
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < 0.1f && !inWater && !isGrounded && lastOnWallTime > 0.1f)
        {
            state = MovementState.falling;
        }
        else if (rb.velocity.y < 0.1f && inWater && !isGrounded)
        {
            state = MovementState.swimming;
        }
        else if (rb.velocity.y > 0.1f && inWater && !isGrounded)
        {
            state = MovementState.swimming;
        }

        animator.SetInteger("movementState", (int)state);
    }

    

    // Data Persistence
    #region SAVE STUFF
    public void LoadData(GameData data)
    {
        transform.position = data.position;
        if (bodySpriteSetter != null) bodySpriteSetter.SetPlayerSprites();
    }

    public void SaveData(GameData data)
    {
        if (PlayerDeath.currentRespawn != null)
        {
            data.respawnPoint = PlayerDeath.currentRespawn.transform.position;
            data.position = data.respawnPoint;
            data.sceneIndex = SceneManager.GetActiveScene().buildIndex;

            // HARD CODED CHANGE THIS IF NECESSARY
            // 5 FOR CITY 1, 6 FOR CITY 2, 7 FOR CITY 3, etc.
            if (data.sceneIndex >= 4 && data.sceneIndex <= 8)
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

        if (collision.gameObject.CompareTag("TempSave"))
        {
            Debug.Log("New TempSave");
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
        // rb.velocity = new Vector2(transform.localScale.x * 20f, rb.velocity.y);
        float direction = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * 20f, rb.velocity.y);
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

    private void NeutralWallJump(bool baseJump)
    {
        AudioManager.instance.PlayJump(baseJump);
        particleManager.PlayJumpParticle();
        stamina -= wallJumpStaminaDrain;
        ApplyAirLinearDrag();
        rb.velocity = new Vector2(0f, wallJumpingPower.y + 2f);
        hangTimeCounter = 0f;
        jumpBufferTimeCounter = 0f;
        isJumping = true;
    }

    private void WallJump(bool baseJump)
    {
        AudioManager.instance.PlayJump(baseJump);
        particleManager.PlayJumpParticle();
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
    }
    private void StickToWall()
    {
        //Push player torwards wall
        float direction = isFacingRight ? 1f : -1f;
        if (onRightWall && direction >= 0f)
        {
            rb.velocity = new Vector2(5f, rb.velocity.y);
        }
        else if (onLeftWall && direction <= 0f)
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
    private void ResetPowerUp()
    {
        Debug.Log("ResetPowerUp()");
        isJumpBoost = false;
        jumpBoostTimer = 0;
        isMoveSpeed = false;
        moveSpeedTimer = 0;
        canDoubleJump = false;
        canDash = false;
        jumpPower = defaultJumpPower;
        runMaxSpeed = defaultMoveSpeed;
        jumpBoostBuff.gameObject.SetActive(false);
        speedBoostBuff.gameObject.SetActive(false);
        doubleJumpBuff.gameObject.SetActive(false);
        dashBuff.gameObject.SetActive(false);
    }
    public void ActivatePowerUpBuff(int powerUp)
    {
        switch (powerUp)
        {
            case 0:
                jumpBoostBuff.gameObject.SetActive(true);
                break;
            case 1:
                speedBoostBuff.gameObject.SetActive(true);
                break;
            case 2:
                doubleJumpBuff.gameObject.SetActive(true);
                break;
            case 3:
                dashBuff.gameObject.SetActive(true);
                break;
        }
    }

    private void DeactivatePowerUpBuff(int powerUp)
    {
        switch (powerUp)
        {
            case 0:
                jumpBoostBuff.gameObject.SetActive(false);
                break;
            case 1:
                speedBoostBuff.gameObject.SetActive(false);
                break;
            case 2:
                doubleJumpBuff.gameObject.SetActive(false);
                break;
            case 3:
                dashBuff.gameObject.SetActive(false);
                break;
        }
    }
    private void UpdatePowerUpBuffSlider()
    {
        
        if (isJumpBoost)
        {
            float sliderValue = 1f - (jumpBoostTimer / jumpBoostTimerCap);
            sliderValue = Mathf.Clamp01(sliderValue); // Ensure the slider value is within 0-1 range
            jumpBoostBuff.value = sliderValue;
        }

        if (isMoveSpeed)
        {
            float sliderValue = 1f - (moveSpeedTimer / moveSpeedTimerCap);
            sliderValue = Mathf.Clamp01(sliderValue); // Ensure the slider value is within 0-1 range
            speedBoostBuff.value = sliderValue;
        }
    }
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
                DeactivatePowerUpBuff(1);
            }
        }
        #endregion

        #region DOUBLE JUMP
        if (canDoubleJump)
        {
            if (jumpBufferTimeCounter > 0f) // replace with jump buffer
            {
                Jump(Vector2.up, baseJump: false);
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
        else
        {
            DeactivatePowerUpBuff(2);
        }
        if (doubleJumpPressed && !isGrounded)
        {
            canDoubleJump = false;
            if (isGrounded)
            {
                doubleJumpPressed = false;
            }
            if (isOnWall)
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
                DeactivatePowerUpBuff(0);
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
        else
        {
            DeactivatePowerUpBuff(3);
        }
        if (dashPressed && !isGrounded)
        {
            canDash = false;
            if (isGrounded)
            {
                dashPressed = false;
            }
            if (isOnWall)
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
        AudioManager.instance.PlayDash();
        particleManager.PlayDashPartcile();
        dashPressed = true;
        canDash = false;
        isDashing = true;
        float direction = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * dashPower, 0f);
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
        if (inWater) return;

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

    private void WaterGravity()
    {
        if (inWater)
        {
            if (isGrounded || rb.velocity.y > 0f)
            {
                SetGravityScale(jumpGravityScale * 2);
            }
            else
            {
                SetGravityScale(waterGravityScale);
            }
            
        }
    }

    #endregion

    #region JUMP METHODS
    private void Jump(Vector2 direction, bool baseJump)
    {
        AudioManager.instance.PlayJump(baseJump);
        if (!inWater) particleManager.PlayJumpParticle();
        ApplyAirLinearDrag();
        // rb.velocity = new Vector2(rb.velocity.x, 0f);
        // rb.AddForce(direction * jumpPower, ForceMode2D.Impulse);
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        hangTimeCounter = 0f;
        jumpBufferTimeCounter = 0f;
        maxFallTimer = 0f;
        isJumping = true;
    }

    private void GroundWaterJump(bool baseJump)
    {
        AudioManager.instance.PlayJump(baseJump);
        ApplyAirLinearDrag();
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        hangTimeCounter = 0f;
        jumpBufferTimeCounter = 0f;
        maxFallTimer = 0f;
        isJumping = true;
    }
    private void WaterJump(bool baseJump)
    {
        AudioManager.instance.PlayJump(baseJump);
        ApplyAirLinearDrag();
        rb.velocity = new Vector2(rb.velocity.x, jumpPower + 2);
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
        return isOnWall && (stamina != staminaMin) && !isGrounded && 
            !Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) 
            && rb.velocity.y < 0f && ((moveInput.x > 0 && isFacingRight) || (moveInput.x < 0 && !isFacingRight));
    }

    private bool CanWallGrab()
    {
        return isOnWall && (stamina != staminaMin) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    private bool CanWallClimb()
    {
        return isOnWall && (stamina != staminaMin) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && moveInput.y != 0;
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
            // SetGravityScale(gravityScale);
        }
    }

    private void GroundCollisionCheck()
    {
        if (coll.onGround)
        {
            isGrounded = true;
            if (lastOnGroundTime > 0.1f)
            {
                AudioManager.instance.PlayFall();
                particleManager.PlayFallParticle();
            }
            lastOnGroundTime = 0f;
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
            if (lastOnGroundTime > 0.1f)
            {
                AudioManager.instance.PlayFall();
                particleManager.PlayFallParticle();
            }
            lastOnGroundTime = 0f;
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
    private void DeactivateAllSliders()
    {
        foreach (Slider slider in sliders)
        {
            slider.gameObject.SetActive(false);
        }
    }
    public void SetStaminaToMax()
    {
        stamina = staminaMax;
    }
    public void SetSpeedToZero()
    {
        animator.SetTrigger("Dying");
        runMaxSpeed = 0f;
        rb.velocity = new Vector2(0f, 0f);
        moveInput = new Vector2(0f, 0f);
    }

    public void SetRbToDynamic()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    public void SetRbToStatic()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void SetSpeedBackToDefault()
    {
        runMaxSpeed = defaultMoveSpeed;
        rb.velocity = new Vector2(0f, 0f);
        moveInput = new Vector2(0f, 0f);
    }

    public void SetAnimationToDefault()
    {
        Debug.Log("SetAnimationToDefault");
        animator.SetTrigger("Respawning");
        isDead = false;
    }
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        /*Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;*/
        
        transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
        // Debug.Log(Mathf.Abs(rb.velocity.x));
        if (isGrounded && Mathf.Abs(rb.velocity.x) > 4 && !inWater)
        {
            
            particleManager.PlayFlipParticle(isFacingRight);
        }
    }

    #endregion
}
