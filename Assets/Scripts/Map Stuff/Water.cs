using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private SugboMovement player;
    private bool inWater;
    public float waterGravity; 
    public float waterSpeed;
    public float waterJump;

    private void Start()
    {
        player = FindObjectOfType<SugboMovement>();
        inWater = false;
        waterGravity = 1;
        waterSpeed = player.defaultMoveSpeed / 2;
        waterJump = player.defaultJumpPower / 2;
    }

    private void FixedUpdate()
    {
        if (inWater) // decrease stamina while in water
        {
            player.stamina -= player.waterStaminaDrain * Time.deltaTime; 
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inWater = true;
            player.runMaxSpeed = waterSpeed;
            player.jumpPower = waterJump;
            player.SetGravityScale(waterGravity);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inWater = false;
            player.SetGravityScale(player.gravityScale);
            player.runMaxSpeed = player.defaultMoveSpeed;
            player.jumpPower = player.defaultJumpPower;
        }
    }
}
