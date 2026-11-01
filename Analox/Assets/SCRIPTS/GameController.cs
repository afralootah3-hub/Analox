using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; set; }
    public enum playerState { isIdle, isMoving, isDisabled }
    [Space(20)]
    public GameObject playerRig;

    [Header("READ ONLY - DEBUG")]
    public playerState currentState = playerState.isIdle; //default state




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
    public void setPlayerState()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        switch (currentState)
        {
            case playerState.isIdle:
                Debug.Log("Player is in Idle state");
                break;
            case playerState.isMoving:
                Debug.Log("Player is in moving state");
                break;
            case playerState.isDisabled:
                Debug.Log("Disabling player movement");
                break;
        }

        
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("DISABLED MOVEMENT");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("ENABLED MOVEMENTS");
        

    }

}
