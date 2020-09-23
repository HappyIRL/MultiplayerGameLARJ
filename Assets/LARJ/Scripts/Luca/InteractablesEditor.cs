using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(Interactables))]
public class InteractablesEditor : Editor
{
    //Events
    public SerializedProperty HoldingFinishedInteractionEventProperty = null;
    public SerializedProperty HoldingStartedInteractionEventProperty = null;
    public SerializedProperty HoldingFailedInteractionEventProperty = null;
    public SerializedProperty PressInteractionEventProperty = null;

    //Button hints
    public SerializedProperty KeyboardButtonHintProperty = null;   
    public SerializedProperty GamepadButtonHintProperty = null;

    private void OnEnable()
    {
        HoldingFinishedInteractionEventProperty = serializedObject.FindProperty("HoldingFinishedInteractionEvent");
        HoldingStartedInteractionEventProperty = serializedObject.FindProperty("HoldingStartedInteractionEvent");
        HoldingFailedInteractionEventProperty = serializedObject.FindProperty("HoldingFailedInteractionEvent");
        PressInteractionEventProperty = serializedObject.FindProperty("PressInteractionEvent");

        KeyboardButtonHintProperty = serializedObject.FindProperty("KeyboardButtonHintImage");
        GamepadButtonHintProperty = serializedObject.FindProperty("GamepadButtonHintImage");
    }

    public override void OnInspectorGUI()
    {
        Interactables interactables = target as Interactables;
        interactables.InteractionType = (InteractionType)EditorGUILayout.EnumPopup("Interaction Type", interactables.InteractionType);

        switch (interactables.InteractionType)
        {
            case InteractionType.PickUp:
                EditorGUILayout.PropertyField(KeyboardButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadButtonHintProperty);
                break;
            case InteractionType.Press:
                EditorGUILayout.PropertyField(KeyboardButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadButtonHintProperty);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(PressInteractionEventProperty);
                break;
            case InteractionType.Hold:
                interactables.HoldingTime = EditorGUILayout.FloatField("HoldingTime", interactables.HoldingTime);
                EditorGUILayout.PropertyField(KeyboardButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadButtonHintProperty);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(HoldingStartedInteractionEventProperty);
                EditorGUILayout.PropertyField(HoldingFinishedInteractionEventProperty);
                EditorGUILayout.PropertyField(HoldingFailedInteractionEventProperty);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}

#endif
