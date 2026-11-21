using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public enum dayState { day1, day2, day3, gameEnd}
public class GameController : MonoBehaviour
{
    public static GameController instance { get; set; }
    [Space(20)]
    //public GameObject playerRig;

    public dayState currentDay;

    public Vector3 spawnPlayerPos; //where we load the player on sceneload
    public Vector3 lastPlayerSpawn; // for when entering main house, so we leave from the room we came into

    private bool _hasSavedPosition;
    [SerializeField] private PlayerController _playerCC;

    public GameObject cameraPrefab;

    [SerializeField] private LoadRoom _loadRoomCache;
    
     public string MainHouse, Apartment, Bedroom, Office, MainMenu, Credits;

    private void Update()
    {
        if (!TryGetPlayerCC()) return;


        //if (GameObject.FindGameObjectWithTag("camHolder") == null)
        //{
        //    Debug.Log("SPAWNED CAM");
        //    _playerCC._pActiveCam = null;
        //    Instantiate(cameraPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        //    _playerCC.ReloadPlayerVariables();
        //}
        //else
        //{
        //    Debug.Log("CAM PREFAB ALREADY EXISTS");
        //}

        if (_loadRoomCache == null)
        {
            _loadRoomCache = FindFirstObjectByType<LoadRoom>();
        }

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
           


            Debug.Log("load room cache is " + _loadRoomCache
                    + "\n loadroom spawn position is : " + _loadRoomCache.spawnPoint.position);
           

            
            _playerCC.transform.position = _loadRoomCache.spawnPoint.position;

            //_playerCC.transform.position = lastPlayerSpawn;
            
        }
    }

    private bool TryGetPlayerCC()
    {
        if(_playerCC != null)
        {
            return true;
        }

        _playerCC = FindFirstObjectByType<PlayerController>();
        if(_playerCC == null)
        {
            Debug.LogWarning("NO PLAYER CONTROLLER FOUND");
            return false;
        }

        return true;

    }

    public void Awake()
    {
        if (instance != null && instance != this)
        {

            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;

            //ad scene manager stuff here 

            DontDestroyOnLoad(this);
        }
    }

    public void InsantiateCameraSetup()
    {
        if (GameObject.FindGameObjectWithTag("camHolder") == null)
        {
            _playerCC._pActiveCam = null;
           Instantiate(cameraPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            _playerCC.ReloadPlayerVariables();
        }
        else
        {
            Debug.Log("CAM PREFAB ALREADY EXISTS");
        }
    }

    private void OnEnable()
    {
        if(instance == this)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDisable()
    {
        if(instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; 
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MainMenu) return;
        if (scene.name == Credits) return;

        _loadRoomCache = FindFirstObjectByType<LoadRoom>();
        InsantiateCameraSetup();

        if(scene.name == MainHouse)
        {
            if (_hasSavedPosition)
            {
                MovePlayerTo(lastPlayerSpawn);
                return;
            }
        }

        if(_loadRoomCache == null)
        {
            Debug.LogWarning("NO LOADROOM IN " + scene.name);
            return;
        }

        MovePlayerTo(_loadRoomCache.spawnPoint.position);

    }

    private void MovePlayerTo(Vector3 position)
    {
        if (!TryGetPlayerCC()) return;



        NavMeshAgent agent = _playerCC.GetComponent<NavMeshAgent>();

        if(agent == null)
        {
            _playerCC.transform.position = position;
        }

        agent.ResetPath();

        //snap to nearest navmesh point 

        if(NavMesh.SamplePosition(position, out NavMeshHit navhit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(navhit.position);
        }
        else
        {
            Debug.LogWarning(("SPAWN POSITION NOT SET NEAR NAVMESH") + position);
            agent.Warp(position);
        }
    }


    //we will toggle true/false in the event system to call whetehr we want to move or not

    public void setDay(dayState day)
    {
        currentDay = day;

        //this is used to set which day dialog the npc does :D



    }

    public void OnSceneLoad(string sceneToLoad)
    {
          if (sceneToLoad == "") Debug.LogWarning("DOOR IS MISSING STRING IN INSPECTOR \n CANNOT LOAD SCENE");
    
        // only save pos if leaving main house

        if(SceneManager.GetActiveScene().name == MainHouse)
        {
            if (TryGetPlayerCC())
            {

                lastPlayerSpawn = _playerCC.transform.position;
                _hasSavedPosition = true;

            }
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    



}
