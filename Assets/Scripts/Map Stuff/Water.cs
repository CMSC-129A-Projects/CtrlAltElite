using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{

    // [SerializeField] private PlayerMovement player;
    // [SerializeField] private TestMovement2 player;
    [SerializeField] private Movement player;

    private bool inWater;
    public float waterGravity; 
    public float waterSpeed;
    public float waterJump;

    private void Start()
    {
        inWater = false;
        //waterGravity = player.data.gravityScale / 2;
        waterGravity = 1;
        //waterSpeed = player.defaultMovementSpeed / 2;
        //waterJump = player.defaultJumpPower / 2;
        waterSpeed = player.data.defaultMoveSpeed / 2;
        waterJump = player.data.defaultJumpPower / 2;
    }

    private void FixedUpdate()
    {
        if (inWater) // decrease stamina while in water
        {
            player.data.stamina -= player.data.waterStaminaDrain * Time.deltaTime;
            //player.stamina -= player.waterStaminaDrain * Time.deltaTime;
            
        }
        else
        {
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inWater = true;
            player.data.runMaxSpeed = waterSpeed;
            //player.movementSpeed = waterSpeed;
            //player.jumpPower = waterJump;
            player.data.jumpPower = waterJump;
            player.rb.gravityScale = waterGravity;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inWater = false;
            //player.rb.gravityScale = player.defaultGravity;
            player.SetGravityScale(player.data.gravityScale);
            //player.movementSpeed = player.defaultMovementSpeed;
            //player.jumpPower = player.defaultJumpPower;
            player.data.runMaxSpeed = player.data.defaultMoveSpeed;
            player.data.jumpPower = player.data.defaultJumpPower;
        }
    }
}
