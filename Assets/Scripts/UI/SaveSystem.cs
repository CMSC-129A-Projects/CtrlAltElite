using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem
{
    public static void SavePlayer(SugboMovement player)
    {
        Debug.Log("Saving Player");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.data";

        try
        {
            if (File.Exists(path)) 
            { 
                Debug.Log("Data exists. Deleting old file and writing a new one.");
                File.Delete(path);
            }
            FileStream stream = new FileStream(path, FileMode.Create);

            PlayerSaveData data = new PlayerSaveData(player);
            formatter.Serialize(stream, data);
            stream.Close();
        }
        catch(Exception e)
        {
            Debug.LogError($"Unable to save due to {e.Message}");
        }
    }

    public static PlayerSaveData LoadPlayer()
    {
        Debug.Log("Loading Player");
        string path = Application.persistentDataPath + "/player.data";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerSaveData data = formatter.Deserialize(stream) as PlayerSaveData;

            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("File not found in " + path);
            return null;
        }
    }
}
