using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    PlayerMovement playerMovement;
    [SerializeField] public GameObject player;

    private int respawnItemTimer = 4;
    // https://www.youtube.com/watch?v=Y7pp2gzCzUI HOW TO ACCESS DATA FROM ANOTHER SCRIPT 

    private void Awake()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
    }


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
            RespawnItem(collision);          
        }
        if (collision.gameObject.CompareTag("JumpBoost"))
        {
            playerMovement.isJumpBoost = true;
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
