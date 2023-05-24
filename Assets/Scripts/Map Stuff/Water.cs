using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{

    // [SerializeField] private PlayerMovement player;
    // [SerializeField] private TestMovement2 player;
    // [SerializeField] private Movement player;
    private SugboMovement player;
    private bool inWater;
    public float waterGravity; 
    public float waterSpeed;
    public float waterJump;

    private void Start()
    {

        player = FindObjectOfType<SugboMovement>();
        inWater = false;
        //waterGravity = player.data.gravityScale / 2;
        waterGravity = 1;
        //waterSpeed = player.defaultMovementSpeed / 2;
        //waterJump = player.defaultJumpPower / 2;
        waterSpeed = player.defaultMoveSpeed / 2;
        waterJump = player.defaultJumpPower / 2;
    }

    private void FixedUpdate()
    {
        if (inWater) // decrease stamina while in water
        {
            player.stamina -= player.waterStaminaDrain * Time.deltaTime;
            //player.stamina -= player.waterStaminaDrain * Time.deltaTime;
            
        }
        else
        {
            
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inWater = true;
            player.runMaxSpeed = waterSpeed;
            //player.movementSpeed = waterSpeed;
            //player.jumpPower = waterJump;
            player.jumpPower = waterJump;
            // player.rb.gravityScale = waterGravity;
            player.SetGravityScale(waterGravity);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inWater = false;
            //player.rb.gravityScale = player.defaultGravity;
            player.SetGravityScale(player.gravityScale);
            //player.movementSpeed = player.defaultMovementSpeed;
            //player.jumpPower = player.defaultJumpPower;
            player.runMaxSpeed = player.defaultMoveSpeed;
            player.jumpPower = player.defaultJumpPower;
        }
    }
}
