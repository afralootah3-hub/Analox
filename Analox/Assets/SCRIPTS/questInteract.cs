using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class questInteract : MonoBehaviour
{
    /*
    public string itemID;
    public TextMeshProUGUI interactText;
    public bool canInteract;
    //public updateDialogPhase changePhase;

    public GameObject[] objsToDestory;

    
    public void Awake()
    {
        itemID = this.name; //used to track which fruit you got. (you need to the correct 2, if you have the wrong ones you get a dialog)
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            // do something
            setText("pick up? (e)");
            canInteract = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // do something
            setText("");
            canInteract = false; 
        }
    }

    public void setText(string text)
    {
        // set the text of a tmpUGUI to the text arguement of this method 
        interactText.text = text;
    }

    public void Update()
    {
        if (canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("Interacting with " + this.name);
                PlayerUI.instance.addItem(itemID); //adds to list to check if player has this item and sends it to yarnspinner
                //changePhase.Invoke("quest_completed"); now controlled by playerUI
                setText("");
                canInteract = false;
                collection();
                Debug.Log(itemID + "  : collected");
                PlayerUI.instance.updateInventoryUI(itemID);
            }
        }
    }

    public void collection()
    {
        GameController.instance.itemCollect(itemID);
        Destroy(this.gameObject);


        //invoke method in GameController (same as coincollect)
        //foreach(GameObject obj in objsToDestory){
        //    Destroy(obj);
        //}
        //SphereCollider col = GetComponent<SphereCollider>();
        //col.enabled = false;
    }

    */
}

