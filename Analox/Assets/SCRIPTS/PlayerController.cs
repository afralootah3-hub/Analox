using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using RangeAttribute = UnityEngine.RangeAttribute;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator playerAnimator;


    public Camera _pActiveCam; //set to private later
    public InputActionAsset playerActions;
    private InputAction _clickAction;
    private InputAction _pointAction;
   

    [Header("Movement Settings")]
    [Range(0, 20)]
    public float moveSpeed = 10f;


    [Header("Object Interaction")]
    public Vector2 boxCastSize = new Vector2(0.1f, 0.1f);
    private Interactable _currentInteractable;
    public LayerMask interactLayer;

    [Tooltip("Whether or not to show a ui prompt when the player is nearby the object or whenever the mouse hovers")]
    public bool hoverWhenNearby = false; 
    private UiManager _ui;

    private List<InteractableObj> _nearby = new List<InteractableObj>();
  

    // reference interactable script / scriptable obj

    private void Awake()
    {
        //anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        
        if(playerActions == null)
        {
            Debug.LogError("MISSING INPUT ACTION ASSET FROM INSPECTOR");
        }

        // fetch needed actions
        _clickAction = playerActions.FindAction("Player/Click");
        _pointAction = playerActions.FindAction("Player/Point");
        
    }

    private void Start()
    {
        _ui = GameController.instance.GetComponent<UiManager>();
    }

    
    private void OnEnable()
    {
        playerActions.Enable();
        _clickAction.performed += OnClickPerformed;
    }

    private void OnDisable()
    {
        playerActions.Disable();
        _clickAction.performed -= OnClickPerformed;

    }

    // handles movement AND checks if we click on an object and are WITHIN RANGE
    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (GameController.instance.currentState == GameController.playerState.isDisabled) return;
        

            Debug.Log("CLICKED");

            //read current screen position of mouse
            Vector2 mouseSPos = _pointAction.ReadValue<Vector2>();

            //ray cast onto floor
            Ray ray = _pActiveCam.ScreenPointToRay(mouseSPos);
            int groundLayerMask = LayerMask.GetMask("ground");
            int objectsLayerMask = LayerMask.GetMask("iObject");

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        // checks object FIRST before ground 

        if(_nearby.Count > 0 && Physics.Raycast(ray, out RaycastHit objHit, Mathf.Infinity, objectsLayerMask))
        {
            InteractableObj clicked = objHit.collider.GetComponentInParent<InteractableObj>();

            //only inspect if inside trigger

            if(clicked != null && _nearby.Contains(clicked))
            {
                agent.ResetPath(); //stop walking
                _ui.EnterInspect(clicked.gameObject);
                return; //so we dont keep walking
            }
        }



            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
            {
                agent.SetDestination(hit.point);
            }



        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "iObj")
        {
            Debug.Log("entered TRIGGER of" + other.name);

            // add obj to list of nearby objects ( this is probably overcomplicated but for now it stays)

            InteractableObj obj = other.GetComponentInParent<InteractableObj>();

            if (obj != null && !_nearby.Contains(obj)) _nearby.Add(obj);


        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "iObj")
        {
            InteractableObj obj = other.GetComponentInParent<InteractableObj>();
            if (obj != null) _nearby.Remove(obj);
        }
    }


}
