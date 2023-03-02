using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Stamina")]
    public float wallGrabStamina;
    public float wallJumpStamina;
    public float wallClimbStamina;
    [Range(0f, 100f)] public float stamina;

    [Header("Gravity")]
    public float gravityScale;

    [Header("Run")]
    public float defaultMoveSpeed;
    public float speed;
    

    [Space]
    [Header("Jump")]
    public float defaultJumpPower;
    public float jumpPower;
    public float jumpCutPower;

    [Space]
    [Header("Wall Mechanics")]
    // wall slide
    public float wallSlidingSpeed;
    // Wall Jump
    public float wallJumpingDirection;
    public float wallJumpingTime;
    public float wallJumpingCounter;
    public float wallJumpingDuration;
    public Vector2 wallJumpingPower;
    // wall climb
    [Range(0.01f, 5f)] public float wallClimbingSpeedUp;
    [Range(0.01f, 5f)] public float wallClimbingSpeedDown;

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

    [Space]
    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
    [HideInInspector] public float coyoteTimeCounter;
    [Range(0.01f, 0.5f)] public float jumpBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.
    [HideInInspector] public float jumpBufferTimeCounter;

    private void OnValidate()
    {
        moveSpeedIncrease = defaultMoveSpeed * 1.5f;

        jumpBoostIncrease = defaultJumpPower * 1.3f;
    }

}
