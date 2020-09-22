using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

/*****************************************************************************
* Project: FPS Puzzler
* File   : ObjectiveTrigger.cs
* Date   : 29.01.2020
* Author : Alexander Michelfelder (AM)
*
* History:
*	29.01.2020 Created - AM
*	
******************************************************************************/

// Combines a quest, an objective in that quest, and an objective status to use
[System.Serializable]
public class ObjectiveTrigger
{
    public Quest quest;
    // The status we want to apply
    public Quest.Status statusToApply;

    // location of this objective in the quest objective list
    public int objectiveNumber;

    public void Invoke()
    {
        var manager = Object.FindObjectOfType<QuestManager>();
        
        manager.UpdateObjectiveStatus(quest, objectiveNumber, statusToApply);
    }
}

#if UNITY_EDITOR
// Custom property drawers override how a type of property appears in the Inspector
[CustomPropertyDrawer(typeof(ObjectiveTrigger))]
public class ObjectiveTriggerDrawer : PropertyDrawer
{
    // Called when Unity needs to draw an ObjectiveTrigger property in the Inspector
    public override void OnGUI( Rect position, 
                                SerializedProperty property, 
                                GUIContent label)

    {
        // Wrap this in Begin/EndProperty to ensure that undo works on the entire 
        // ObjectiveTrigger property.
        EditorGUI.BeginProperty(position, label, property);

        // Reference the three properties in ObjectiveTrigger
        var questProperty = property.FindPropertyRelative("quest");
        var statusProperty = property.FindPropertyRelative("statusToApply");
        var objectiveNumberProperty = property.FindPropertyRelative("objectiveNumber");

        /* We want to display three controls:
         * - An Object field for dropping a quest object into
         * - a popup field for selecting desired quest status
         * - a popup field for selecting the specific objective;
         * 
         */

        // Calculate the rectangles in which we're displaying
        var lineSpacing = 2;

        // Calculate the rectangle for the first line
        var firstLinePosition = position;

        firstLinePosition.height = base.GetPropertyHeight(questProperty, label);

        // Second line same as first, but shifter one line down
        var secondLinePosition = position;
        secondLinePosition.y = firstLinePosition.y +
            firstLinePosition.height + lineSpacing;
        secondLinePosition.height =
            base.GetPropertyHeight(statusProperty, label);

        // ditto for third
        var thirdLinePosition = position;
        thirdLinePosition.y = secondLinePosition.y + secondLinePosition.height + lineSpacing;
        thirdLinePosition.height = base.GetPropertyHeight(objectiveNumberProperty, label);


        // Draw quest and status properties, using the automatic property fields
        EditorGUI.PropertyField(firstLinePosition, questProperty,
                            new GUIContent("Quest"));
        EditorGUI.PropertyField(secondLinePosition, statusProperty,
                            new GUIContent("Status"));
        // Now we draw custom property for the object.
        // Draw a lable on the left-hand side, and get a new rectangle
        // to draw the pop up in
        thirdLinePosition = EditorGUI.PrefixLabel(thirdLinePosition,
                            new GUIContent("Objective"));

        // draw the UI for choosing a property
        var quest = questProperty.objectReferenceValue as Quest;

        // only draw this if we have a quest and it has objectives
        if(quest != null && quest.objectives.Count > 0)
        {
            // Get name of every objective, as an array
            var objectiveNames = quest.objectives.Select(o => o.name).ToArray();

            // get the index of selected objetctive
            var selectedObjective = objectiveNumberProperty.intValue;

            // if we are referring to an object which is not in the list,
            // reset it to first objective
            if(selectedObjective >= quest.objectives.Count)
            {
                selectedObjective = 0;
            }

            // Draw the pop up, and get back the new selection
            var newSelectedObjective = EditorGUI.Popup(thirdLinePosition, selectedObjective, objectiveNames);

            // if it was different, store it in the property
            if (newSelectedObjective != selectedObjective)
            {
                objectiveNumberProperty.intValue = newSelectedObjective;
            }
        }
        else
        {
            // draw a disabled pop up as a visual placeholder
            using (new EditorGUI.DisabledGroupScope(true))
            {
                // show a pop up with a single entry: the string "-".
                // ignore its return value since its not interactive
                EditorGUI.Popup(thirdLinePosition, 0, new[] { "-" });
            }
        }
        EditorGUI.EndProperty();
    }

    // Called by Unity to figure out height of this property
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // number of lines in this property
        var lineCount = 3;
        // number of pixels in between each line
        var lineSpacing = 2;
        // height of each line
        var lineHeight = base.GetPropertyHeight(property, label);

        // the height of this property is the number of lines time the
        // height of each line, plus the spacing in between each line
        return (lineHeight * lineCount) + (lineSpacing * (lineCount - 1));
    }
}
#endif
