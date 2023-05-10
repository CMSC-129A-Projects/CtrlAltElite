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
        this.respawnPoint = Vector2.zero;
    }
}
