using UnityEngine;

public class InteractableObj : MonoBehaviour
{
    

    public ItemData itemdata;

    [Header("READ ONLY - AUTOMATICALLY ASSIGNED AT RUNTIME")]
    public Renderer itemRenderer;

    private void Start()
    {
        itemRenderer = GetComponentInChildren<MeshRenderer>();
        itemdata.wasScanned = false;
    }

}
