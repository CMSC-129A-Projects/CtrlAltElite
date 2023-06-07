using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public string currentBoundary;
    public static GameObject currentRespawn;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            currentBoundary = collision.gameObject.name;
            if (collision.gameObject.transform.childCount > 0)
            {
                Debug.Log("Current Boundary");
                currentRespawn = collision.transform.GetChild(0).gameObject;
                NewDataPersistenceManager.instance.SaveGame();
            }
        }

        if (collision.gameObject.CompareTag("TempSave"))
        {
            currentBoundary = collision.gameObject.name;
            if (collision.gameObject.transform.childCount > 0)
            {
                // Debug.Log("Current TempSave");
                StartCoroutine(RefreshCurrentRespawn(collision));
                
            }
        }



        if (collision.gameObject.CompareTag("Spikes"))
        {
            HandleDeath();
        }
    }

    private IEnumerator RefreshCurrentRespawn(Collider2D collision)
    {
        yield return null;
        Debug.Log("Refreshed");
        currentRespawn = collision.transform.GetChild(0).gameObject;
        NewDataPersistenceManager.instance.SaveGame();
    }
    public void HandleDeath()
    {
        TransitionManager.instance.PlayDeathTransition();
        SugboMovement.isDead = true;
        SugboMovement.canMove = false;
        transform.GetComponent<CapsuleCollider2D>().enabled = false;
        animator.SetBool("Running", false);
        animator.SetBool("Idling", false);
        animator.SetBool("Climbing", false);
        animator.SetBool("Grabbing", false);
        animator.SetBool("Falling", false);
        animator.SetBool("Jumping", false);
        animator.SetBool("DoubleJumping", false);
        animator.SetBool("Death", true);
        animator.SetTrigger("Dying");
    }

    public void HandleRespawn() // called in FixedDeath animation frame
    {
        Debug.Log("Respawning");
        SugboMovement player = GetComponent<SugboMovement>();
        player.SetStaminaToMax();
        TransitionManager.instance.PlayRespawnTransition();
        animator.ResetTrigger("Dying");
        animator.SetTrigger("Respawning");
        animator.SetBool("Death", false);
        animator.SetBool("Idling", true);
        transform.GetComponent<CapsuleCollider2D>().enabled = true;
        NewDataPersistenceManager.instance.LoadGame(); 
    }
}
