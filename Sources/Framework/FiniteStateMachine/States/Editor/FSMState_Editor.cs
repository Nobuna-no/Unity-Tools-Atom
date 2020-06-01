using UnityEngine;
using UnityEditor;


namespace UnityTools.Atom
{

    public class FSMState_Editor<T> : Editor
        where T : FSMState
    {
        protected T Target;
        //FSMStateModule
        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }

            // Step 0: Setup.
            Target = (T)target;

            SerializedProperty Modules = serializedObject.FindProperty("StateModule");

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            //DrawCustomInspector();
            DrawModules();
        }

        private void DrawCustomInspector()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.EndVertical();
        }

        bool DrawModule(Editor editor, int index)
        {
            if (editor == null || editor.target == null)
            {
                return false;
            }

            var fouldout = EditorGUILayout.InspectorTitlebar(EditorPrefs.GetBool(editor.target.GetType().Name, true), editor.target);
            EditorPrefs.SetBool(editor.target.GetType().Name, fouldout);

            if (fouldout)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Remove", EditorStyles.miniButtonRight))
                {
                    Target.RemoveStateModule(index);
                    EditorGUIUtility.ExitGUI();
                    return false;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                editor.DrawDefaultInspector();
                EditorGUI.indentLevel--;
                editor.serializedObject.ApplyModifiedProperties();
            }

            if (editor.target == null)
            {
                EditorGUIUtility.ExitGUI();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void DrawModules()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUIStyle st = new GUIStyle(EditorStyles.miniButton);
            st.fontStyle = FontStyle.Bold;
            st.fontSize = EditorStyles.boldLabel.fontSize;

            EditorGUILayout.LabelField("State Modules", st);

            EditorGUILayout.Space();

            bool draw = true;
            SerializedProperty modules = serializedObject.FindProperty("StateModules");
            for (int i = 0, c = modules.arraySize; i < c; ++i)
            {
                Object m = modules.GetArrayElementAtIndex(i).objectReferenceValue;
                if (m == null)
                {
                    Target.RemoveStateModule(i);
                    EditorGUIUtility.ExitGUI();
                    return;
                }
                else
                {
                    draw &= DrawModule(Editor.CreateEditor(m), i);
                }
            }

            if (draw && !Application.isPlaying)
            {
                EditorGUILayout.Space();
                AddButtonScript_EditorWindow.Show(CreateScriptInstance, AddButtonScript_EditorWindow.CanAddScriptOfType<FSMStateModule>, "", "Add State Module");
            }

            EditorGUILayout.EndVertical();
        }

        private void CreateScriptInstance(MonoScript info)
        {
            if (info == null)
            {
                Debug.LogError("Failed to CreateScriptInstance(): info is null");
                return;
            }

            Target.AddStateModule(Target.gameObject.AddComponent(info.GetClass()) as FSMStateModule);

            return;
        }
    }

    [CustomEditor(typeof(FSMState))]
    public class FSMState_EditorNoT : FSMState_Editor<FSMState> { }

}