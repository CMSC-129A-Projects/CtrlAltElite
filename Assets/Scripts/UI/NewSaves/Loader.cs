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
        /*Debug.Log("newGame " + NewDataPersistenceManager.instance.gameData.newGame);
        *//*NewDataPersistenceManager.instance.LoadGame();
        NewDataPersistenceManager.instance.SaveGame();
        NewDataPersistenceManager.instance.LoadGame();*//*
        if (NewDataPersistenceManager.instance.gameData.newGame)
        {
            Debug.Log("True newGame");
            *//*NewDataPersistenceManager.instance.SaveGame();
            NewDataPersistenceManager.instance.LoadGame();*//*
            SugboMovement.SavePlayer();
            SugboMovement.LoadPlayer();
            NewDataPersistenceManager.instance.gameData.newGame = false;
        }
        else
        {
            Debug.Log("False newGame");
            // NewDataPersistenceManager.instance.LoadGame();
            SugboMovement.LoadPlayer();
        }
        Debug.Log("ASDASD");
        Debug.Log(NewDataPersistenceManager.instance.gameData.position + 
            " " + NewDataPersistenceManager.instance.gameData.newGame);*/


        // NewDataPersistenceManager.instance.LoadGame();
        // NewDataPersistenceManager.instance.SaveGame();
        // NewDataPersistenceManager.instance.LoadGame();
        // Debug.Log(NewDataPersistenceManager.instance.gameData.position);
        /*Debug.Log(NewDataPersistenceManager.instance.gameData.headIndex);
        Debug.Log(NewDataPersistenceManager.instance.gameData.bodyIndex);
        Debug.Log(NewDataPersistenceManager.instance.gameData.armIndex);
        Debug.Log(NewDataPersistenceManager.instance.gameData.legIndex);*/

        // bodySpriteSetter.SetPlayerSprites();
        // call SetSprites() here
    }
}
