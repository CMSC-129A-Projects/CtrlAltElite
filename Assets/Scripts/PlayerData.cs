using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Gravity")]
    public float gravityScale;

    [Header("Run")]
    public float speed;

    [Space]
    [Header("Jump")]
    public float jumpPower;
    public float jumpCutPower;

    [Space]
    [Header("Wall Mechanics")]
    public float wallSlidingSpeed;
    // Wall Jump
    public float wallJumpingDirection;
    public float wallJumpingTime;
    public float wallJumpingCounter;
    public float wallJumpingDuration;
    public Vector2 wallJumpingPower;

    [Space]
    [Header("Power Ups")]
    // Move Speed
    public float moveSpeedTimer;
    public float moveSpeedTimerCap;
    public float defaultMoveSpeed;
    public float moveSpeedIncrease;
    // Jump Boost
    public float jumpBoostTimer;
    public float jumpBoostTimerCap;
    public float defaultJumpPower;
    public float jumpBoostIncrease;

    [Space]
    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
    [HideInInspector] public float coyoteTimeCounter;
    [Range(0.01f, 0.5f)] public float jumpBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.
    [HideInInspector] public float jumpBufferTimeCounter;

    private void OnValidate()
    {
        defaultMoveSpeed = speed;
        moveSpeedIncrease = defaultMoveSpeed * 1.5f;

        defaultJumpPower = jumpPower;
        jumpBoostIncrease = defaultJumpPower * 1.3f;
    }

}
