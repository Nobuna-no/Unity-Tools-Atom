using UnityEngine;

using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxRange))]
public class MinMaxRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int indent = EditorGUI.indentLevel;

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);
        EditorGUI.BeginChangeCheck();

        SerializedProperty min = property.FindPropertyRelative("MinLimit");
        SerializedProperty max = property.FindPropertyRelative("MaxLimit");
        SerializedProperty minValue = property.FindPropertyRelative("MinValue");
        SerializedProperty maxValue = property.FindPropertyRelative("MaxValue");

       
        float cey = position.y;
        //Rect re1 = new Rect(position.x, cey, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        Rect re2 = new Rect(position.x, cey + 2, EditorGUIUtility.fieldWidth * 0.5f, EditorGUIUtility.singleLineHeight * 0.8f);
        Rect re3 = new Rect(EditorGUIUtility.currentViewWidth - EditorGUIUtility.fieldWidth, cey + 2, EditorGUIUtility.fieldWidth * 0.5f, EditorGUIUtility.singleLineHeight * 0.8f);

        float pos = re2.position.x + re2.width + EditorGUIUtility.fieldWidth * 0.25f;
        Rect re4 = new Rect(pos, cey + 4, re3.position.x - pos - EditorGUIUtility.fieldWidth * 0.25f, EditorGUIUtility.singleLineHeight);

        float minf = minValue.floatValue, maxf = maxValue.floatValue;
        // Step 1: Label.
        //EditorGUI.LabelField(re1, property.displayName);
        EditorGUI.indentLevel = 0;

        // Step 2: Min - Max.
        GUIStyle style = new GUIStyle(EditorStyles.numberField);
        style.alignment = TextAnchor.MiddleCenter;
        min.floatValue = EditorGUI.FloatField(re2, min.floatValue, style);
        max.floatValue = EditorGUI.FloatField(re3, max.floatValue, style);

        // Step 3: Slider.
        EditorGUI.MinMaxSlider(re4, ref minf, ref maxf, min.floatValue, max.floatValue);

        // Step 4: Update values.
        minValue.floatValue = minf < min.floatValue ? min.floatValue : minf;
        maxValue.floatValue = maxf > max.floatValue ? max.floatValue : maxf;

        if(minValue.floatValue > maxValue.floatValue)
        {
            minValue.floatValue = maxValue.floatValue;
        }


        re4.yMin -= 8;
        re4.yMax -= 8;
        
        // Step 5: Infos.
        EditorGUI.LabelField(re4, "Min:" + minf.ToString("0.00") + " | Max:" + maxf.ToString("0.00"), EditorStyles.centeredGreyMiniLabel);

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);//height;
    }
}

