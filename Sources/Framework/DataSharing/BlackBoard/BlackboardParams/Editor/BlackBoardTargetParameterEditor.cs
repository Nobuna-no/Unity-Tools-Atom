using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityScript.Steps;

namespace UnityTools.Atom
{
    [CustomPropertyDrawer(typeof(BlackboardTargetParameter))]
    public class BlackboardTargetParameterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int indent = EditorGUI.indentLevel;

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.BeginChangeCheck();

            GUIContent detail = new GUIContent(GUIContent.none);

            SerializedProperty target = property.FindPropertyRelative("Target");
            SerializedProperty blackboard = property.FindPropertyRelative("BlackBoard");
            SerializedProperty entryName = property.FindPropertyRelative("EntryName");

            float we = EditorGUIUtility.currentViewWidth * .19f;
            float cey = position.y;
            Rect re2 = new Rect(position.x, position.y, we, EditorGUIUtility.singleLineHeight);
            Rect re3 = new Rect(1 + position.x + we, position.y, we, EditorGUIUtility.singleLineHeight);
            Rect re4 = new Rect(1 + position.x + we * 2, position.y, we, EditorGUIUtility.singleLineHeight);
            Rect re5 = new Rect(1 + position.x + we * 3, position.y, we, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(re2, target, GUIContent.none);
            EditorGUI.PropertyField(re3, blackboard, GUIContent.none); 
            EditorGUI.PropertyField(re4, entryName, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }
    }

    public class BlackboardTargetParameterDrawer<T, TAsset, TVariable> : BlackboardTargetParameterDrawer
    {
    }

    [CustomPropertyDrawer(typeof(BBTP_Bool))]
    public class BBTP_BoolDrawer : BlackboardTargetParameterDrawer<bool, BoolAsset, BoolVariable>
    {

    }

}