using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(Interactable), true)]
public class InteractablesEditor : Editor
{
    //Button hints
    public SerializedProperty KeyboardButtonHintProperty = null;
    public SerializedProperty GamepadButtonHintProperty = null;
    public SerializedProperty KeyboardPickedUpButtonHintProperty = null;
    public SerializedProperty GamepadPickedUpButtonHintProperty = null;


    private void OnEnable()
    {
        KeyboardButtonHintProperty = serializedObject.FindProperty("KeyboardButtonHintImage");
        GamepadButtonHintProperty = serializedObject.FindProperty("GamepadButtonHintImage");
        KeyboardPickedUpButtonHintProperty = serializedObject.FindProperty("MousePickedUpInteractionButtonHintImage");
        GamepadPickedUpButtonHintProperty = serializedObject.FindProperty("GamepadPickedUpInteractionButtonHintImage");
    }

    public override void OnInspectorGUI()
    {     
        EditorGUILayout.LabelField("Interactable", EditorStyles.boldLabel);

        Interactable interactables = target as Interactable;
        interactables.InteractionType = (InteractionType)EditorGUILayout.EnumPopup("Interaction Type", interactables.InteractionType);

        switch (interactables.InteractionType)
        {
            case InteractionType.PickUp:
                EditorGUILayout.PropertyField(KeyboardButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadButtonHintProperty);
                interactables.CanInteractWhenPickedUp = EditorGUILayout.Toggle("CanInteractWhenPickedUp", interactables.CanInteractWhenPickedUp);
                if (interactables.CanInteractWhenPickedUp)
                {
                    EditorGUILayout.PropertyField(KeyboardPickedUpButtonHintProperty);
                    EditorGUILayout.PropertyField(GamepadPickedUpButtonHintProperty);
                }
                break;
            case InteractionType.Press:
                EditorGUILayout.PropertyField(KeyboardButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadButtonHintProperty);
                break;
            case InteractionType.Hold:
                interactables.HoldingTime = EditorGUILayout.FloatField("HoldingTime", interactables.HoldingTime);
                EditorGUILayout.PropertyField(KeyboardButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadButtonHintProperty);
                break;
            case InteractionType.MultiPress:
                interactables.PressCountToFinishTask = EditorGUILayout.IntField("PressCountToFinishTask", interactables.PressCountToFinishTask);
                EditorGUILayout.PropertyField(KeyboardButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadButtonHintProperty);
                break;
        }

        DrawPropertiesExcluding(serializedObject, "m_Script");

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
