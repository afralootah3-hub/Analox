using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(npcData))]
public class customNpcData : Editor
{
    public override void OnInspectorGUI()
    {
        //this built-in method 'draws' the inspector as it normally would be viewed
        // 'draw' in this instance means to visually render in the editor 
        //DrawDefaultInspector();
        serializedObject.Update();


        EditorGUILayout.PropertyField(serializedObject.FindProperty("npcName"));
        EditorGUILayout.HelpBox("Name that will be shown during Dialog", MessageType.Info);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("startingNode"));
        EditorGUILayout.HelpBox("When writing dialog in YS script, the first node for each day must be titled \n" +
            "startingNode + startDay1 or whichever day the dialog is"
            +"  \n OTHERWISE DIALOG WON'T RUN AS INTENDED", MessageType.Warning);

        serializedObject.ApplyModifiedProperties();

    }

    

}
