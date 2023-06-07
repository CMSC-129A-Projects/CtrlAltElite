using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTransition : MonoBehaviour
{

    private void Start()
    {
        DeactivateDeathTransition();
    }
    public void DeactivateDeathTransition()
    {
        Debug.Log("Deactivating Death Transition");
        this.gameObject.SetActive(false);
    }

    public void ActivateDeathTransition()
    {
        Debug.Log("Activating Death Transition");
        this.gameObject.SetActive(true);
    }

    public void AllowMovePlayer()
    {
        Debug.Log("AllowMovePlayer");
        SugboMovement.isDead = false;
        SugboMovement.canMove = true;
    }
}
