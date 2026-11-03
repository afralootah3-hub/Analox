using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public enum iType { npc, item } //interaction type enum

public class NpcInteract : MonoBehaviour
{
    
    [Tooltip("Select the type of Interaction to gain access to necessary variables")]
    public iType type; //type of interaction


    [System.Serializable]
    public struct TextData
    {
        //ensure both start letters are capitalized
        public TextMeshProUGUI promptText;
        public string interactText;


    }

    [Header("Text Data Variables")]
    public TextData textdata;

    public bool canInteract;

    [System.Serializable]
    public struct DialogData
    {
        public DialogueRunner npcDialogue;
        public npcData storyData;


    }
    [Header("YarnSpinner & Story Data Variables")]
    public DialogData dialogdata;

    [System.Serializable]
    public struct CamData
    {

        public GameObject npcCamPrefab;
        public NpcCamRig npcCamRig;

    }
    [Header("NPC Camera Rig Variables")]
    public CamData camdata;


    private void OnEnable()
    {
        PlayerUI.instance.changePhase.AddListener(getStoryPhase);
    }

    private void OnDisable()
    {
        PlayerUI.instance.changePhase.RemoveListener(getStoryPhase);
    }


    public void Start()
    {
        //setting this variable looks a bit 'static' for our system design tastes
        //BUT we can practice a bit of deliberate design because we know we're
        //creating a systematized NPC rig & Interaction for multiple instances

        dialogdata.npcDialogue = FindFirstObjectByType<DialogueRunner>();

    }

    //---------EDITOR METHODS---------//

    public void loadText()
    {
        textdata.promptText = GameObject.FindGameObjectWithTag("pText").GetComponent<TextMeshProUGUI>();
    }

    public void loadNpcCamPrefab()
    {
        Instantiate(camdata.npcCamPrefab, transform);
        camdata.npcCamRig = transform.GetComponentInChildren<NpcCamRig>();
    }

    public void loadDialogRunner()
    {
        dialogdata.npcDialogue = GameObject.FindGameObjectWithTag("dRunner").GetComponent<DialogueRunner>();
    }


    //--------RUNTIME METHODS--------//



    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //prompt interaction pops up 
            setText(textdata.interactText);
            canInteract = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //reset everything
            setText("");
            canInteract = false;
        }
    }

    //using an internal setter class
    public void setText(string txt)
    {
        textdata.promptText.text = txt;
    }


    // Update is called once per frame
    void Update()
    {

        if (canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                dialogdata.npcDialogue = FindFirstObjectByType<DialogueRunner>();


                dialogdata.npcDialogue.StartDialogue(dialogdata.storyData.startingNode); //passing in the node (Case sensitive)
                //camdata.npcCamRig.movePlayer(); 
                Debug.Log("START NPC DIALOGUE");
                setText("");
                //load in dialogue system



                canInteract = false;

            }
        }

    }

    /// <summary>
    /// 'sets' the $dialogPhase variable in YarnSpinner script
    /// </summary>

    //used in 'loadPhase' node to route the YarnSpinner script its correct story path
    [YarnCommand("setPhase")]
    public void setStoryPhase()
    {
        InMemoryVariableStorage vStore = GameObject.FindAnyObjectByType<InMemoryVariableStorage>();
        vStore.SetValue("$dialogPhase", dialogdata.storyData.currentPhase.ToString());
    }

    /// <summary>
    /// 'gets' a string from YarnSpinner and 'converts' it to a dialogPhase enum
    /// </summary>
    /// <param name="phase"></param>

    //converts to the phase we're on in our npcData scritpableobj
    //also contains a nice error reporting in case a string is passed that does not
    //correspond to any dialogphase
    [YarnCommand("getPhase")]
    public void getStoryPhase(string phase)
    {
        PlayerUI.instance.pUiUpdate(phase); //send current phase to pui
        Debug.Log(phase);
        if (phase == "comeback")
        {
            dialogdata.storyData.currentPhase = npcData.dialogPhase.comeback;
        }
        else if (phase == "quest_started")
        {
            dialogdata.storyData.currentPhase = npcData.dialogPhase.quest_started;

        }
        else if (phase == "quest_completed")
        {
            dialogdata.storyData.currentPhase = npcData.dialogPhase.quest_completed;
            PlayerUI.instance.pUiUpdate("quest_completed");
        }
        else
        {
            Debug.LogError("The string you passed in the Yarn Script does not match any current dialogPhase in the npcData ScriptableObject");
        }


    }

    [YarnCommand("GetItem")]
    public void getItem(string itemName)
    {
        //gets the items you have checks if you have oranges for hidden dialog

    }

    [YarnCommand("SetItem")]
    public void setItemNeeded(string itemNeededName)
    {
        //sets the item you need to complete the quest
        PlayerUI.instance.itemNeeded = itemNeededName;
    }



    [YarnCommand("RemoveItem")]
    public void removeItem(string itemToRemove)
    {
        PlayerUI.instance.removeItem(itemToRemove);
    }    

    public void OnApplicationQuit()
    {
        dialogdata.storyData.dataReset();
    }

    
}
