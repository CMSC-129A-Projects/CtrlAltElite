using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private BodySpriteSetter bodySpriteSetter;
    private void Awake()
    {
        bodySpriteSetter = new BodySpriteSetter();
        Debug.Log("Loader Awake");
        NewDataPersistenceManager.instance.SaveGame();
        // NewDataPersistenceManager.instance.LoadGame();
        /*Debug.Log(NewDataPersistenceManager.instance.gameData.headIndex);
        Debug.Log(NewDataPersistenceManager.instance.gameData.bodyIndex);
        Debug.Log(NewDataPersistenceManager.instance.gameData.armIndex);
        Debug.Log(NewDataPersistenceManager.instance.gameData.legIndex);*/

        bodySpriteSetter.SetPlayerSprites();
        // call SetSprites() here
    }
}
