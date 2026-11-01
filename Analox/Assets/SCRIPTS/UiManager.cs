using UnityEngine;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    // MUST BE AND STAY ATTACHED TO GAMECONTROLLER GAMEOBJECT //


    //      Handles     //

    // -> handles object inspection system


    [Header("Object Inspection")]
    public CanvasGroup inspectPanel;
    public float fadeDuration = .3f; // <- this animates a fade in, can be changed for smth cooler

    
    [Tooltip("How fast the object can be rotated when being inspected")]
    public float rotateSpeed = 200f;

    //      PRIVATE REFERENCES      //
    public Transform holdpoint;
    private Transform _inspectedObj;
    private Vector3 _savedPosition;
    private Transform _savedParent;
    private Vector3 _savedScale;
    private Quaternion _savedRotation;
    private Rigidbody _savedRB;
    private bool _rbWasKinematic;
    private bool _isInspecting; //temporary bool, will be swapped to follow state machine instead



    private void Awake()
    {
        SetPanel(false); //setter
    }

    private void Update()
    {
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
        if (_isInspecting) return;

        _isInspecting = true;

        //disable player input HERE
        _inspectedObj = targetOBJ.transform;

        GameController.instance.currentState = GameController.playerState.isDisabled;

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

    //called by interactableobj when player presses interact key or the exit button 

    //this will probably get swapped to a button they can press?
    public void ExitInspect()
    {
        if (!_isInspecting) return;
        _isInspecting = false;
        SetPanel(false);
        GameController.instance.currentState = GameController.playerState.isIdle;

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

        //rotate around world axes
        //space.world makes sure the object rotates relative to the global coordinate system,
        // it moves along global axes regardless of which way the object is currently facing

        //note this is a little janky and mind need to be restructured ??

        _inspectedObj.transform.Rotate(holdpoint.up
            , -mouseX * rotateSpeed * Time.deltaTime, Space.World);
        _inspectedObj.transform.Rotate(holdpoint.right,
            mouseY * rotateSpeed * Time.deltaTime, Space.World);

    }

    private void SetPanel(bool visible)
    {
        inspectPanel.alpha = visible ? 1f : 0f;
        inspectPanel.interactable = visible;
        inspectPanel.blocksRaycasts = visible;
    }

    #endregion


}
