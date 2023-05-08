using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public float defaultMoveSpeed;
    public float defaultJumpPower;
    public float staminaMax;
    public float[] currentRespawnPosition;

    public PlayerSaveData(SugboMovement player)
    {
        defaultMoveSpeed = player.defaultMoveSpeed;
        defaultJumpPower = player.defaultJumpPower;
        staminaMax = player.staminaMax;

        currentRespawnPosition = new float[3];
        currentRespawnPosition[0] = player.death.respawnPosition[0];
        currentRespawnPosition[1] = player.death.respawnPosition[1];
        currentRespawnPosition[2] = player.death.respawnPosition[2];
    }
    
}
