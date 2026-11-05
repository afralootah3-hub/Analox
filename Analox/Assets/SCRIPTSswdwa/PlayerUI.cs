using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;


public class PlayerUI : MonoBehaviour
{
    // changing this to evidence log stuff


    // a singleton that handles persistent ui updates across scenes
    // updates according to the story phase
    // updates according to what objects/items the player has (just the quest item and reward item)

    public static PlayerUI instance { get; set; }
    public TextMeshProUGUI uiText;
    public TextMeshProUGUI inventoryText;
    public string _inventoryText;
    public List<string> _inventoryItems; //stores everything in inventory
    public updateDialogPhase changePhase;

    //temporary debug vars
    public string itemNeeded; //this is the itemID of the obj needed

    public void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void pUiUpdate(string currentPhase)
    {
        //uses npcdata to determine what quest, and what phase
        if (currentPhase == "quest_started")
        {
            Debug.Log("Updating UI...");
            uiText.text = "Current Quest:  Pick some fruit!";
        }
        else if(currentPhase == "quest_completed")
        {
            Debug.Log("Updating UI...");
            uiText.text = "Pick some fruit complete!";
        }

        

    }



    public void updateInventoryUI(string item)
    {
        inventoryText.text += item + "  ";
        Debug.Log("Added " + item + " to inventorytext");

        if(item == "apples")
        {
            changePhase.Invoke("quest_completed");
        }
        else
        {
            Debug.Log("Added orangves");
        }
    }


    public void addItem(string item)
    {
        _inventoryItems.Add(item); //add item to inventory list
        itemNeeded = item;
        
    }

    public void removeItem(string item)
    {
        _inventoryItems.Remove(item);
        inventoryText.text = "";
        

    }

   

    public void DestoryAllCollected()
    {
        foreach(string item in _inventoryItems)
        {
            GameObject gObj = GameObject.Find(item);
            Destroy(gObj);
        }
    }

    [YarnFunction("HasItem")]
    public static bool checkForItem(string itemChecked)
    {
        //checks if player has oranges in 
        return PlayerUI.instance._inventoryItems.Contains(itemChecked);
    }



}

[System.Serializable]
public class updateDialogPhase : UnityEvent<string>
{

}
