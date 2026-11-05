using Unity.VisualScripting;
using UnityEngine;

public class InteractableObj : MonoBehaviour
{
    

    public ItemData itemdata;

    [Header("READ ONLY - AUTOMATICALLY ASSIGNED AT RUNTIME")]
    public Renderer itemRenderer;
  
    

    private void Awake()
    {
        itemRenderer = GetComponentInChildren<MeshRenderer>();
        itemdata.wasScanned = false;
        if(itemdata.objtype == itemType.Door)
        {
            itemdata.doorPivot = this.transform;
            itemdata.closedRot = this.transform.localRotation; // cache starting rotation
            itemdata.isOpeningDoor = false;
            itemdata.isOpen = false;
        }
    }

}
