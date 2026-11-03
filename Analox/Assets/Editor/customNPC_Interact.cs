//using UnityEngine;
//using UnityEditor;

////Below attribute for this class ensures that this editor script affects the NpcInteract class
//[CustomEditor(typeof(NpcInteract))]
//public class customNPC_Interact : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        //this built-in method 'draws' the inspector as it normally would be viewed
//        // 'draw' in this instance means to visually render in the editor 
//        //DrawDefaultInspector();

//        serializedObject.Update(); // this built-in method on this obj will continue to 'draw' the editor code within this OnInspectorGUI Method

//        SerializedProperty typeProperty = serializedObject.FindProperty("type");
//        EditorGUILayout.PropertyField(typeProperty);

//        iType currentType = (iType)typeProperty.enumValueIndex; //temporarily storing the enum type in the inspector

//        if(currentType == iType.npc)
//        {
//            NpcInteract npcI = (NpcInteract)target;


//            //display/draw the properties/variables associated with this interaction type
//            EditorGUILayout.PropertyField(serializedObject.FindProperty("textdata"));
//            EditorGUILayout.HelpBox("Warning: Make sure to write the Interact text String before entering play mode", MessageType.Warning);

//            if (GUILayout.Button("Load Prompt Text"))
//            {
//                npcI.loadText();
//            }


//            EditorGUILayout.PropertyField(serializedObject.FindProperty("camdata"));
//            if(GUILayout.Button("Instantiate NPC Camera Prefab"))
//            {
//                npcI.loadNpcCamPrefab();
//            }

//            EditorGUILayout.HelpBox("Manually load the NPC_CAM_Prefab before Instantiating in Scene", MessageType.Info);


//            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogdata"));
//            if(GUILayout.Button("Load in NPC Dialog Runner"))
//            {
//                npcI.loadDialogRunner();
//            }

//            // make sure we're going to load in all of the functionality of npcinteract
//            // to affect and add to the inspector, we need to gain accesss to the methods in the NpcInteract class
//            // below line allows us to access those methods using the temp variable of 'npcI'


//            //for an editor script if we're using an IF statement with GUILayout as the condition
//            // then the editor will GENERATE that button as opposed to check to see if the condition 
//        }

//        //---ESSENTIAL LINE OF CODE TO ENSURE WE CAN EDIT THE PROPERTIES/VARIABLES OF OUR CLASS---//
//        //-without this we cant add any data to our variables-//
//        serializedObject.ApplyModifiedProperties();
//    }

    

//}
