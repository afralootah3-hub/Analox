using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class CorkboardItem : MonoBehaviour , IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{

    public ItemData itemData;
    public npcData npcdata;

    public TextMeshProUGUI clueTitle;

    // accessed through parent or child?
    public TextMeshProUGUI clueDesc;
  

    

    private RectTransform _boardRect; // what it drags around of 
    private RectTransform _rect;
    public string clueDescStr = "";
    public string clueScanStr = "";
    public Transform lastPos;
    private Canvas _canvas;


   


    //


    public void InitalizeObjectClue(ItemData IData)
    {
        itemData = IData;
        clueDescStr = IData.ItemDescription;
        clueScanStr = IData.ScanInformation;
        if (!MenusManager.instance.allCorkItems.Contains(this))
        {
            MenusManager.instance.allCorkItems.Add(this);

        }

        // add the add components and stuff here 
        this.AddComponent<RawImage>();
        RawImage _img = GetComponent<RawImage>();
        
        _img.texture = IData.imageTexture;
        _rect = GetComponent<RectTransform>();
        _boardRect = MenusManager.instance.corkBoardHolder.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

    }



    public void InitalizeNpcClue(npcData NData)
    {
        npcdata = NData;

        if (NData.hasScannedNPC)
        {
            clueDescStr = NData.scanDesc;
        }
        else
        {
            clueDescStr = NData.basicDesc;
        }

        //clueDescStr = NData.basicDesc;
        //clueScanStr = NData.scanDesc;

        RawImage _img = this.AddComponent<RawImage>();
        _img.texture = NData.imageTexture;
        _rect = GetComponent<RectTransform>();
        _boardRect = MenusManager.instance.corkBoardHolder.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

    }

    public void Update()
    {
       
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        

        Vector3 worldPos = transform.position;
        if (_canvas == null) _canvas = GetComponentInParent<Canvas>();

        lastPos = transform.parent;
        transform.SetParent(MenusManager.instance.corkBoardHolder.transform, true);
        transform.SetAsLastSibling();

        transform.position = worldPos;

        //this.GetComponent<RawImage>().raycastTarget = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if(RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //    _boardRect, eventData.position, Camera.main, out Vector2 localPoint))
        //{
        //    _rect.localPosition = localPoint;
        //}
        Debug.Log(_rect + "Rect" + _canvas + "canvas");
        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        
    }

    public void OnEndDrag(PointerEventData eventData)
    { 
        this.GetComponent<RawImage>().raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HOVERING?");

        if(itemData != null)
        {
            
            if (itemData.wasScanned == true)
            {
                MenusManager.instance.itemDescTMP.text = itemData.ScanInformation;
                //MenusManager.instance.scanDescTransform.SetParent(this.transform, false);
            }
            else
            {
                MenusManager.instance.itemDescTMP.text = itemData.ItemDescription;

            }
        }
        else if (itemData == null && npcdata != null)
        {
           
            if (npcdata.hasScannedNPC == true)
            {
                MenusManager.instance.itemDescTMP.text = npcdata.scanDesc;
                //MenusManager.instance.scanDescTransform.SetParent(this.transform, false);
            }
            else
            {
                MenusManager.instance.itemDescTMP.text = npcdata.basicDesc;
                //MenusManager.instance.itemDescTransform.SetParent(this.transform, false);
            }
        }


        
    }

   

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exit fired");
        MenusManager.instance.itemDescTMP.text = "";
        
    }

   

}
