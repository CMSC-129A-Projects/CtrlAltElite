using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelineStuff : MonoBehaviour
{
    [SerializeField] private Animator basePlayerAnimator;

    public void PlayBaseIdleAnim()
    {
        Debug.Log("PlayingIdle");
        basePlayerAnimator.SetInteger("movementState", 0);
    }

    public void PlayBaseRunAnim()
    {
        basePlayerAnimator.SetInteger("movementState", 1);
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadSceneAsync("TestMenuSave");
    }

    public void GoToCity1()
    {
        // SceneManager.LoadSceneAsync("City 1");
        // SceneManager.LoadSceneAsync("City 5");
        SceneManager.LoadSceneAsync("F City 1");
    }
}
