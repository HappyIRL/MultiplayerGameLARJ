using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/*****************************************************************************
 * NOTES:
 * 
 *  This Script lets us create Quests from within the Project window.
 *  
 *  A Quest is made up of several objectives
 *  
 *  Objectives can be added, removed and moved up and down the Objective list.
 *  Objectives each carry a name, optional sidequest toggle, visibility toggle
 *  and Initial Status
 * 
******************************************************************************/

// Quest - Stores info of quest (name + objectives)

// Create Asset Menu to make Quest a creatable asset
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest", order = 100)]
    public class Quest : ScriptableObject
{
    // Status of Quest and objective
    public enum Status
    {
        NotYetComplete,

        Complete,

        Failed
    }

    // Name of quest
    public string questName;

    // List of objectives making up the quest
    public List<Objective> objectives;

    // Objectives/Tasks that make up quest
    [System.Serializable]
    public class Objective
    {
        // visible name shown to player
        public string name = "New Objective";

        // if true, side quest = optional quest
        public bool optional = false;

        // if false & !complete objective isnt shown
        public bool visible = true;

        public Status initialStatus = Status.NotYetComplete;
    }


}

#if UNITY_EDITOR
// Drawing custom Editor for building list of objectives
[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    // called when unity draws inspector for quest
    public override void OnInspectorGUI()
    {
        // ensures pending changes
        serializedObject.Update();

        // Draw name of quest
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("questName"),
            new GUIContent("Name")
            );

        // Header for list of Objectives
        EditorGUILayout.LabelField("Objectives");

        // Get Property containing list of objectives
        var objectiveList = serializedObject.FindProperty("objectives");

        // Indent the objectives
        EditorGUI.indentLevel += 1;

        // For each objective in list, draw an entry
        for (int i = 0; i < objectiveList.arraySize; i++)
        {
            // Draw single line of controls
            EditorGUILayout.BeginHorizontal();

            // Draw the objective itself (name and flags)
            EditorGUILayout.PropertyField(
                objectiveList.GetArrayElementAtIndex(i),
                includeChildren: true
                );

            // draw button that moves item up the list
            if (GUILayout.Button("Up", EditorStyles.miniButtonLeft,
                                    GUILayout.Width(25)))
            {
                objectiveList.MoveArrayElement(i, i - 1);
            }
            
            // draw button that moves item down in the list
            if(GUILayout.Button("Down", EditorStyles.miniButtonRight, 
                                    GUILayout.Width(40)))
            {
                objectiveList.DeleteArrayElementAtIndex(i);
            }

            // draw button that removes the item from the list
            if (GUILayout.Button("-", EditorStyles.miniButtonRight,
                                    GUILayout.Width(25)))
            {
                objectiveList.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();

        }

        // Remove indent
        EditorGUI.indentLevel -= 1;

        // Draw button that adds new objectives to list
        if (GUILayout.Button("Add Objective"))
        {
            objectiveList.arraySize += 1;
        }

        // save any changes
        serializedObject.ApplyModifiedProperties();
        
    }

}
#endif
