using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CorkboardItem : MonoBehaviour , IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{

    public ItemData itemData;
    public npcData npcData;

    public TextMeshProUGUI clueTitle;

    // accessed through parent or child?
    public TextMeshProUGUI clueBaseDesc;
    public TextMeshProUGUI clueScanDesc;

    private RectTransform _boardRect; // what it drags around of 
    private RectTransform _rect;
    public string clueDescStr = "";
    public string clueScanStr = "";
    public Transform lastPos;
    private Canvas _canvas;

  

    //

    private MenusManager _manager = null;

    public void InitalizeObjectClue(ItemData IData)
    {
        itemData = IData;
        clueDescStr = IData.ItemDescription;
        clueScanStr = IData.ScanInformation;

        // add the add components and stuff here 
        this.AddComponent<RawImage>();
        RawImage _img = GetComponent<RawImage>();
        
        _img.texture = IData.imageTexture;
        _rect = GetComponent<RectTransform>();
        _boardRect = MenusManager.instance.corkBoardHolder.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

    }

    public void Update()
    {
       
    }


    public void InitalizeNPCClue(npcData NData)
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

        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    
    }

    public void OnEndDrag(PointerEventData eventData)
    { 
        this.GetComponent<RawImage>().raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HOVERING?");
        MenusManager.instance.itemDescTMP.text += itemData.ItemDescription;
        MenusManager.instance.itemDescTransform.SetParent(this.transform, false);
        if(itemData.wasScanned == true)
        {
            MenusManager.instance.scanDescTMP.text += itemData.ScanInformation;
            MenusManager.instance.scanDescTransform.SetParent(this.transform, false);
        }
        
    }

   

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exit fired");
        MenusManager.instance.itemDescTMP.text = "";
        MenusManager.instance.scanDescTMP.text = "";
        MenusManager.instance.itemDescTransform = MenusManager.instance.itemDescStartPos;
        MenusManager.instance.scanDescTransform = MenusManager.instance.scanDescStartPos;
    }

   

}
