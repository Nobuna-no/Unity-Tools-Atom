using UnityEditor;
using UnityEngine;

namespace UnityTools.Atom
{
    [CustomEditor(typeof(Action))]
    public class ActionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Raise action"))
                ((Action)target).Raise();
        }
    }
}
