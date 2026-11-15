using UnityEngine;

public class MenusManager : MonoBehaviour
{
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

    public struct CorkBoardCaches
    {
        //these are used to temporarily 
        private ItemData _itemDataCache;
        private npcData _npcDataCache;
        private string _clueInfoStr;
        private string _clueScanStr;
        private string _clueTitleStr;
        
    }

    // corkboard var // 
    // restructured later to use tags and custom inspector for easy assinging in inspector//
    public GameObject corkBoardHolder;
    public RectTransform sideBarTransform;
    public RectTransform dragLayer; 
    // reference crockboard item script here



}
