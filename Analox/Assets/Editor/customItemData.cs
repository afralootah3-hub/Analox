using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemData))]
public class customItemData : Editor
{

    public override void OnInspectorGUI()
    {
        //this built-in method 'draws' the inspector as it normally would be viewed
        // 'draw' in this instance means to visually render in the editor 
        //DrawDefaultInspector();
        serializedObject.Update();

        EditorGUILayout.HelpBox("This holds the DATA for any item you can INTERACT with (does not include doors/drawers)", MessageType.Info);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemName"));
        EditorGUILayout.HelpBox("Name that will be shown during inspection", MessageType.None);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemDescription"));
        EditorGUILayout.HelpBox("description that will be shown during inspection", MessageType.None);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ScanInformation"));
        EditorGUILayout.HelpBox("This is shown after the player SCANS the object (using up sanity)", MessageType.Info);


        // make sure we're going to load in all of the functionality of npcinteract
        // to affect and add to the inspector, we need to gain accesss to the methods in the NpcInteract class
        // below line allows us to access those methods using the temp variable of 'npcI'


        //for an editor script if we're using an IF statement with GUILayout as the condition
        // then the editor will GENERATE that button as opposed to check to see if the condition 

        serializedObject.ApplyModifiedProperties();
    
    }

    //---ESSENTIAL LINE OF CODE TO ENSURE WE CAN EDIT THE PROPERTIES/VARIABLES OF OUR CLASS---//
    //-without this we cant add any data to our variables-//

    
}


