using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelineStuff : MonoBehaviour
{
    [SerializeField] private Animator basePlayerAnimator;
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;
    private bool pressedEsc = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pressedEsc)
            {
                // If the confirmation menu is active, close it
                confirmationPopupMenu.gameObject.SetActive(false);
                pressedEsc = false;
            }
            else
            {
                confirmationPopupMenu.ActivateMenu(
                    "Skip cutscene?",
                    // function to execute if we select 'yes'
                    () =>
                    {
                        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
                    },
                    // function to execute if we select 'no'
                    () =>
                    {
                    // do nothing
                    }
                );
                pressedEsc = true;
            }
            
        }
    }
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
        SceneManager.LoadSceneAsync("F City 4");
    }

    /*public void OnYesClicked()
    {
        confirmationPopupMenu.ActivateMenu(
                "Skip cutscene?",
                // function to execute if we select 'yes'
                () =>
                {
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
                },
                // function to execute if we select 'cancel'
                () =>
                {
                    
                }
            );
    }

    public void OnNoClicked()
    {
        this.gameObject.SetActive(false);
    }*/
}
