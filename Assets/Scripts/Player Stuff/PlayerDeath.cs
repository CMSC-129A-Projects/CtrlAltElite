using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public string currentBoundary;
    public static GameObject currentRespawn;
    private Animator animator;
    private SugboMovement player;
    private enum MovementState { idling, running, jumping, doubleJumping, falling, swimming, grabbing, climbing, dying }

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<SugboMovement>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        /*if (collision.gameObject.CompareTag("Boundary"))
        {
            currentBoundary = collision.gameObject.name;
            if (collision.gameObject.transform.childCount > 0)
            {
                currentRespawn = collision.transform.GetChild(0).gameObject;
                NewDataPersistenceManager.instance.SaveGame();
            }
        }

        if (collision.gameObject.CompareTag("TempSave"))
        {
            currentBoundary = collision.gameObject.name;
            if (collision.gameObject.transform.childCount > 0)
            {
                StartCoroutine(RefreshCurrentRespawn(collision));
            }
        }*/
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            currentBoundary = collision.gameObject.name;
            if (collision.gameObject.transform.childCount > 0)
            {
                Debug.Log($"Saves {collision.gameObject.name}");
                currentRespawn = collision.transform.GetChild(0).gameObject;
                NewDataPersistenceManager.instance.SaveGame();
            }
        }

        if (collision.gameObject.CompareTag("TempSave"))
        {
            currentBoundary = collision.gameObject.name;
            if (collision.gameObject.transform.childCount > 0)
            {
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
        Debug.Log("HandleDeath");
        AudioManager.instance.PlayDeath();
        TransitionManager.instance.PlayDeathTransition();
        SugboMovement.isDead = true;
        SugboMovement.canMove = false;
        transform.GetComponent<CapsuleCollider2D>().enabled = false;
        player.SetSpeedToZero();
        
        /*MovementState state;
        state = MovementState.dying;
        animator.SetInteger("movementState", (int)state);*/
        /*animator.SetBool("Running", false);
        animator.SetBool("Idling", false);
        animator.SetBool("Swimming", false);
        animator.SetBool("Climbing", false);
        animator.SetBool("Grabbing", false);
        animator.SetBool("Falling", false);
        animator.SetBool("Jumping", false);
        animator.SetBool("DoubleJumping", false);
        animator.SetBool("Death", true);
        animator.SetTrigger("Dying");*/
    }

    public void HandleRespawn() // called in FixedDeath animation frame
    {
        Debug.Log("Respawning");  
        player.SetStaminaToMax();
        player.SetSpeedBackToDefault();
        player.SetAnimationToDefault();
        TransitionManager.instance.PlayRespawnTransition();

        /*MovementState state;
        state = MovementState.idling;
        animator.SetInteger("movementState", (int)state);*/
        /*animator.ResetTrigger("Dying");
        animator.SetTrigger("Respawning");
        animator.SetBool("Running", false);
        animator.SetBool("Idling", true);
        animator.SetBool("Swimming", false);
        animator.SetBool("Climbing", false);
        animator.SetBool("Grabbing", false);
        animator.SetBool("Falling", false);
        animator.SetBool("Jumping", false);
        animator.SetBool("DoubleJumping", false);
        animator.SetBool("Death", false); */
        NewDataPersistenceManager.instance.LoadGame();
        transform.GetComponent<CapsuleCollider2D>().enabled = true;
        
    }
}
