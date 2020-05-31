using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace UnityTools.Atom
{
    public static class BlackboardParameterDrawerUtility
    {
        private static SerializedProperty TryGetSafeElement(ReorderableList rList, int index)
        {
            if (rList.serializedProperty.arraySize <= index)
            {
                return null;
            }
            SerializedProperty element = rList.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null || element.objectReferenceValue == null)
            {
                return null;
            }

            return element;
        }

        private static readonly string[] ConditionPopupOptions =
               { "==", "!=", ">", ">=", "<", "<=" };

        public static void DrawElementCallback(ReorderableList rList, Rect rect, int index)
        {
            SerializedProperty element = TryGetSafeElement(rList, index);
            if (element == null)
            {
                Rect labelRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelRect, "this element seems to have been manually deleted. Remove it.", EditorStyles.boldLabel);
                return;
            }

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float lineHeightSpace = lineHeight + 5f;

            SerializedObject elementObject = new SerializedObject(element.objectReferenceValue);
            elementObject.Update();



            SerializedProperty name = elementObject.FindProperty("Name");
            bool valid = true;
            string paramName = "";
            {
                if (name.FindPropertyRelative("_UseConstant").boolValue)
                {
                    paramName = name.FindPropertyRelative("ConstantValue").stringValue;
                }
                else
                {
                    UnityEngine.Object var = name.FindPropertyRelative("Asset").objectReferenceValue as UnityEngine.Object;
                    if (var)
                    {
                        paramName = (var as StringAsset).Value;
                    }
                }
                if (paramName == "")
                {
                    paramName = "None";
                    valid = false;
                }
            }

            string finalSentence;
            if (valid)
            {
                finalSentence = "Parameter: " + "\"" + paramName + "\"";
            }
            else
            {
                finalSentence = "Invalid parameter";
            }

            // Remove all prefix in the type (namespace + BlackboardParameter_ ...)
            string typeStr = elementObject.targetObject.GetType().ToString();
            typeStr = typeStr.Remove(0, typeStr.IndexOf("_") + 1);

            float ry = rect.y + lineHeight * 0.1f;
            Rect labelRect0 = new Rect(rect.x, ry, rect.width * 0.7f, lineHeight);
            Rect labelRect1 = new Rect(rect.x + rect.width * 0.75f, ry, rect.width * 0.25f, lineHeight);

            GUIStyle customHelpBoxStyle = new GUIStyle(EditorStyles.helpBox);
            customHelpBoxStyle.alignment = TextAnchor.MiddleCenter;
            customHelpBoxStyle.fontStyle = FontStyle.Bold;
            EditorGUI.LabelField(labelRect0, finalSentence, valid ? EditorStyles.helpBox : customHelpBoxStyle);
            EditorGUI.LabelField(labelRect1, typeStr, customHelpBoxStyle);

           

            SerializedProperty description = elementObject.FindProperty("Description");
            SerializedProperty bshared = elementObject.FindProperty("MustBeShared");
            SerializedProperty ValueRef = elementObject.FindProperty("Value");

            Rect buttonRect0 = new Rect(rect.x, rect.y + lineHeightSpace, rect.width, lineHeight);
            Rect buttonRect1 = new Rect(rect.x, rect.y + lineHeightSpace * 2, rect.width, lineHeight);
            Rect buttonRect2 = new Rect(rect.x, rect.y + lineHeightSpace * 3, rect.width, lineHeight);
            Rect buttonRect3 = new Rect(rect.x, rect.y + lineHeightSpace * 4, rect.width, lineHeight);

            EditorGUI.PropertyField(buttonRect0, name, new GUIContent("Entry Name"), false);
            //EditorGUI.ObjectField(buttonRect0, name, GUIContent.none);

            description.stringValue = EditorGUI.TextField(buttonRect1, "Entry Description", description.stringValue);
            description.serializedObject.ApplyModifiedProperties();

            bshared.boolValue = EditorGUI.Toggle(buttonRect2, new GUIContent("Instance Synced", "May this parameter be shared with all blackboard's owners."), bshared.boolValue);
            bshared.serializedObject.ApplyModifiedProperties();

            if (bshared.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(buttonRect3, ValueRef, new GUIContent("Shared Variable"));
                EditorGUI.indentLevel--;
            }

            if (elementObject.hasModifiedProperties)
            {
                elementObject.ApplyModifiedProperties();
            }
        }

        public static float ElementHeightCallback(ReorderableList rList, int index)
        {
            SerializedProperty element = TryGetSafeElement(rList, index);
            if (element == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            SerializedObject elementObject = new SerializedObject(element.objectReferenceValue);
            SerializedProperty bshared = elementObject.FindProperty("MustBeShared");


            float height = 0;
            float lineHeightSpace = EditorGUIUtility.singleLineHeight + 5f;
            height = lineHeightSpace * (bshared.boolValue ? 5f : 4f);
            return height;
        }
    }

}