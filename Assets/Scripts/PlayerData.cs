using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
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


    private void OnValidate()
    {
        defaultMoveSpeed = speed;
        moveSpeedIncrease = defaultMoveSpeed * 1.5f;

        defaultJumpPower = jumpPower;
        jumpBoostIncrease = defaultJumpPower * 1.3f;
    }

}
