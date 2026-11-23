using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Yarn.Unity;

public enum iType { npc, token, stormsOut } //interaction type enum (unsued)

public class NpcInteract : MonoBehaviour
{
    
    [Tooltip("Select the type of Interaction to gain access to necessary variables")]
    public iType type; //type of interaction

    // vision/stealth stuff 
    public Transform eyes;
    public float viewRange = 10f;

    private bool _hasScannedNPC;



    #region private refs
    private Transform _playerT;
    private bool _spotting;
    private ActionWheel _wheel;
    private PlayerController _player;
    private UiManager _ui;
    private Collider _triggerCol;

    private bool _inRangeOfNPC;

    #endregion

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

        public npcStateCheck npcstatecheck;

    }
    [Header("YarnSpinner & Story Data Variables")]
    public DialogData dialogdata;
    private string startingNode;

    //[System.Serializable] old stuff
    //public struct CamData
    //{

    //    public GameObject npcCamPrefab;
    //    public NpcCamRig npcCamRig;

    //}
    //[Header("NPC Camera Rig Variables")]
    //public CamData camdata; old stuff


    private void OnEnable()
    {
        if (dialogdata.npcDialogue == null) dialogdata.npcDialogue = FindFirstObjectByType<DialogueRunner>();
        //PlayerUI.instance.changePhase.AddListener(getStoryPhase);
        dialogdata.npcDialogue.onDialogueComplete.AddListener(OnDialogEnd);
    }

    private void OnDisable()
    {
        //PlayerUI.instance.changePhase.RemoveListener(getStoryPhase);
        dialogdata.npcDialogue.onDialogueComplete.RemoveListener(OnDialogEnd);
    }

    void OnDialogEnd()
    {
        Debug.Log("Dialog from " + this.name + "has ended");
        if (_inRangeOfNPC) canInteract = true;

    }


    public void Start()
    {
        //setting this variable looks a bit 'static' for our system design tastes
        //BUT we can practice a bit of deliberate design because we know we're
        //creating a systematized NPC rig & Interaction for multiple instances

        dialogdata.npcDialogue = FindFirstObjectByType<DialogueRunner>();
        _playerT = GameObject.FindGameObjectWithTag("Player").transform;
        _player = _playerT.GetComponent<PlayerController>();
        _ui = GameController.instance.GetComponent<UiManager>();
        _wheel = FindFirstObjectByType<ActionWheel>(FindObjectsInactive.Include);
        _triggerCol = GetComponent<SphereCollider>();

       

    }

    //---------EDITOR METHODS---------//

    public void loadText()
    {
        textdata.promptText = GameObject.FindGameObjectWithTag("pText").GetComponent<TextMeshProUGUI>();
    }

    //public void loadNpcCamPrefab()
    //{
    //    Instantiate(camdata.npcCamPrefab, transform);
    //    camdata.npcCamRig = transform.GetComponentInChildren<NpcCamRig>();
    //}

    public void loadDialogRunner()
    {
        dialogdata.npcDialogue = GameObject.FindGameObjectWithTag("dRunner").GetComponent<DialogueRunner>();
    }


    //--------RUNTIME METHODS--------//

    public void HasScannedNPC()
    {
        // runs when player chooses the index aligning with scan from action wheel 
        if (_hasScannedNPC)
        {
            // dont waste sanity
            //call uimanager thingy
        }
        else
        {
            _hasScannedNPC = true;
        }
        

    }




    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //prompt interaction pops up 
            setText(textdata.interactText);
            canInteract = true;
            Debug.Log(canInteract);
            _inRangeOfNPC = true;
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
            Debug.Log("ran first");
            if(_wheel.isOpen == false)
            {
                Debug.Log("ran second");
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Debug.Log("pressed, canInteract = " + canInteract);

                    if (mouseIsOverNPC() && !_ui.isInspectingNPC)
                    {
                        Debug.Log("mouse is over npc");
                        OpenWheel();
                    }
                }
            }
        }


        bool seen = CanSeePlayer();
        if (seen && !_spotting)
        {
            _spotting = true;
            Debug.Log("ENTERED range");
            PillManager.instance.UpdateStealth(-1);
        }
        else if (!seen && _spotting)
        {
            _spotting = false;
            Debug.Log("LEFT range");
            PillManager.instance.UpdateStealth(1);
        }


    }

    private void StartDialogue()
    {
        _player.setPlayerState(PlayerController.playerState.isDisabled);
        dialogdata.npcDialogue = FindFirstObjectByType<DialogueRunner>();
        Debug.Log("START NPC DIALOGUE");
        setText("");

        //_ui.isInspectingNPC = false;

        dayState currentState = GameController.instance.currentDay;

        string startingNodeName = dialogdata.npcstatecheck.checkState(currentState);
        string startingNode = dialogdata.storyData.startingNode;

        dialogdata.npcDialogue.StartDialogue(startingNode + startingNodeName); //start dialog depending on what day it is
        Debug.Log(startingNode + startingNodeName);
        // note: if ys script is too long or should be split up, it might cause some issues? with remembering past conversations

        //dialogdata.npcDialogue.StartDialogue(dialogdata.storyData.startingNode); //passing in the node (Case sensitive)
        //camdata.npcCamRig.movePlayer(); 


        //load in dialogue system



        canInteract = false;
    }

    #region action wheel stuff
    private bool mouseIsOverNPC()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        int layerMask = LayerMask.GetMask("iObject");

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Collider hitobj = hit.collider.GetComponentInParent<SphereCollider>();
            Debug.Log("ray hit " + hit.collider.name + ", its InteractableObj: " + hitobj + ", interactable of this: " + _triggerCol);


            if (hitobj == _triggerCol)
            {
                return true;
            }
            else return false;
        }
        else
        {
            Debug.Log("ray hit nothing on the iObject layer");

            return false;
        }
    }
    private void OpenWheel()
    {
        _player.setPlayerState(PlayerController.playerState.isDisabled);

        Vector2 mousePos = Mouse.current.position.ReadValue();
        _wheel.OpenWheel(mousePos, OnWheelChoice);
    }

    /// <summary>
    /// Handles what happens when selecting each option in the action wheel
    /// </summary>
    /// <param name="index"></param>
    private void OnWheelChoice(int index)
    {
        

        if(index == 0) // first in list
        {
            StartDialogue();
            _ui.isInspectingNPC = false;
            
        }
        else if (index == 1)
        {
            Debug.Log(index);
            _ui.OnViewNPCInfo(dialogdata.storyData);
            return;
        }
        else if(index == 2)
        {
            Debug.Log(index);
            _ui.OnScannedNPC(dialogdata.storyData);
            return;
        }
        
        else if (index == -1) // out of list bounds -> exit interaction
        {
            _player.setPlayerState(PlayerController.playerState.isIdle);
            Debug.Log(index);
            return;
        }
    }

    private bool CanSeePlayer()
    {
        return Vector3.Distance(eyes.position, _playerT.position) <= viewRange;

    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);
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

    //          YarnCommands for NPC and player Sprite Changes      //
    //written later



    public void OnApplicationQuit()
    {
        dialogdata.storyData.dataReset();
    }
   


}
