using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemCollector : MonoBehaviour
{
    private SugboMovement playerMovement;
    [SerializeField] private GameObject achievementText;

    private void Awake()
    {
        playerMovement = GetComponent<SugboMovement>();
    }

    private int respawnItemTimer = 4;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("JumpBoost"))
        {
            playerMovement.ActivatePowerUpBuff(0);
            playerMovement.isJumpBoost = true;
            RespawnItem(collision);
        }
        if (collision.gameObject.CompareTag("MoveSpeed"))
        {
            playerMovement.ActivatePowerUpBuff(1);
            playerMovement.isMoveSpeed = true;
            RespawnItem(collision);   
        }
        if(collision.gameObject.CompareTag("DoubleJump"))
        {
            playerMovement.ActivatePowerUpBuff(2);
            playerMovement.canDoubleJump = true;
            playerMovement.doubleJumpPressed = false;
            RespawnItem(collision);          
        } 
        if (collision.gameObject.CompareTag("Dash"))
        {
            playerMovement.ActivatePowerUpBuff(3);
            playerMovement.canDash = true;
            playerMovement.dashPressed = false;
            RespawnItem(collision);
        }

        if (collision.gameObject.CompareTag("MedalPiece"))
        {
            AudioManager.instance.PlayMedal();
            collision.gameObject.SetActive(false);
            NewDataPersistenceManager.instance.gameData.medalsCollected += 1;
            achievementText.SetActive(true);
            string text = $"Medal {NewDataPersistenceManager.instance.gameData.medalsCollected}/" +
              $"{NewDataPersistenceManager.instance.gameData.totalMedals} collected.";
            achievementText.GetComponent<TextMeshProUGUI>().text = text;
            
            StartCoroutine(DisableAchievementText());
        }

        if (collision.gameObject.CompareTag("FinalTreasure"))
        {
            AudioManager.instance.PlayFinal();
            collision.gameObject.SetActive(false);
            NewDataPersistenceManager.instance.gameData.medalsCollected += 1;
            achievementText.SetActive(true);
            string text = $"Medal {NewDataPersistenceManager.instance.gameData.medalsCollected}/" +
              $"{NewDataPersistenceManager.instance.gameData.totalMedals} collected.";
            achievementText.GetComponent<TextMeshProUGUI>().text = text;

            StartCoroutine(DisableAchievementText());
        }

        if (collision.gameObject.CompareTag("StartGame"))
        {
            AudioManager.instance.PlayMedal();
            CharacterCreationMenu ccMenu = FindObjectOfType<CharacterCreationMenu>(true);
            ccMenu.StartGame();
        }
    }

    IEnumerator SwitchToNextScene(float time)
    {
        Debug.Log("Switching to next city...");
        yield return new WaitForSeconds(time);
        int currentSceneIndex = NewDataPersistenceManager.instance.gameData.sceneIndex;
        NewDataPersistenceManager.instance.gameData.previousSceneIndex = currentSceneIndex;
        NewDataPersistenceManager.instance.IncrementSceneIndex();

        SceneManager.LoadSceneAsync(currentSceneIndex + 1);
        // TransitionManager.instance.NextScene(currentSceneIndex + 1);
    }

    IEnumerator DisableAchievementText()
    {
        Debug.Log("DisableAchievement");
        yield return new WaitForSeconds(3);
        achievementText.SetActive(false);
        StartCoroutine(SwitchToNextScene(0));
    }

    #region RESPAWN ITEM
    public void RespawnItem(Collider2D collision)
    {
        AudioManager.instance.PlayPickup();
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
