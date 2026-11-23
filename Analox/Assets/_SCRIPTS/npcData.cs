using UnityEngine;

[CreateAssetMenu(fileName = "npcData", menuName = "NPC Profile")]
public class npcData : ScriptableObject
{
    public string npcName;
    public Texture2D imageTexture;

    [TextArea(2, 15)]
    public string basicDesc, scanDesc, reportInfoDesc;
    // the check for whether we scanned this npc should be handled in YS script :)

    public string startingNode; 

    public enum dialogPhase { comeback, begin, quest_completed, quest_started ,reward_given }
    public dialogPhase currentPhase;
    public bool hasScannedNPC;

    public void dataReset()
    {
        currentPhase = dialogPhase.begin;
        hasScannedNPC = false;
    }


}
