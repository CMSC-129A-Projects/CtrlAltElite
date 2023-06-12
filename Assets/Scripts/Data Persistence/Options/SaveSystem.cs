using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Unity.VisualScripting;

public static class SaveSystem
{
    public static void SavePlayerOptions(OptionsMenu options)
    {
        // Debug.Log("Saving Player Options");
        string path = Application.persistentDataPath + "/playerOptions.data";

        try
        {
            if (File.Exists(path)) 
            { 
                // Debug.Log("Data exists. Deleting old file and writing a new one.");
                File.Delete(path);
            }

            OptionsData data = new OptionsData(options);

            string dataToStore = JsonUtility.ToJson(data, true);

            // write the serialized data to the file
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch(Exception e)
        {
            Debug.LogError($"Unable to save due to {e.Message}");
        }
    }

    public static OptionsData LoadPlayerOptions()
    {
        // Debug.Log("Loading Player Options");
        string path = Application.persistentDataPath + "/playerOptions.data";
        OptionsData loadedData = null;

        if (File.Exists(path))
        {
            string dataToLoad = "";
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }
            // deserialize the data from Json back into the C# object
            loadedData = JsonUtility.FromJson<OptionsData>(dataToLoad);

            return loadedData;
        }
        else
        {
            Debug.Log("File not found in " + path);
            return null;
        }
    }
}
