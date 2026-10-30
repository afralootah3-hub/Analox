using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    public Camera _pActiveCam; //set to private later
    public InputActionAsset playerActions;
    private InputAction _clickAction;
    private InputAction _pointAction;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    [Header("Input Settings")]
    public float sampleDistance = 0.5f;
    public LayerMask groundLayer;

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

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("CLICKED");
        //read current screen position of mouse
        Vector2 mouseSPos = _pointAction.ReadValue<Vector2>();

        //ray cast onto floor
        Ray ray = _pActiveCam.ScreenPointToRay(mouseSPos);
        int groundLayerMask = LayerMask.GetMask("ground");

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            agent.SetDestination(hit.point);
        }
    }



}
