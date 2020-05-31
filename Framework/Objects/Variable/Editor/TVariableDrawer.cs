using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace UnityTools.Atom
{
    public class TVariableDrawer : PropertyDrawer
    {
        #region PARAMETERS
        // Use to display tooltips in inspector.
        private readonly string[] _PopupOptions = { "Use Constant", "Use Asset" };

        // Used to store guistyle of the property drawer.
        private GUIStyle _PopupStyle;
        #endregion

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_PopupStyle == null)
            {
                _PopupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                _PopupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            // Step 1: Get properties
            SerializedProperty useConstant = property.FindPropertyRelative("_UseConstant");
            SerializedProperty constantValue = property.FindPropertyRelative("ConstantValue");
            SerializedProperty variable = property.FindPropertyRelative("Asset");


            // Step 2: Calculate rect for configuration button
            Rect buttonRect = new Rect(position);
            buttonRect.yMin += _PopupStyle.margin.top;
            buttonRect.width = _PopupStyle.fixedWidth + _PopupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // Step 3: Store old indent level and set it to 0, the prefix Lavel takes care of it
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, _PopupOptions, _PopupStyle);
            useConstant.boolValue = result == 0;

            EditorGUI.PropertyField(position, useConstant.boolValue ? constantValue : variable, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(IntegerVariable))]
    public class IntegerVariableDrawer : TVariableDrawer
    { }

    [CustomPropertyDrawer(typeof(StringVariable))]
    public class StringVariableDrawer : TVariableDrawer
    { }


    [CustomPropertyDrawer(typeof(AnyVariable))]
    public class AnyVariableDrawer : PropertyDrawer
    {
        #region PARAMETERS
        // Use to display tooltips in inspector.
        private readonly string[] _PopupOptions = { "Use Constant", "Use Asset" };

        // Used to store guistyle of the property drawer.
        private GUIStyle _PopupStyle;
        #endregion

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_PopupStyle == null)
            {
                _PopupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                _PopupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            // Step 1: Get properties
            SerializedProperty useConstant = property.FindPropertyRelative("_UseConstant");
            SerializedProperty constantValue = property.FindPropertyRelative("ConstantValue");
            SerializedProperty variable = property.FindPropertyRelative("AnyAsset");


            // Step 2: Calculate rect for configuration button
            Rect buttonRect = new Rect(position);
            buttonRect.yMin += _PopupStyle.margin.top;
            buttonRect.width = _PopupStyle.fixedWidth + _PopupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // Step 3: Store old indent level and set it to 0, the prefix Lavel takes care of it
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, _PopupOptions, _PopupStyle);
            useConstant.boolValue = result == 0;

            EditorGUI.PropertyField(position, useConstant.boolValue ? constantValue : variable, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}