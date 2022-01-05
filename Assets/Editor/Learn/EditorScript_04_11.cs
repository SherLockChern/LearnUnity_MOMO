using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Script_04_11))]
public class EditorScript_04_11 : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty propertyKey = serializedObject.FindProperty("m_keys");
        SerializedProperty propertyValue = serializedObject.FindProperty("m_values");
        int size = propertyKey.arraySize;
        GUILayout.BeginVertical();
        for (int i = 0; i < size; i++)
        {
            GUILayout.BeginHorizontal();
            SerializedProperty key = propertyKey.GetArrayElementAtIndex(i);
            SerializedProperty value = propertyValue.GetArrayElementAtIndex(i);
            key.stringValue = EditorGUILayout.TextField("key", key.stringValue);
            value.objectReferenceValue =
                EditorGUILayout.ObjectField("value", value.objectReferenceValue, typeof(Sprite), false);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+"))
        {
            (target as Script_04_11).spriteDic[size.ToString()] = null;
        }
        GUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }
}
