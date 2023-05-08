using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    // [SerializeField] public PlayerMovement playerMovement;
    // [SerializeField] public TestMovement2 playerMovement;
    private SugboMovement playerMovement;

    private void Awake()
    {
        playerMovement = FindObjectOfType<SugboMovement>();
    }

    private int respawnItemTimer = 4;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MoveSpeed"))
        {
            playerMovement.isMoveSpeed = true;
            RespawnItem(collision);   
        }
        if(collision.gameObject.CompareTag("DoubleJump"))
        {
            playerMovement.canDoubleJump = true;
            playerMovement.doubleJumpPressed = false;
            RespawnItem(collision);          
        }
        if (collision.gameObject.CompareTag("JumpBoost"))
        {
            playerMovement.isJumpBoost = true;
            RespawnItem(collision);
        }
        if (collision.gameObject.CompareTag("Dash"))
        {
            playerMovement.canDash = true;
            playerMovement.dashPressed = false;
            RespawnItem(collision);
        }
    }


    #region RESPAWN ITEM
    public void RespawnItem(Collider2D collision)
    {
        collision.gameObject.SetActive(false); //deactivates fruit
        StartCoroutine(Respawn(collision, respawnItemTimer));
    }

    IEnumerator Respawn(Collider2D collision, int time)
    {
        yield return new WaitForSeconds(time);
        collision.gameObject.SetActive(true);
    }
    #endregion
}
