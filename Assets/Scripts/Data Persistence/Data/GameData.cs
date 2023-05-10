using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public float defaultMoveSpeed;
    public float defaultJumpPower;
    public float staminaMax;
    public Vector2 respawnPoint;
    public GameData()
    {
        Debug.Log("GameData");
        // this.respawnPoint = Vector2.zero;
        this.respawnPoint = new Vector2(-115.97f, -11.44f);
    }
}
