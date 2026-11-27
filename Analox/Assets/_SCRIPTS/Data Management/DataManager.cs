using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }
    private List<IDataHandler> dataObjs = new List<IDataHandler>();
    private GameData gameData;
    private FileDataManager fileManager;
    private bool loadPending = false;
    [SerializeField] private string menuSceneName;

    [SerializeField]
    private string fileName;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(instance != this)
        {
            return;
        }

        getAllDataObj();

        if (loadPending)
        {
            loadPending = false;

            foreach(IDataHandler dataHandler in dataObjs)
            {
                dataHandler.LoadData(gameData);
            }
        }

    }

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            this.fileManager = new FileDataManager(Application.persistentDataPath, fileName);
        }


    }

    public void Start()
    {
        if(instance != this)
        {
            return;
        }

        if(SceneManager.GetActiveScene().name == menuSceneName)
        {
            return;
        }
      
        // for editor testing through a game scene //
        getAllDataObj();
        LoadGame();
    }

    public void getAllDataObj()
    {
        dataObjs.Clear();

        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < allScripts.Length; i++)
        {
            if (allScripts[i] is IDataHandler)
            {
                dataObjs.Add(allScripts[i] as IDataHandler);
            }
        }
    }

    public void NewGame()
    {
        //we're using the GameData class constructor to make sure there is some data stored in this variable
        //before we start overwriting data to it
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // TODO: Use the file writing system to deserialize the stored game data on my computer and load into this class
        this.gameData = fileManager.LoadFile();

        if (this.gameData == null)
        {
            Debug.Log("No Game Data Was Found. Loaded default data from NewGame().");
            NewGame();
        }

        Debug.Log("Loaded Player Position: " + gameData.playerPos.ToString());



        // TODO: Take the gameData variable in this class and "push" it to all of my interface instances
        foreach (IDataHandler dataHandler in dataObjs)
        {
            dataHandler.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        //never save from menu 
        if (SceneManager.GetActiveScene().name == menuSceneName) return;
        if (gameData == null) return; //protect against saving before anything loaded

        //grab active scene
        gameData.sceneName = SceneManager.GetActiveScene().name;


        // FINISHED: Get all GameData from interface instances and store in the local gameData variable
        foreach (IDataHandler dataHandler in dataObjs)
        {
            dataHandler.SaveData(ref gameData);
        }

        Debug.Log("Saved Player Position: " + gameData.playerPos.ToString());



        // FINISHED: Use the file writing system to serialize the gameData varialbe into a file on my computer
        fileManager.SaveFile(gameData);
    }

    public void PlayGame()
    {
        this.gameData = fileManager.LoadFile();

        if(this.gameData == null)
        {
            NewGame();
        }
        loadPending = true;
        SceneManager.LoadScene(gameData.sceneName);
    }

    public void OnApplicationQuit()
    {
        SaveGame();
    }
}