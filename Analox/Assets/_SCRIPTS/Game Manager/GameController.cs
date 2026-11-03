using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; set; }
    [Space(20)]
    public GameObject playerRig;

  


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
   

}
