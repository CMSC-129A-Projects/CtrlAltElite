using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public long lastUpdated;
    public float defaultMoveSpeed;
    public float defaultJumpPower;
    public float staminaMax;
    public Vector2 respawnPoint;
    public Vector2 position;
    public string name;
    public int headIndex;
    public int bodyIndex;
    public int armIndex;
    public int legIndex;

    public string Fname;
    public int FheadIndex;
    public int FbodyIndex;
    public int FarmIndex;
    public int FlegIndex;

    public int sceneIndex;
    
    public float volumePreference;
    public bool newGame;
    public int medalsCollected;
    public int totalMedals;
    public GameData()
    {
        
        this.respawnPoint = Vector2.zero;
        this.position = Vector2.zero;
        this.name = "";
        this.headIndex = 0;
        this.bodyIndex = 0;
        this.armIndex = 0;
        this.legIndex = 0;

        this.Fname = "";
        this.FheadIndex = 0;
        this.FbodyIndex = 0;
        this.FarmIndex = 0;
        this.FlegIndex = 0;

        this.sceneIndex = 0;
        
        this.volumePreference = 1f;
        this.newGame = true;
        this.medalsCollected = 0;
        this.totalMedals = 5;


        // this.respawnPoint = new Vector2(-115.8605f, -9.98346f);

        // starting position of JaniTest DO NOT CHANGE THIS 
        this.position = new Vector2(-87, -15);

        // starting position of SaveTest DO NOT CHANGE THIS 
        // this.position = new Vector2(-115.8605f, -9.98346f);

        // test
        /*GameObject baseRespawn = GameObject.FindGameObjectWithTag("BaseRespawn");
        if (baseRespawn != null)
        {
            this.respawnPoint = baseRespawn.transform.position;
            this.position = this.respawnPoint;
        }*/
    }

    /*public int GetPercentageComplete()
    {
        // figure out how many coins we've collected
        int totalCollected = 0;
        foreach (bool collected in coinsCollected.Values)
        {
            if (collected)
            {
                totalCollected++;
            }
        }

        // ensure we don't divide by 0 when calculating the percentage
        int percentageCompleted = -1;
        if (coinsCollected.Count != 0)
        {
            percentageCompleted = (totalCollected * 100 / coinsCollected.Count);
        }
        return percentageCompleted;
    }*/
}
