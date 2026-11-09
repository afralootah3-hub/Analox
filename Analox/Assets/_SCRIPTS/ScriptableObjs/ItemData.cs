using Unity.VisualScripting;
using UnityEngine;

public enum itemType { clueObj, Door, Drawer} //interaction type enum

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    //      HANDLES INSPECTABLE OBJS, DOORS, DRAWERS        //

    public itemType objtype;
    public string ItemName;
    public string ItemDescription;
    public string ScanInformation;
    public bool wasScanned = false; // READ ONLY //


    //
    public Transform doorPivot;
    public bool isOpen;
    public bool isOpeningDoor;

    public Quaternion closedRot; //or starting rotation
    //

    public bool drawerIsOpen;
    public bool isOpeningDrawer;
    public Transform drawerPivot;
    public Collider insideObjCol;


    private void Awake()
    {
        wasScanned = false;
        isOpeningDrawer = false;
        drawerIsOpen = false;
        isOpen = false; 
       
    }

}
