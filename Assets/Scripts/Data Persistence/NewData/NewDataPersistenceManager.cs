using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NewDataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private string testSelectedProfileId = "test";

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    [Header("Auto Saving Configuration")]
    [SerializeField] private float autoSaveTimeSeconds = 60f;
    [SerializeField] private float autoSaveTimeAnimationSeconds = 1f;
    [SerializeField] private float autoSaveTimeAnimationFadeSeconds = 1f;
    [SerializeField] private Canvas saveLoadCanvas;
    private float _timer = 0f;
    private CanvasGroup canvasGroup;
    private bool fadeOutAnimation = false;
    private bool fadeInAnimation = false;

    public GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private NewFileDataHandler dataHandler;

    private string selectedProfileId = "";

    private Coroutine autoSaveCoroutine;

    public static NewDataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistence)
        {
            //Debug.LogWarning("Data Persistence is currently disabled!");
        }

        this.dataHandler = new NewFileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        canvasGroup = saveLoadCanvas.GetComponent<CanvasGroup>();
        InitializeSelectedProfileId();
        InitializeSaveLoadGUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

        // start up the auto saving coroutine
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        if (SceneManager.GetActiveScene().name != "CharacterCustomization" &&
            SceneManager.GetActiveScene().name != "TestMenuSave")
        {
            autoSaveCoroutine = StartCoroutine(AutoSave());
        }
            
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        // update the profile to use for saving and loading
        this.selectedProfileId = newProfileId;
        // load the game, which will use that profile, updating our game data accordingly
        LoadGame();
    }

    public string GetCurrentProfileId()
    {
        return this.selectedProfileId;
    }
    public void DeleteSelectedProfileId()
    {
        dataHandler.Delete(this.selectedProfileId);
        InitializeSelectedProfileId();
    }

    public void DeleteProfileData(string profileId)
    {
        // delete the data for this profile id
        dataHandler.Delete(profileId);
        // initialize the selected profile id
        InitializeSelectedProfileId();
        // reload the game so that our data matches the newly selected profile id
        LoadGame();
    }

    private void InitializeSelectedProfileId()
    {
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testSelectedProfileId;
            //Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedProfileId);
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // return right away if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(selectedProfileId);

        // start a new game if the data is null and we're configured to initialize data for debugging purposes
        if (this.gameData == null && initializeDataIfNull)
        {
            //Debug.Log("Here");
            NewGame();
        }

        // if no data can be loaded, don't continue
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;
        }

        // push the loaded data to all other scripts that need it
        if (dataPersistenceObjects != null)
        {
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadData(gameData);
            }
        }
        
    }

    public void IncrementSceneIndex()
    {
        gameData.previousSceneIndex++;
        gameData.newGame = true;

        // timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileId);
    }

    public void SaveGame()
    {
        // return right away if data persistence is disabled
        if (disableDataPersistence)
        {
            Debug.Log("Disabled Data Persistence");
            return;
        }

        // if we don't have any data to save, log a warning here
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            // NewGame();
            return;
        }

        if (dataPersistenceObjects == null)
        {
            // Debug.Log("No Data Persistence Objects");
            return;
        }
        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        // timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileId);
        // 
    }

    public void StartSaveAnimation()
    {
        StartCoroutine(PlaySaveAnimation());
    }

    private void InitializeSaveLoadGUI()
    {
        saveLoadCanvas.gameObject.SetActive(false);
    }
    private IEnumerator PlaySaveAnimation()
    {
        yield return null;
        saveLoadCanvas.gameObject.SetActive(true);
        fadeInAnimation = true;
        fadeOutAnimation = false;
        yield return new WaitForSeconds(autoSaveTimeAnimationSeconds);
        fadeInAnimation = false;
        fadeOutAnimation = true;
    }
    private void Update()
    {

        if (fadeInAnimation)
        {
            _timer += Time.deltaTime;
            // Calculate the normalized progress of the animation
            float progress = _timer / (autoSaveTimeAnimationSeconds / 2);
            // Increase the canvas group's alpha based on the progress
            canvasGroup.alpha = progress;

            if (_timer >= autoSaveTimeAnimationFadeSeconds)
            {
                _timer = 0f;
                fadeInAnimation = false;
            }
        }

        if (fadeOutAnimation)
        {
            _timer += Time.deltaTime;

            // Calculate the normalized progress of the animation
            float progress = _timer / autoSaveTimeAnimationFadeSeconds;

            // Reduce the canvas group's alpha based on the progress
            canvasGroup.alpha = 1f - progress;

            if (_timer >= autoSaveTimeAnimationFadeSeconds)
            {
                _timer = 0f;
                fadeOutAnimation = false;
                saveLoadCanvas.gameObject.SetActive(false);
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return dataHandler.HasFilesIn();
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    private IEnumerator AutoSave()
    {
        Debug.Log("AutoSave Started");
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            SaveGame();
            Debug.Log("Auto Saved Game");
            StartSaveAnimation();
        }
    }
}