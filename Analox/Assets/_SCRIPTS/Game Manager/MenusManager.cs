using System.Collections;
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

    private Coroutine notifRoutine;
    // corkboard var // 
    // restructured later to use tags and custom inspector for easy assinging in inspector//
    public GameObject corkBoardHolder;
    public GameObject itemsHolder;
    public RectTransform boardArea;
    public Transform boardItemsParent;
    public RectTransform dragLayer; 
    private List<string> _clueItems; //stores everything in inventory

    private TextMeshProUGUI notifTMP;
    private CanvasGroup notifCanvasGrp;
    private bool _corkMenuOpen;

    public bool _canDraw;
    public float notificationDuration = 6f;

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

       
        notifTMP = GameObject.FindGameObjectWithTag("notif").GetComponent<TextMeshProUGUI>();
        notifCanvasGrp = GameObject.FindGameObjectWithTag("notifGrp").GetComponent<CanvasGroup>();

        if(notifCanvasGrp != null)
        notifCanvasGrp.alpha = 0;

        if (drawingCam == null) drawingCam = GameObject.FindGameObjectWithTag("drawCam").GetComponent<Camera>();

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
            if (!_corkMenuOpen && PlayerController.instance.currentState == PlayerController.playerState.isDisabled)
            {
                //if menu isn't open but we're trying to open don't
                Debug.LogWarning("ATTEMPTING TO OPEN WHILE PLAYER IS DISABLED");
                return;
                
            }

            ToggleMenu();
            //ToggleDrawing();

            

            if (_playercc == null) _playercc = FindFirstObjectByType<PlayerController>();
        }

        if(notifCanvasGrp == null || notifTMP == null)
        {
            notifTMP = GameObject.FindGameObjectWithTag("notif").GetComponent<TextMeshProUGUI>();
            //notifCanvasGrp.alpha = 0;
            notifCanvasGrp = GameObject.FindGameObjectWithTag("notifGrp").GetComponent<CanvasGroup>();
            return;
        }

    }

    public void ToggleMenu()
    {
        if (_corkMenuOpen)
        {
            PlayerController.instance.setPlayerState(PlayerController.playerState.isIdle);
            CloseCork();
        }
        else
        {
            OpenCork();
            PlayerController.instance.setPlayerState(PlayerController.playerState.isDisabled);
            
        }
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
        if (drawingCam == null) drawingCam = GameObject.FindGameObjectWithTag("drawCam").GetComponent<Camera>();
        drawingCam.enabled = true;
        _corkMenuOpen = true;
        _playercc.setPlayerState(PlayerController.playerState.isDisabled);
        PillManager.instance.pillButton.SetActive(false);
         

        

    }

    public void CloseCork()
    {
        PillManager.instance.pillButton.SetActive(true);
        if (_playercc == null) _playercc = FindFirstObjectByType<PlayerController>();
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

    [YarnCommand("Notify")]
    public static void ShowNotification(string notification)
    {
        if(MenusManager.instance.notifTMP == null || MenusManager.instance.notifCanvasGrp == null)
        {
            MenusManager.instance.notifTMP = GameObject.FindGameObjectWithTag("notif").GetComponent<TextMeshProUGUI>();
            MenusManager.instance.notifCanvasGrp = GameObject.FindGameObjectWithTag("notifGrp").GetComponent<CanvasGroup>();
        }

        MenusManager.instance.notifTMP.text = notification;

        MenusManager.instance.StartNotifRoutine();


    }

    public void StartNotifRoutine()
    {
        if(notifRoutine != null)
        {
            StopCoroutine(notifRoutine);
        }

        notifRoutine = StartCoroutine(ShowNotification());

    }


    public IEnumerator ShowNotification()
    {
        notifCanvasGrp.alpha = 1;
        float elapsed = 0f;
        float duration = notificationDuration;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            notifCanvasGrp.alpha = Mathf.Lerp(1, 0, elapsed);
            yield return null;
        }

        notifCanvasGrp.alpha = 0;
        notifRoutine = null;


    }








}
