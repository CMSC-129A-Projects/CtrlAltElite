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
    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Found one more Data Persistence Instance. Destroying the newest one");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    #region SCENE MANAGING
    // don't change this order
    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        Debug.Log("Persistence Manager Startup: Loading Game");
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("Saving Game");
        SaveGame();
    }

    #endregion

    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileID = newProfileId;
        LoadGame();
    }

    public void NewGame()
    {
        Debug.Log("New Game");
        this.gameData = new GameData();

        baseRespawn = GameObject.FindGameObjectWithTag("BaseRespawn");
        if (baseRespawn != null)
        {
            Debug.Log("BaseRespawn spotted at " + baseRespawn.transform.position);
            this.gameData.respawnPoint.x = baseRespawn.transform.position.x;
            this.gameData.respawnPoint.y = baseRespawn.transform.position.y;
        }
        
    }

    public void SaveGame()
    {
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. New game must be created");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            if (dataPersistenceObj != null)
            {
                dataPersistenceObj.SaveData(gameData);
            }
            
        }

        // save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileID);
    }

    public void LoadGame()
    {
        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(selectedProfileID);

        // start a new game if data is null and we want to init data if null
        if (this.gameData == null && initializeDataIfNull)
        {
            Debug.Log("Init Load");
            NewGame();
        }

        if (this.gameData == null)
        {
            Debug.Log("No data was found. New game must be created");
            return;
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

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }
}
