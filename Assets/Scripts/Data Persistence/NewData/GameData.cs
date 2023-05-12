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
    public GameData()
    {
        Debug.Log("GameData");
        this.respawnPoint = Vector2.zero;
        this.position = Vector2.zero;


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
