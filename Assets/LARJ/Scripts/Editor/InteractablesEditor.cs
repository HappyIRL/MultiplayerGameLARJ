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

    //Arrow keys
    public SerializedProperty KeyboardUpArrowHintProperty = null;
    public SerializedProperty KeyboardLeftArrowHintProperty = null;
    public SerializedProperty KeyboardDownArrowHintProperty = null;
    public SerializedProperty KeyboardRightArrowHintProperty = null;
    public SerializedProperty GamepadUpArrowHintProperty = null;
    public SerializedProperty GamepadLeftArrowHintProperty = null;
    public SerializedProperty GamepadDownArrowHintProperty = null;
    public SerializedProperty GamepadRightArrowHintProperty = null;

    //Progressbar
    public SerializedProperty ProgressbarProperty = null;
    public SerializedProperty ProgressbarBackgroundProperty = null;

    //Pick up
    public SerializedProperty PickUpTransformProperty = null;

    private void OnEnable()
    {
        KeyboardPressedButtonHintProperty = serializedObject.FindProperty("KeyboardPressedButtonHintImage");
        KeyboardReleaseButtonHintProperty = serializedObject.FindProperty("KeyboardReleaseButtonHintImage");
        GamepadPressedButtonHintProperty = serializedObject.FindProperty("GamepadPressedButtonHintImage");
        GamepadReleaseButtonHintProperty = serializedObject.FindProperty("GamepadReleaseButtonHintImage");
        HoldingHintProperty = serializedObject.FindProperty("HoldingHintImage");
        KeyboardPickedUpButtonHintProperty = serializedObject.FindProperty("MousePickedUpInteractionButtonHintImage");
        GamepadPickedUpButtonHintProperty = serializedObject.FindProperty("GamepadPickedUpInteractionButtonHintImage");

        KeyboardUpArrowHintProperty = serializedObject.FindProperty("KeyboardUpArrowHintImage");
        KeyboardLeftArrowHintProperty = serializedObject.FindProperty("KeyboardLeftArrowHintImage");
        KeyboardDownArrowHintProperty = serializedObject.FindProperty("KeyboardDownArrowHintImage");
        KeyboardRightArrowHintProperty = serializedObject.FindProperty("KeyboardRightArrowHintImage");
        GamepadUpArrowHintProperty = serializedObject.FindProperty("GamepadUpArrowHintImage");
        GamepadLeftArrowHintProperty = serializedObject.FindProperty("GamepadLeftArrowHintImage");
        GamepadDownArrowHintProperty = serializedObject.FindProperty("GamepadDownArrowHintImage");
        GamepadRightArrowHintProperty = serializedObject.FindProperty("GamepadRightArrowHintImage");

        ProgressbarProperty = serializedObject.FindProperty("Progressbar");
        ProgressbarBackgroundProperty = serializedObject.FindProperty("ProgressbarBackground");

        PickUpTransformProperty = serializedObject.FindProperty("TransformForPickUp");
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
                EditorGUILayout.PropertyField(PickUpTransformProperty);
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

                EditorGUILayout.LabelField("Progressbar", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(ProgressbarProperty);
                EditorGUILayout.PropertyField(ProgressbarBackgroundProperty);

                EditorGUILayout.LabelField("Press Images", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(KeyboardPressedButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadPressedButtonHintProperty);
                EditorGUILayout.PropertyField(HoldingHintProperty);
                break;
            case InteractionType.MultiPress:
                interactables.PressCountToFinishTask = EditorGUILayout.IntField("PressCountToFinishTask", interactables.PressCountToFinishTask);

                EditorGUILayout.LabelField("Progressbar", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(ProgressbarProperty);
                EditorGUILayout.PropertyField(ProgressbarBackgroundProperty);

                EditorGUILayout.LabelField("Press Images", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(KeyboardPressedButtonHintProperty);
                EditorGUILayout.PropertyField(KeyboardReleaseButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadPressedButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadReleaseButtonHintProperty);
                break;
            case InteractionType.PressTheCorrectKeys:
                interactables.CorrectKeysPressedCountToFinishTask = EditorGUILayout.IntField("CorrectKeysPressedCountToFinishTask", interactables.CorrectKeysPressedCountToFinishTask);

                EditorGUILayout.LabelField("Progressbar", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(ProgressbarProperty);
                EditorGUILayout.PropertyField(ProgressbarBackgroundProperty);

                EditorGUILayout.LabelField("Press Images", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(KeyboardPressedButtonHintProperty);
                EditorGUILayout.PropertyField(GamepadPressedButtonHintProperty);

                EditorGUILayout.LabelField("Arrow Images", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(KeyboardUpArrowHintProperty);
                EditorGUILayout.PropertyField(KeyboardLeftArrowHintProperty);
                EditorGUILayout.PropertyField(KeyboardDownArrowHintProperty);
                EditorGUILayout.PropertyField(KeyboardRightArrowHintProperty);
                EditorGUILayout.PropertyField(GamepadUpArrowHintProperty);
                EditorGUILayout.PropertyField(GamepadLeftArrowHintProperty);
                EditorGUILayout.PropertyField(GamepadDownArrowHintProperty);
                EditorGUILayout.PropertyField(GamepadRightArrowHintProperty);
                break;
        }

        DrawPropertiesExcluding(serializedObject, "m_Script");
        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null) EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }
}

#endif
