using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForeignerRandomizer : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject arm;
    [SerializeField] private GameObject otherArm;
    [SerializeField] private GameObject leg;
    [SerializeField] private GameObject otherLeg;

    private SpriteRenderer headSpriteRenderer;
    private SpriteRenderer bodySpriteRenderer;
    private SpriteRenderer armSpriteRenderer;
    private SpriteRenderer otherArmSpriteRenderer;
    private SpriteRenderer legSpriteRenderer;
    private SpriteRenderer otherLegSpriteRenderer;

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
        otherArmSpriteRenderer = otherArm.GetComponent<SpriteRenderer>();
        legSpriteRenderer = leg.GetComponent<SpriteRenderer>();
        otherLegSpriteRenderer = otherLeg.GetComponent<SpriteRenderer>();
    }
    public void SetPlayerSprites()
    {
        headSpriteRenderer.sprite = headOptions[NewDataPersistenceManager.instance.gameData.FheadIndex];
        bodySpriteRenderer.sprite = bodyOptions[NewDataPersistenceManager.instance.gameData.FbodyIndex];
        armSpriteRenderer.sprite = armOptions[NewDataPersistenceManager.instance.gameData.FarmIndex];
        otherArmSpriteRenderer.sprite = armOptions[NewDataPersistenceManager.instance.gameData.FarmIndex];
        legSpriteRenderer.sprite = legOptions[NewDataPersistenceManager.instance.gameData.FlegIndex];
        otherLegSpriteRenderer.sprite = legOptions[NewDataPersistenceManager.instance.gameData.FlegIndex];
    }

    public void LoadData(GameData data)
    {
        SetPlayerSprites();
    }

    public void SaveData(GameData data)
    {

    }
}
