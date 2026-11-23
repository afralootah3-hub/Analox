using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadRoom : MonoBehaviour
{
    public string loadTo;
    [Header("This is where the player SPAWNS when they \n enter this room.")]
    public Transform spawnPoint;
   // public Transform roomSpawn; //if we dont need to track lastpos

    public GameObject doorChoiceHolder;

  
    

    public Button leaveButton, stayButton;

    private PlayerController _playercc;

    private Collider col;
    private bool canInteractDoor;

    public bool _lockDoor;

    private void Awake()
    {
        InitalizeDoor();
        doorChoiceHolder.SetActive(false);
        col = GetComponent<SphereCollider>();
        
    }
    private void Start()
    {
        
    }
    public void OpenDoorMenu()
    {


        _playercc.setPlayerState(PlayerController.playerState.isDisabled);
        doorChoiceHolder.SetActive(true);
        leaveButton.onClick.AddListener(EnterDoor);
        stayButton.onClick.AddListener(CloseDoorMenu);
    }


    #region button functions
    public void Lockdoor()
    {
        col.enabled = false;
        GameController.instance.lockedDoors.Add(col);
    }

    public void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            EnterDoor(); 
        }
    }

    public void EnterDoor()
    {
        //called by button and invokes method in gamecontroller
        //  doorChoiceHolder.SetActive(false);

        if (_lockDoor) Lockdoor();
        GameController.instance.OnSceneLoad(loadTo);
    }

    public void CloseDoorMenu()
    {
        _playercc.setPlayerState(PlayerController.playerState.isIdle);
        doorChoiceHolder.SetActive(false);
        leaveButton.onClick.RemoveListener(EnterDoor);
        stayButton.onClick.RemoveListener(CloseDoorMenu);
    }

    #endregion


    public void InitalizeDoor() //called by gamecontroller onsceneLoad :D
    {
        doorChoiceHolder = GameObject.FindGameObjectWithTag("DoorHolder");
        _playercc = FindFirstObjectByType<PlayerController>();

        doorChoiceHolder.SetActive(false);
        Debug.Log("Loaded Door");


        if (doorChoiceHolder == null || _playercc == null) Debug.LogWarning("DOOR FAILED TO GRAB COMPONENT");
    }

    private void OnTriggerEnter(Collider other)
    {
        canInteractDoor = true;
    }

    private void OnTriggerExit(Collider other)
    {
        canInteractDoor = false;

    }

}
