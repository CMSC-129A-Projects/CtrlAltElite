using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public string currentBoundary;
    public static GameObject currentRespawn;
    private Animator animator;
    private BodySpriteSetter bodySpriteSetter;

    private void Start()
    {
        animator = GetComponent<Animator>();
        bodySpriteSetter = GetComponent<BodySpriteSetter>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            currentBoundary = collision.gameObject.name;
            if (collision.gameObject.transform.childCount > 0)
            {
                currentRespawn = collision.transform.GetChild(0).gameObject;
                NewDataPersistenceManager.instance.SaveGame();
            }
        }

        if (collision.gameObject.CompareTag("Spikes"))
        {
            // StartCoroutine(StartRespawn());
            HandleDeath();
        }
    }

    public void HandleDeath()
    {
        TransitionManager.instance.PlayDeathTransition();
        // bodySpriteSetter.DisablePlayerSprites();
        SugboMovement.isDead = true;
        SugboMovement.canMove = false;
        transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
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

    public void HandleRespawn()
    {
        Debug.Log("Respawning");
        TransitionManager.instance.PlayRespawnTransition();
        // bodySpriteSetter.EnablePlayerSprites();
        animator.ResetTrigger("Dying");
        animator.SetBool("Death", false);
        animator.SetBool("Idling", true);
        transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        NewDataPersistenceManager.instance.LoadGame(); 
    }

    public void AllowMovePlayer()
    {
        SugboMovement.isDead = false;
        SugboMovement.canMove = true;
    }
}
