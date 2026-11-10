using UnityEngine;
using UnityEngine.SceneManagement;

public enum dayState { day1, day2, day3, gameEnd}
public class GameController : MonoBehaviour
{
    public static GameController instance { get; set; }
    [Space(20)]
    //public GameObject playerRig;

    public dayState currentDay;


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
   
    public void setDay(dayState day)
    {
        currentDay = day;

        //this is used to set which day dialog the npc does :D



    }



}
