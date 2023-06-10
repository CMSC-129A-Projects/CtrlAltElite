using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityTransition : MonoBehaviour
{
    [SerializeField] private int cityIndexFromBuild;

    private void Start()
    {
        // DeactivateTransition();
    }
    public void DeactivateTransition() // called in transitionExit animation frame
    {
        Debug.Log("Deactivating City Transition");
        this.gameObject.SetActive(false);
    }

    public void ActivateTransition() // called in transitionStart animation frame
    {
        Debug.Log("Activating City Transition");
        this.gameObject.SetActive(true);
    }

    public void AllowMovePlayer() // called in transitionExit animation frame
    {
        Debug.Log("AllowMovePlayer");
        SugboMovement.isDead = false;
        SugboMovement.canMove = true;
    }

    public void DontAllowMovePlayer() // called in transitionExit animation frame
    {
        Debug.Log("DontAllowMovePlayer");
        SugboMovement.canMove = false;
    }

    public void TransitionManagerSwitchScene()
    {
        Debug.Log("Here");
        DontAllowMovePlayer();
        StartCoroutine(TransitionManager.instance.SwitchScene(this.cityIndexFromBuild));
    }
}
