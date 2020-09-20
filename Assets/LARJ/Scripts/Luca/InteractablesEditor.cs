using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(Interactables))]
public class InteractablesEditor : Editor
{
    public SerializedProperty HoldingInteractionEventProperty = null;
    public SerializedProperty PressInteractionEventProperty = null;

    private void OnEnable()
    {
        HoldingInteractionEventProperty = serializedObject.FindProperty("HoldingInteractionEvent");
        PressInteractionEventProperty = serializedObject.FindProperty("PressInteractionEvent");
    }

    public override void OnInspectorGUI()
    {
        Interactables interactables = target as Interactables;
        interactables.InteractionType = (InteractionType)EditorGUILayout.EnumPopup("Interaction Type", interactables.InteractionType);

        switch (interactables.InteractionType)
        {
            case InteractionType.PickUp: 
                break;
            case InteractionType.Press:
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(PressInteractionEventProperty);
                break;
            case InteractionType.Hold:
                interactables.HoldingTime = EditorGUILayout.FloatField("HoldingTime", interactables.HoldingTime);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(HoldingInteractionEventProperty);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}

#endif
