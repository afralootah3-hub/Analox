using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class UiManager : MonoBehaviour
{
    // MUST BE AND STAY ATTACHED TO GAMECONTROLLER GAMEOBJECT //


    //      Handles     //

    // -> handles object inspection system

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


    [Header("Object Inspection")]
    public CanvasGroup inspectPanel;
    public float fadeDuration = .3f; // <- this animates a fade in, can be changed for smth cooler

    [Tooltip("Degress of Rotation")]
    [Range(0, 1)]
    public float rotateSpeed = 200f; 
    
    //      PRIVATE REFERENCES      //
    private Renderer _inspectObjRend; // cache //
    public Transform holdpoint;
    public GameObject holdLight; 
    private Transform _inspectedObj;
    private Vector3 _savedPosition;
    private Transform _savedParent;
    private Vector3 _savedScale;
    private Quaternion _savedRotation;
    private PlayerController _playerCC; //cache to access
    private Rigidbody _savedRB;
    private bool _rbWasKinematic;
    private bool _isInspecting; //temporary bool, will be swapped to follow state machine instead

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesc;
    public TextMeshProUGUI scanDesc;

    public GameObject scanButton; 

    private ItemData _itemDataCache;

    private List<string> itemIndex; //stores the itemID's (which is the itemName)//

    private string _scanString; //cache


    private void Awake()
    {
        SetPanel(false); //setter
        itemName.text = "";
        itemDesc.text = "";
        scanDesc.text = "";
        activeCam = Camera.main;
        promptCanvas.SetActive(false);

        _playerCC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();




    }
    private void Start()
    {
        if (scanDesc == null || itemDesc == null || itemName == null) Debug.LogWarning("MISSING COMPONENT IN INSPECTOR");

    }

    private void Update()
    {
        UpdatePrompt(); 

        // temporary, swapped to state machine later
        if (!_isInspecting || _inspectedObj == null) return; //if already inspecting ignore

        // temp 
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
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

        _isInspecting = true;
        holdLight.SetActive(true);

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
        }



            // //////////////////// // NOTE: add check if item has been scanned, if you look at it again show the scanned information without needing to drain sanity

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

        scanDesc.text = _scanString; //set string 
        _itemDataCache.wasScanned = true; //set true so you dont need to rescan 
        scanButton.SetActive(false);




    }




    //called by interactableobj when player presses interact key or the exit button 

    //this will probably get swapped to a button they can press?
    public void ExitInspect()
    {
        if (!_isInspecting) return;
        holdLight.SetActive(false);

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
            _inspectedObj.transform.SetParent(null);
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
