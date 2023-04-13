using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{

    // [SerializeField] private PlayerMovement player;
    [SerializeField] private TestMovement2 player;

    private bool inWater;
    public float waterGravity; 
    public float waterSpeed;
    public float waterJump;

    private void Start()
    {
        inWater = false;
        //waterGravity = player.data.gravityScale / 2;
        waterGravity = 1;
        waterSpeed = player.data.defaultMoveSpeed / 2;
        waterJump = player.data.defaultJumpPower / 2;
    }

    private void FixedUpdate()
    {
        if (inWater) // decrease stamina while in water
        {
            player.data.stamina -= player.data.waterStaminaDrain * Time.deltaTime;
            
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
            player.data.jumpPower = waterJump;
            player.rb.gravityScale = waterGravity;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inWater = false;
            player.SetGravityScale(player.data.gravityScale);
            player.data.runMaxSpeed = player.data.defaultMoveSpeed;
            player.data.jumpPower = player.data.defaultJumpPower;
        }
    }
}
