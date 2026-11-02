using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    /* this will be restructured (eventually..)

    //this is a abstract base class that all interactable objects in the scene inherit from
    // it handles spawning and managing the popup ui above objects, and providing hover enter/exit for playercc
    // to access to drive the popup through the raycast it shoots!


    [Header("POPUP")]
    public GameObject popupPrefab; //holds the canvas and child prefab
    public Vector3 popupOffset = new Vector3(0f, 1.5f, 0f); //height above obj (fine tune later)

    private InteractablePopUp _popupInstance; //reference to the spawned popup, private so subclasses dont mess it up



    //Protected means only this CLASS (interactable) and its subclasses can see/call it
    //Virtual allows subclasses it to override it, without it a subclass start would break this
    // later on when adding other interactables, they all use the same pop up regardless, so all the logic goes
    // here to let them access it quickly without writing even more code
    protected virtual void Start()
    {
        if (popupPrefab != null)
        {
            //spawn pop up as a child so it moves w/ the obj
            //then get interactablepopup component so we can call show/hide 

            _popupInstance = Instantiate(popupPrefab, transform).GetComponent<InteractablePopUp>();

            _popupInstance.transform.localPosition = popupOffset;

        }

    }

    //called by playercc check interact when raycast hits an object w a subclass of this script
    // ? is just a null check to prevent random errors
    public void OnHoverEnter()
    {
        //this is called by playercc when raycast  hits this obj
        _popupInstance?.Show();
    }

    //called by playercc when raycast leaves this object, asw as handleinteract when the player presses E, because they'll transition into inspecting the object!
    public void OnHoverExit()
    {
        //called when raycast stops hitting obj
        _popupInstance?.Hide();
    }

  
    */

}