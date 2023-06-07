using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTransition : MonoBehaviour
{

    private void Start()
    {
        DeactivateDeathTransition();
    }
    public void DeactivateDeathTransition() // called in transitionExit animation frame
    {
        Debug.Log("Deactivating Death Transition");
        this.gameObject.SetActive(false);
    }

    public void ActivateDeathTransition() // called in transitionStart animation frame
    {
        Debug.Log("Activating Death Transition");
        this.gameObject.SetActive(true);
    }

    public void AllowMovePlayer() // called in transitionExit animation frame
    {
        Debug.Log("AllowMovePlayer");
        SugboMovement.isDead = false;
        SugboMovement.canMove = true;
    }
}
