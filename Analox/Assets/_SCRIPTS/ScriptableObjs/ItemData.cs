using UnityEngine;


[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    
    public string ItemName;
    public string ItemDescription;
    public string ScanInformation;
    public bool wasScanned = false; // READ ONLY //

    private void Awake()
    {
        wasScanned = false;
    }

}
