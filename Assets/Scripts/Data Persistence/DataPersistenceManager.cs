using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameObject baseRespawn;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found one more Data Persistence Instance");
        }
        Instance = this;
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        Debug.Log("Persistence Manager Startup: Loading Game");
        LoadGame();
    }

    public void NewGame()
    {
        Debug.Log("New Game");
        baseRespawn = GameObject.FindGameObjectWithTag("BaseRespawn");
        Debug.Log("BaseRespawn spotted at " + baseRespawn.transform.position);
        this.gameData = new GameData();
        this.gameData.respawnPoint.x = baseRespawn.transform.position.x;
        this.gameData.respawnPoint.y = baseRespawn.transform.position.y;
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        // save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    public void LoadGame()
    {
        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();


        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing default values");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Persistence Manager AppQuit: Saving Game");
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
