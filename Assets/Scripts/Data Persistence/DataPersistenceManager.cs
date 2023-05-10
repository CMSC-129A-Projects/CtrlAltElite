using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull;
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameObject baseRespawn;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    private string selectedProfileID = "test";
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found one more Data Persistence Instance. Destroying the newest one");
            /*Destroy(this.gameObject);
            return;*/
        }
        instance = this;
        // DontDestroyOnLoad(this.gameObject);
        // dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        baseRespawn = GameObject.FindGameObjectWithTag("BaseRespawn");
        if (baseRespawn != null)
        {
            Debug.Log("BaseRespawn spotted at " + baseRespawn.transform.position);
            this.gameData.respawnPoint.x = baseRespawn.transform.position.x;
            this.gameData.respawnPoint.y = baseRespawn.transform.position.y;
        }
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing default data");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded player respawn: " + gameData.respawnPoint.x + " " + gameData.respawnPoint.y);
    }
    
    public void SaveGame() 
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        Debug.Log("Saved player respawn: " + gameData.respawnPoint.x + " " + gameData.respawnPoint.y);

        dataHandler.Save(gameData);
    }


    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    /*public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }*/

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
