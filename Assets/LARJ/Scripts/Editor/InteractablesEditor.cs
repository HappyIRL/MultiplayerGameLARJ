using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(Interactable), true)]
public class InteractablesEditor : Editor
{
    //Button hints
    public SerializedProperty KeyboardPressedButtonHintProperty = null;
    public SerializedProperty KeyboardReleaseButtonHintProperty = null;
    public SerializedProperty GamepadPressedButtonHintProperty = null;
    public SerializedProperty GamepadReleaseButtonHintProperty = null;
    public SerializedProperty HoldingHintProperty = null;
    public SerializedProperty KeyboardPickedUpButtonHintProperty = null;
    public SerializedProperty GamepadPickedUpButtonHintProperty = null;


    private void OnEnable()
    {
        KeyboardPressedButtonHintProperty = serializedObject.FindProperty("KeyboardPressedButtonHintImage");
        KeyboardReleaseButtonHintProperty = serializedObject.FindProperty("KeyboardReleaseButtonHintImage");
        GamepadPressedButtonHintProperty = serializedObject.FindProperty("GamepadPressedButtonHintImage");
        GamepadReleaseButtonHintProperty = serializedObject.FindProperty("GamepadReleaseButtonHintImage");
        HoldingHintProperty = serializedObject.FindProperty("HoldingHintImage");
        KeyboardPickedUpButtonHintProperty = serializedObject.FindProperty("MousePickedUpInteractionButtonHintImage");
        GamepadPickedUpButtonHintProperty = serializedObject.FindProperty("GamepadPickedUpInteractionButtonHintImage");
    }

    public override void OnInspectorGUI()
    {     
        EditorGUILayout.LabelField("Interactable", EditorStyles.boldLabel);

        Interactable interactables = target as Interactable;
        interactables.InteractionType = (InteractionType)EditorGUILayout.EnumPopup("Interaction Type", interactables.InteractionType);

        EditorGUI.BeginChangeCheck();

        switch (interactables.InteractionType)
        {
            case InteractionType.PickUp:
                EditorGUILayout.PropertyField(KeyboardPressedButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadPressedButtonHintProperty);
                interactables.CanInteractWhenPickedUp = EditorGUILayout.Toggle("CanInteractWhenPickedUp", interactables.CanInteractWhenPickedUp);
                if (interactables.CanInteractWhenPickedUp)
                {
                    EditorGUILayout.PropertyField(KeyboardPickedUpButtonHintProperty);
                    EditorGUILayout.PropertyField(GamepadPickedUpButtonHintProperty);
                }
                break;
            case InteractionType.Press:
                EditorGUILayout.PropertyField(KeyboardPressedButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadPressedButtonHintProperty);
                break;
            case InteractionType.Hold:
                interactables.HoldingTime = EditorGUILayout.FloatField("HoldingTime", interactables.HoldingTime);
                EditorGUILayout.PropertyField(KeyboardPressedButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadPressedButtonHintProperty);
                EditorGUILayout.PropertyField(HoldingHintProperty);
                break;
            case InteractionType.MultiPress:
                interactables.PressCountToFinishTask = EditorGUILayout.IntField("PressCountToFinishTask", interactables.PressCountToFinishTask);
                EditorGUILayout.PropertyField(KeyboardPressedButtonHintProperty);
                EditorGUILayout.PropertyField(KeyboardReleaseButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadPressedButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadReleaseButtonHintProperty);
                break;
        }

        serializedObject.ApplyModifiedProperties();
        DrawPropertiesExcluding(serializedObject, "m_Script");

        if (EditorGUI.EndChangeCheck())
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null) EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }
}

#endif
