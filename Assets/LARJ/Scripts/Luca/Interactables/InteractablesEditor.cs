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
    public SerializedProperty MousePressInteractionEventProperty = null;
    public SerializedProperty MouseReleaseInteractionEventProperty = null;

    //Button hints
    public SerializedProperty KeyboardButtonHintProperty = null;   
    public SerializedProperty GamepadButtonHintProperty = null;
    public SerializedProperty KeyboardPickedUpButtonHintProperty = null;
    public SerializedProperty GamepadPickedUpButtonHintProperty = null;

    
    private void OnEnable()
    {
        HoldingFinishedInteractionEventProperty = serializedObject.FindProperty("HoldingFinishedInteractionEvent");
        HoldingStartedInteractionEventProperty = serializedObject.FindProperty("HoldingStartedInteractionEvent");
        HoldingFailedInteractionEventProperty = serializedObject.FindProperty("HoldingFailedInteractionEvent");
        PressInteractionEventProperty = serializedObject.FindProperty("PressInteractionEvent");
        MousePressInteractionEventProperty = serializedObject.FindProperty("MousePressInteractionEvent");
        MouseReleaseInteractionEventProperty = serializedObject.FindProperty("MouseReleaseInteractionEvent");

        KeyboardButtonHintProperty = serializedObject.FindProperty("KeyboardButtonHintImage");
        GamepadButtonHintProperty = serializedObject.FindProperty("GamepadButtonHintImage");
        KeyboardPickedUpButtonHintProperty = serializedObject.FindProperty("MousePickedUpInteractionButtonHintImage");
        GamepadPickedUpButtonHintProperty = serializedObject.FindProperty("GamepadPickedUpInteractionButtonHintImage");
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
                interactables.CanInteractWhenPickedUp = EditorGUILayout.Toggle("CanInteractWhenPickedUp", interactables.CanInteractWhenPickedUp);
                if (interactables.CanInteractWhenPickedUp)
                {
                    EditorGUILayout.PropertyField(KeyboardPickedUpButtonHintProperty);
                    EditorGUILayout.PropertyField(GamepadPickedUpButtonHintProperty);
                    EditorGUILayout.PropertyField(MousePressInteractionEventProperty);
                    EditorGUILayout.PropertyField(MouseReleaseInteractionEventProperty);
                }
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
