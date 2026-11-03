using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; set; }
    public enum playerState { isIdle, isMoving, isDisabled }
    [Space(20)]
    public GameObject playerRig;

    [Header("READ ONLY - DEBUG")]
    [SerializeField] private playerState currentState = playerState.isIdle; //default state
    // other scripts can subscribe to REACT to changes
    public event System.Action<playerState> OnStateChanged;


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

    //we will toggle true/false in the event system to call whetehr we want to move or not
   
    //          PLAYER STATE MACHINE        //
    
    public void setPlayerState(playerState newstate)
    {
        
        if (newstate == currentState) return; //same state so return

        currentState = newstate;


        // PLAYER ANIMATION NOT HANDLED HERE, HANDLED BY VELOCITY CHECK //

        switch (currentState)
        {
            case playerState.isIdle:
                Debug.Log("Player is in Idle state");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
            case playerState.isMoving:
                Debug.Log("Player is in moving state");
                break;
            case playerState.isDisabled:
                Debug.Log("Disabling player movement");
                // disable player movement
                
                break;
        }




    }

}
