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

    //

    public bool drawerIsOpen;



    private void Awake()
    {
        wasScanned = false;
    }

}
