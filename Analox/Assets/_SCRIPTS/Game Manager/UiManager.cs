using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Yarn.Unity;


public class UiManager : MonoBehaviour
{
    // MUST BE AND STAY ATTACHED TO GAMECONTROLLER GAMEOBJECT //
    

    

    // -> handles object inspection system
    #region VARIABLES

    [Header("UI ELEMENTS")]
    [Space(20)]
    [Header("INTERACTION PROMPT")]
    private Vector3 _basePosition; //set by interactable via popupOffset which will sit on the interactable obj itself (later)
    public Vector3 _popUpOffset = new Vector3(0, 2, 0); // TEMP //
    public GameObject promptCanvas;
    public Camera activeCam;
    private Transform _promptTarget;
    private bool _visible; //debug temp var
    // this is basic pop up stuff, but you can add animation stuff to this script //

    private MenusManager _menusManager;

    [Header("Object Inspection")]
    public CanvasGroup inspectPanel;
    public float fadeDuration = .3f; // <- this animates a fade in, can be changed for smth cooler (UNUSED RN)
    public float scaleMult = 3f;
    public Transform holdpoint;
    public GameObject holdLight;

    [Tooltip("Degress of Rotation")]
    [Range(0, 1)]
    public float rotateSpeed = 200f;

    [Header("NPC INTERACTION STUFF")]
    public CanvasGroup npcInspectPanel;
    public TextMeshProUGUI scanInfoTMP;
    public TextMeshProUGUI reportInfoTMP;

    public CanvasGroup npcScanDesc;
    public bool isInspectingNPC;
    public GameObject scanNpcButton;

    [Header("YARNSPINNER DROP DOWN MENU VARS")]
    [SerializeField] private GameObject _dropPanel;
    [SerializeField] private TMP_Dropdown _dropDownTMP;
    [SerializeField] private InMemoryVariableStorage _variableStorage;

    private string chosenNode; //what option the player picks from dropdown
    
    //node names in the same order as dropdown entries
    private List<string> nodeNames = new List<string>(); 

    

    #region ///           PRIVATE REFERENCES    not organized </3         ///
    private Renderer _inspectObjRend; // cache //
    private Transform _inspectedObj;
    private Vector3 _savedPosition;
    private Transform _savedParent;
    private Vector3 _savedScale;
    private Quaternion _savedRotation;
    private PlayerController _playerCC; //cache to access
    private Rigidbody _savedRB;
    private bool _rbWasKinematic;
    private bool _isInspecting; //temporary bool, will be swapped to follow state machine instead

    private ItemData _itemDataCache;
    private npcData _npcDataCache;
    private Transform _doorTransformCache;
    private PillManager _pillmanager;
    private List<string> itemIndex; //stores the itemID's (which is the itemName)//
    private string _scanString; //cache
    


    #endregion
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesc;
    public TextMeshProUGUI scanDesc;

    public GameObject scanButton;

    


    [Tooltip("This is how long doors/drawers wait before closing after opening them")]
    public float doorCooldown = 5f;

    #endregion

    private void Awake()
    {
        SetPanel(false); //setter
        itemName.text = "";
        itemDesc.text = "";
        scanDesc.text = "";
        activeCam = Camera.main;
        promptCanvas.SetActive(false);
        npcInspectPanel.gameObject.SetActive(false);
        _dropPanel.SetActive(false);
        _dropDownTMP.onValueChanged.AddListener(OnDropdownChanged);

        _playerCC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _pillmanager = GetComponent<PillManager>();
        _menusManager = GetComponent<MenusManager>();


    }
    private void Start()
    {
        if (scanDesc == null || itemDesc == null || itemName == null) Debug.LogWarning("MISSING COMPONENT IN INSPECTOR");
        if (npcScanDesc == null) Debug.LogWarning("MISSING NPCSCANDESC IN INSPECTOR");
    }

    /*  Dropdown logic
        1. yarn runs this command <<showDropDown GAMEMANAGER "NPCprefix">> 
             the prefix is just what the yarnspinner nodes start with, like npc name 
        2. build a dropdown based on the players inventory
             each item is paired with the name of the yarn node that should play if the 
             player picks the option, built from Npc prefix: "name" + "_show" + "objectname"
        3. The method is a coroutine, so yarn waits for it to finish, it shows panel then
            pauses on WaitUntil, keeping the YS script paused on the command

                ^ this just makes it easier than individually assigning a node per item
                    manually here
        4. when the player picks smth, OnDropDownChanged stores the matching node in chosen node
            and ends the WaitUntil

        5. the method hides the panel and writes chosenNode into the YS variable $next_node
            yarn then handles everything after that with a jump command 
    */

    [YarnCommand("showDropdown")]
    public IEnumerator ShowInventoryDropdown(string npcPrefix)
    {
        //reset from earlier uses
        chosenNode = null;
        nodeNames.Clear();

        List<string> labels = new List<string>();

        //index 0 is a placeholder
        // a drop down doesn't fire its change event when you pick the entry that's already
        // selected, so this placeholder makes sure the first real item always counts as a change
        labels.Add("Choose..");
        nodeNames.Add("");

        //one entry per item, label is itemID which is just its name
        // organize it by npcprefix_show_itemname
        foreach(string itemID in PlayerUI.instance._inventoryItems)
        {
            labels.Add(itemID);
            nodeNames.Add(npcPrefix + "_show" + itemID);
        }

        //this should always be the last entry for convinience of the player
        labels.Add("Never mind");
        nodeNames.Add(npcPrefix + "_leave");

        // rebuild the dropdown each time and select the placeholder without firing change event
        _dropDownTMP.ClearOptions();
        _dropDownTMP.AddOptions(labels);
        _dropDownTMP.SetValueWithoutNotify(0);
        _dropDownTMP.RefreshShownValue();

        _dropPanel.SetActive(true);

        //yarn stays stuck until chosen note is set, because theres a <<jump>> that we don't want
        // to fire immediately until the player decides which node to go to
        yield return new WaitUntil(() => chosenNode != null);

        //send the node to yarnspinner so it automatically knows what node to jump to
        _dropPanel.SetActive(false);
        _variableStorage.SetValue("$next_node", chosenNode);
    
    }

    private void OnDropdownChanged(int index)
    {
        if(index == 0) //ignore placeholder
        {
            return;
        }

        //look up the node name for picked entry, this also ends the WaitUntil
        chosenNode = nodeNames[index];
    }

    private void Update()
    {
        if(isInspectingNPC && (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame))
        {
            Debug.Log("Exiting npc inspection");
            CloseNPCPanel();
            isInspectingNPC = false;
        }

        //Debug.Log(isInspectingNPC);


        UpdatePrompt();

        // temporary, swapped to state machine later
        if (!_isInspecting || _inspectedObj == null) return; //if already inspecting ignore

        // temp 
        if (Keyboard.current.escapeKey.wasPressedThisFrame || (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame) )
        {
            ExitInspect();

            return;
        }


        RotatetInspectedObj();

    }

    #region object inspection stuff
    public void EnterInspect(GameObject targetOBJ)
    {
        HidePrompt();
        if (_isInspecting) return;

        PillManager.instance.pillButton.SetActive(false);

        _isInspecting = true;
        //holdLight.SetActive(true);

        //disable player input HERE
        _inspectedObj = targetOBJ.transform;



        //      SET UP      //

        // disable input
        _playerCC.setPlayerState(PlayerController.playerState.isDisabled); // lockinput

        InteractableObj itemContainer = targetOBJ.GetComponent<InteractableObj>(); //fetch itemdata
        _itemDataCache = itemContainer.itemdata;
        _inspectObjRend = itemContainer.itemRenderer; // cache renderer to disable casting shadows on the ground

        _inspectObjRend.shadowCastingMode = ShadowCastingMode.Off; // turn off cast shadows to not cast shadows on the ground 
        _inspectObjRend.receiveShadows = false; //also recieving shadows 


        itemName.text = _itemDataCache.ItemName;
        itemDesc.text = _itemDataCache.ItemDescription;
        _scanString = _itemDataCache.ScanInformation; //cache text but don't show it yet

        // ADD TO LIST AND ENABLE BOOL //

        //if (!itemIndex.Contains(_itemDataCache.ItemName))
        //{
        //    itemIndex.Add(_itemDataCache.ItemName);  //add to list
        //}

        _menusManager.AddObjectClue(_itemDataCache); // menus manager handles duplicates (hopefully)

        if (_itemDataCache.wasScanned)
        {
            //if it was already scanned show the text 
            // DONT invoke scan function because it decreases sanity
            scanButton.SetActive(false);

            scanDesc.text = _scanString;

          

        }
        else
        {
            scanButton.SetActive(true);

            if (!PlayerUI.instance._inventoryItems.Contains(_itemDataCache.ItemName))
            {
                Debug.Log("added item to inventory (not scanned)");
                MenusManager.instance.notifPopUp("Updated corkboard \n [TAB]");
                PlayerUI.instance.addItem(_itemDataCache.ItemName);
            }

        }


        // add object to list to inventory 

        _savedRB = targetOBJ.GetComponent<Rigidbody>();

        if (_savedRB != null)
        {
            //some objects might be permanently kinematic to make sure they dont fall off anything
            _rbWasKinematic = _savedRB.isKinematic;
            _savedRB.isKinematic = true; //make them kinematic 
        }

        //cache current transforms and rotations 
        _savedScale = _inspectedObj.localScale;
        _savedParent = _inspectedObj.parent;
        _savedPosition = targetOBJ.transform.position;
        _savedRotation = targetOBJ.transform.rotation;

        // worldposition stays false keeps the local position as relative to the new parent 
        // this basically makes sure the obj doesnt stay floating wherever it was
        targetOBJ.transform.SetParent(holdpoint, worldPositionStays: false);
        targetOBJ.transform.localPosition = Vector3.zero;
        targetOBJ.transform.localRotation = Quaternion.identity;
        targetOBJ.transform.localScale = _savedScale * scaleMult;


        // show cursor and do some cool transition to inspection (idk) 

        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = false;

        //   cool ui stuff can go here (for transitioning INTO inspection)

        SetPanel(true); // setter
    }

    //          SCAN STUFF          //

    public void OnScannedObject()
    {
        // this shows the scanned information (for now it just appears but it'll be a cool transition eventually)

        // when click button to scan (or whatever you do)

        // do some animation/transition stuff and show text 
        _pillmanager.SetSanity(-1);
        scanDesc.text = _scanString; //set string 
        _itemDataCache.wasScanned = true; //set true so you dont need to rescan 
        scanButton.SetActive(false);

        // if was scanned, alter its index in the list 

        if (PlayerUI.instance._inventoryItems.Contains(_itemDataCache.ItemName))
        {
            MenusManager.instance.notifPopUp("Updated corkboard \n [TAB]");
            Debug.Log("updated inventory index to have updated altered itemID");
            PlayerUI.instance._inventoryItems.Remove(_itemDataCache.ItemName);

            PlayerUI.instance.addItem(_itemDataCache.ItemName + "_scan"); // alter index to mark we have the scanned info 
        }


    }

    #region NPC Interaction stuff
    public void OnScannedNPC() //called by button 
    {
        // pull the scan desc (AND UPDATE TO CORKBOARD INFO? Or this gets updated on  manager?? i forgot)
        /*
        npcInspectPanel.gameObject.SetActive(true);
        scanInfoTMP.text = nData.scanDesc;
        isInspectingNPC = true;
        if (nData.hasScannedNPC) return;
        MenusManager.instance.notifPopUp("Updated corkboard \n [TAB]");
        _pillmanager.SetSanity(-1);
        nData.hasScannedNPC = true;
        */

        // when hit the button to scan display text, and later add some cool animations or smth 
        if (_npcDataCache == null) return;
        if (_npcDataCache.hasScannedNPC) return;
        isInspectingNPC = true;
        MenusManager.instance.notifPopUp("Updated corkboard! \n [TAB]");
        _pillmanager.SetSanity(-1);
        scanInfoTMP.text = _npcDataCache.scanDesc;
        npcScanDesc.alpha = 1; // make it visible
        scanNpcButton.SetActive(false); //get rid of button
        _npcDataCache.hasScannedNPC = true;

    }
    


    public void OnViewNPCInfo(npcData nData)
    {
        // viewing the info
        // save cache for running OnScan
        _npcDataCache = nData;


        npcInspectPanel.gameObject.SetActive(true);
        reportInfoTMP.text = _npcDataCache.reportInfoDesc;
        
        isInspectingNPC = true;

        if (_npcDataCache.hasScannedNPC)
        {
            scanInfoTMP.text = _npcDataCache.scanDesc;
            npcScanDesc.alpha = 1; // make it visible
            scanNpcButton.SetActive(false); //get rid of button
        }
        else
        {
            npcScanDesc.alpha = 0;
        }


    }




    public void CloseNPCPanel()
    {
        _npcDataCache = null;
        npcInspectPanel.gameObject.SetActive(false);
        reportInfoTMP.text = "";
        _playerCC.setPlayerState(PlayerController.playerState.isIdle);
        scanInfoTMP.text = "";
        npcScanDesc.alpha = 0;
    }


    #endregion



    //called by interactableobj when player presses interact key or the exit button 

    //this will probably get swapped to a button they can press?
    public void ExitInspect()
    {
        if (!_isInspecting) return;
        //holdLight.SetActive(false);

        PillManager.instance.pillButton.SetActive(true);
        itemName.text = "";
        itemDesc.text = "";
        scanDesc.text = "";
        _itemDataCache = null; //clear cache

        _inspectObjRend.shadowCastingMode = ShadowCastingMode.On; // turn on cast shadows
        _inspectObjRend.receiveShadows = true; //recieving shadows on

        // clear cache//
        _inspectObjRend = null;

        _isInspecting = false;
        SetPanel(false);
        _playerCC.setPlayerState(PlayerController.playerState.isIdle); //revert to idle

        //return obj position to its original orientation 
        if (_inspectedObj != null)
        {
            _inspectedObj.transform.SetParent(_savedParent);
            _inspectedObj.transform.position = _savedPosition;
            _inspectedObj.transform.rotation = _savedRotation;
            _inspectedObj.transform.localScale = _savedScale;

            if (_savedRB != null)
            {
                _savedRB.isKinematic = _rbWasKinematic;
            }

            _inspectedObj = null;

        }

        //relock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = true;



    }

    public bool IsInspecting => _isInspecting;


    //                  INTERACTION ROTATION WITH MOUSE             //

    private void RotatetInspectedObj()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        /* old code
        //rotate around world axes
        //space.world makes sure the object rotates relative to the global coordinate system,
        // it moves along global axes regardless of which way the object is currently facing

        //note this is a little janky and mind need to be restructured ??

        _inspectedObj.transform.Rotate(holdpoint.up
            , -mouseX * rotateSpeed * Time.deltaTime, Space.World);
        _inspectedObj.transform.Rotate(holdpoint.right,
            mouseY * rotateSpeed * Time.deltaTime, Space.World);
        */



        // only rotate while dragging with left mouse button (clikc and dragged)
        // isPressed is true every frame the button stays down
        if (!Mouse.current.leftButton.isPressed) return;

        // delta means change -> its how far the mouse moved since the LAST frame
        // delta.x > 0 -> mouse moved right -- delta.x < 0 -> mouse moved left
        // delta.y does the same for up and down
        // delta is 0,0 if mouse is held still

        Vector2 delta = Mouse.current.delta.ReadValue();
        Transform cam = activeCam.transform;

        //rotate around CAMERAS axes so obj follows drag on screen

        //-delta is used as without it the controls are inverted (idk why)

        // 
        _inspectedObj.Rotate(cam.up, -delta.x * rotateSpeed, Space.World);
        _inspectedObj.Rotate(cam.right, delta.y * rotateSpeed, Space.World);

    }
    public void OpenDrawer(ItemData drawerData)
    {
        Debug.Log(drawerData.isOpeningDrawer);
        if (drawerData.isOpeningDrawer) return;
        if (drawerData.drawerIsOpen) return;
        Vector3 targetPosition;

        Debug.Log("OPENDRAWER RUN");

        drawerData.drawerIsOpen = !drawerData.drawerIsOpen;

        if (!drawerData.drawerIsOpen)
        {
            targetPosition = new Vector3(drawerData.drawerPivot.localPosition.x, drawerData.drawerPivot.localPosition.y, drawerData.drawerPivot.localPosition.z - .5f);
            drawerData.insideObjCol.isTrigger = false;
        }
        else
        {
            targetPosition = new Vector3(drawerData.drawerPivot.localPosition.x, drawerData.drawerPivot.localPosition.y, drawerData.drawerPivot.localPosition.z + .5f);
            drawerData.insideObjCol.isTrigger = true;

        }

        StartCoroutine(OpenDrawer(drawerData, targetPosition, 1f));


    }

    #region opening doors stuff
    public void OpenDoor(InteractableObj intOBJ)
    {
        if (intOBJ == null) return;
        if (intOBJ.isOpeningDoor) return;

        
        // call routine
        StartCoroutine(OpenDoorCor(intOBJ));




    }

    private IEnumerator OpenDoorCor(InteractableObj door)
    {
        door.isOpeningDoor = true;

        Quaternion openRot = door.closedRot * Quaternion.Euler(0, door.itemdata.openAngle, 0);

        yield return StartCoroutine(RotateDoor(door.transform, openRot, 3f));
        yield return new WaitForSeconds(doorCooldown);
        yield return StartCoroutine(RotateDoor(door.transform, door.closedRot, 3f));
        door.isOpeningDoor = false;
        

    } 

    private IEnumerator RotateDoor(Transform pivot, Quaternion targetRot, float duration)
    {
       
        Quaternion startRot = pivot.localRotation;
        float t = 0f; 

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            pivot.localRotation = Quaternion.Slerp(startRot, targetRot, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        pivot.localRotation = targetRot;
        

        

    }



    #endregion


    private IEnumerator OpenDrawer(ItemData drawer, Vector3 targetPos, float duration)
    {
        drawer.isOpeningDrawer = true;

        Vector3 startPos = drawer.drawerPivot.localPosition;
        Debug.Log("IS OPENING DRAWER");

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            drawer.drawerPivot.localPosition = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        drawer.drawerPivot.localPosition = targetPos;
        drawer.isOpeningDrawer = false; 

    }

    private void SetPanel(bool visible)
    {
        inspectPanel.alpha = visible ? 1f : 0f;
        inspectPanel.interactable = visible;
        inspectPanel.blocksRaycasts = visible;
    }

    #endregion

    #region interaction pop up 

    public void ShowPrompt(Transform target)
    {
        if (_isInspecting) return;
        _promptTarget = target;
        promptCanvas.SetActive(true);
    }

    public void HidePrompt()
    {
        
        _promptTarget = null;
        promptCanvas.SetActive(false);
    }

    private void UpdatePrompt()
    {
        if (_promptTarget == null) return;
        // add any animation here //

        Transform t = promptCanvas.transform;
        t.position = _promptTarget.position + _popUpOffset;

        if(activeCam != null)
        {
            t.rotation = Quaternion.LookRotation(t.position - activeCam.transform.position);
        }
    }

    #endregion

    


}
