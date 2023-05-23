using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySpriteSetter : MonoBehaviour
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject arm;
    [SerializeField] private GameObject leg;

    private SpriteRenderer headSpriteRenderer;
    private SpriteRenderer bodySpriteRenderer;
    private SpriteRenderer armSpriteRenderer;
    private SpriteRenderer legSpriteRenderer;

    [Header("Sprite to Cycle Through")]
    [Header("Head")]
    public List<Sprite> headOptions = new List<Sprite>();
    [Header("Body")]
    public List<Sprite> bodyOptions = new List<Sprite>();
    [Header("Arm")]
    public List<Sprite> armOptions = new List<Sprite>();
    [Header("Leg")]
    public List<Sprite> legOptions = new List<Sprite>();

    private void Awake()
    {
        headSpriteRenderer = head.GetComponent<SpriteRenderer>();
        bodySpriteRenderer = body.GetComponent<SpriteRenderer>();
        armSpriteRenderer = arm.GetComponent<SpriteRenderer>();
        legSpriteRenderer = leg.GetComponent<SpriteRenderer>();
    }
    public void SetPlayerSprites()
    {
        Debug.Log("BSS SETTING");
        Debug.Log(NewDataPersistenceManager.instance.gameData.respawnPoint);
        /*Debug.Log(NewDataPersistenceManager.instance.gameData.headIndex);
        Debug.Log(NewDataPersistenceManager.instance.gameData.bodyIndex);
        Debug.Log(NewDataPersistenceManager.instance.gameData.armIndex);
        Debug.Log(NewDataPersistenceManager.instance.gameData.legIndex);*/
        headSpriteRenderer.sprite = headOptions[NewDataPersistenceManager.instance.gameData.headIndex];
        bodySpriteRenderer.sprite = bodyOptions[NewDataPersistenceManager.instance.gameData.bodyIndex];
        armSpriteRenderer.sprite = armOptions[NewDataPersistenceManager.instance.gameData.armIndex];
        legSpriteRenderer.sprite = legOptions[NewDataPersistenceManager.instance.gameData.legIndex];
    }
}
