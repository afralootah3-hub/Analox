using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Unity;

public class MenusManager : MonoBehaviour
{
    public static MenusManager instance { get; set; }


    // manages:
    // corkboard 
    // any menus 

    /* corkboard system]
     1. PlayerUI sends the inventory list here on OpenMenu/Added item/information
     2. this accesses the scan info, image, and anythign else 
    3. player hover over image -> reveal info
    4. player drags image around
    5. player can draw red lines conecting evidence (WHICH SHOULD PERSIST ACROSS SCENES) -> use logic from mail game / last year midterm (render on diff 
    */

   
    // corkboard var // 
    // restructured later to use tags and custom inspector for easy assinging in inspector//
    public GameObject corkBoardHolder;
    public GameObject itemsHolder;
    public RectTransform boardArea;
    public Transform boardItemsParent;
    public RectTransform dragLayer; 
    private List<string> _clueItems; //stores everything in inventory

  
    private bool _corkMenuOpen;

    public bool _canDraw;

    private PlayerController _playercc;
    public Camera drawingCam;
    public GameObject drawingHolder;
    public Toggle paintToggle;

    public RectTransform itemDescTransform;
    public TextMeshProUGUI itemDescTMP;
    
   
    public RectTransform itemDescStartPos, scanDescStartPos;

    // these are STORING the data recieved form ui manager and should not be assigned in inspector
    public List<ItemData> allClueItems;
    public List<npcData> allNpcData;
    public List<CorkboardItem> allCorkItems;

    private void Awake()
    {
        if (instance != null && instance != this)
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

    /// <summary>
    /// disable the raycasts of all corkboard items when drawing
    /// </summary>
    public void ToggleAllItemRaycasts(bool isOn)
    {
        if(allCorkItems.Count == 0)
        {
            Debug.Log("CONTAINS NOTHING IN CORKITEMS");
        }

        foreach(CorkboardItem cItem in allCorkItems)
        {
            if(cItem != null && cItem.TryGetComponent(out RawImage rawImg))
            {
                if (!isOn) rawImg.raycastTarget = false; else rawImg.raycastTarget = true;
            }
        }
    }




    private void Start()
    {
        corkBoardHolder.SetActive(false);
        _corkMenuOpen = false;
        drawingCam.enabled = false;
        _canDraw = false;
        _playercc = FindFirstObjectByType<PlayerController>();
       // drawingHolder.SetActive(false);
    }

    // placeholder, using id as display name (will change to smth else -> id is messy and holds _scan)
    public string GetDisplayName(string itemID)
    {
        return itemID;
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleMenu();
            //ToggleDrawing();
            
        }
    }

    public void ToggleMenu()
    {
        if (_corkMenuOpen) CloseCork(); else OpenCork();
    }

    public void ToggleDrawing()
    {
        if (_canDraw)
        {
            drawingHolder.SetActive(false);
            _canDraw = false;
            Debug.Log(_canDraw);
            //PillManager.instance._mainCamera = drawingCam;
        }
        else
        {
            drawingHolder.SetActive(true);
            _canDraw = true;
            //PillManager.instance._mainCamera = Camera.main;
        }
    }


    // spawn an item on the side bar for every item added since the last time this menu was opened

    public void OpenCork()
    {
        corkBoardHolder.SetActive(true);
        drawingCam.enabled = true;
        _corkMenuOpen = true;
        _playercc.setPlayerState(PlayerController.playerState.isDisabled);

         

        

    }

    public void CloseCork()
    {
        _playercc.setPlayerState(PlayerController.playerState.isIdle);
        _corkMenuOpen = false;
        drawingCam.enabled = false;
        corkBoardHolder.SetActive(false);
        paintToggle.isOn = true;
    }

    public void AddObjectClue(ItemData iData)
    {
        if (allClueItems.Contains(iData)) return; // no duplicates
        allClueItems.Add(iData);

        GameObject newClue = new GameObject("Clue" + iData.ItemName.ToString(), typeof(RectTransform)); //creates the holder
        newClue.AddComponent<CorkboardItem>();
        
        CorkboardItem cbItem = newClue.GetComponent<CorkboardItem>();

        
        newClue.transform.SetParent(itemsHolder.transform, false); // set as child for now 
        cbItem.InitalizeObjectClue(iData); // this adds any missing things and sets up image and text stuff according to the iData
        // ^ this handles the drag and drop stuff now 

        //allCorkItems.Add(cbItem);
    }

    [YarnCommand("AddNPC")]
    public static void AddNpcClue(GameObject npcObj)
    {
        npcData nData = npcObj.GetComponent<NpcInteract>().dialogdata.storyData;


        if (MenusManager.instance.allNpcData.Contains(nData)) return; // no duplicates 
        MenusManager.instance.allNpcData.Add(nData);

        GameObject newClue = new GameObject("NPCClue" + nData.npcName.ToString(), typeof(RectTransform)); //creates the holder
        newClue.AddComponent<CorkboardItem>();
        CorkboardItem cbItem = newClue.GetComponent<CorkboardItem>();
        newClue.transform.SetParent(MenusManager.instance.itemsHolder.transform, false); // set as child for now

        cbItem.InitalizeNpcClue(nData); // this adds any missing things and sets up image and text stuff according to the iData
        // ^ this handles the drag and drop stuff now 


    }









}
