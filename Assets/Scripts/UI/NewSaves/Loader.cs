using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] private GameObject transition;
    [SerializeField] private SugboMovement player;
    private void Awake()
    {
        Debug.Log("Loader Awake");
        
        transition.SetActive(true);

    }

    private void Start()
    {
        if (player != null) 
        {
            DontAllowMovePlayer();
            StartCoroutine(AllowMovePlayer());
        }
        
    }

    private void DontAllowMovePlayer()
    {
        Debug.Log("LOADER DONT ALLOW MOVE");
        player.inTransition = true;
    }

    private IEnumerator AllowMovePlayer()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("LOADER ALLOW MOVE PLAYER");
        player.inTransition = false;
    }
}
