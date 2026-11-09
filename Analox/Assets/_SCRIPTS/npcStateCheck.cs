using UnityEngine;

public class npcStateCheck : MonoBehaviour
{    
    //  The below method uses a 'string' as the return value 
    //  all methods can use a data type, but they MUST have a 'return' keyword when calling
    //  when this method is called, it will then return the 'currentPlayerState' string value 

    public string checkState(dayState currentDay) 
    {
        string currentPlayerState = "";

        //   'switch' is a type of logic that easily replaces if/else if/else blocks
        //   we use 'case' to see if the arguement in the switch is met 
        //   all 'cases' must include a 'break' in order for the switch to operate as intended



        // the string passed here should NOT be the same as the ENUM -- it will be passed to YS //

        switch (currentDay)
        {
            case dayState.day1:
                //  in this particular switch we want to return a string value that WILL be PASSED into our Dialog Runner (Yarn Spinner) class
                currentPlayerState = "startDay1";
                Debug.Log("CURRENT DAY IS DAY 1");
                break;
            case dayState.day2:
                currentPlayerState = "startDay2";
                Debug.Log("CURRENT DAY IS DAY 2");
                break;
        }

        return currentPlayerState;
    
    }


}
