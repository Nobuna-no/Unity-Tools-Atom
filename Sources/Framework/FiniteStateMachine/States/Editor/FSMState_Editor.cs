using UnityEngine;
using UnityEditor;

using UnityEditorInternal;
using System.Collections.Generic;
using UnityEditor.Rendering;

namespace UnityTools.Atom
{

    public class FSMState_Editor<T> : Editor
        where T : FSMState
    {
        protected T Target;

        private ReorderableList TransitionArray;


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

            TransitionListSetup();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            DrawCustomInspector();

            EditorGUILayout.Space(25);
            DrawModules();
        }

        private void DrawCustomInspector()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            GUIStyle st = new GUIStyle(EditorStyles.miniButton);
            st.fontStyle = FontStyle.Bold;
            st.fontSize = EditorStyles.boldLabel.fontSize;

            SerializedProperty show = serializedObject.FindProperty("_ShowTransitions");

            show.boolValue = GUILayout.Toggle(show.boolValue, new GUIContent("State Transitions"), st);
            serializedObject.ApplyModifiedProperties();

            if(show.boolValue)
            {
                EditorGUILayout.Space(10);
                TransitionArray.DoLayoutList();
            }


            EditorGUILayout.EndVertical();
        }

        private void TransitionListSetup()
        {
            // Step 1: Get properties.
            SerializedProperty ExConditions = serializedObject.FindProperty("_Transitions");

            // Step 1.1: Security check.
            if (ExConditions == null)
            {
                return;
            }

            // Step 2: Setup Reoderable lists.
            TransitionArray = new ReorderableList(serializedObject, ExConditions, true, true, true, true);

            // Step 2.1: CallBacks setup.
            // Draw Header
            TransitionArray.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Transition", EditorStyles.boldLabel);
            };

            // Draw Element
            TransitionArray.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocus) =>
            {
                Rect r1 = new Rect(rect.x, rect.y, rect.width * 0.75f, rect.height);
                Rect r2 = new Rect(rect.x + r1.width * 1.025f, rect.y, rect.width * 0.225f, rect.height);

                EditorGUI.ObjectField(r1, ExConditions.GetArrayElementAtIndex(index), GUIContent.none);

                SerializedProperty subscribed = ExConditions.GetArrayElementAtIndex(index);
                FSMStateTransition b = subscribed.objectReferenceValue as FSMStateTransition;
                
                if(b != null)
                {
                    GUIStyle st = new GUIStyle(EditorStyles.helpBox);
                    //st.fontStyle = FontStyle.Bold;
                    st.fontSize = EditorStyles.boldLabel.fontSize;
                    st.alignment = TextAnchor.MiddleCenter;
                    st.focused.textColor = st.active.textColor = st.hover.textColor = st.normal.textColor = b.BlockTransition ? new Color(0.95f, 0.55f, 0.05f)  : Color.green;
                    
                    b.BlockTransition = EditorGUI.ToggleLeft(r2, b.BlockTransition ? "Disabled" : "Enabled", b.BlockTransition, st);
                    ExConditions.GetArrayElementAtIndex(index).serializedObject.ApplyModifiedProperties();
                }
            };

            // On Add
            SetAddDropdownCallback(TransitionArray);

            // On Remove
            SetRemoveCallback(TransitionArray, Target.Transitions);
        }

        private void SetAddDropdownCallback(ReorderableList list)
        {
            list.onAddDropdownCallback = (Rect rect, ReorderableList rlist) =>
            {
                GameObject newTransition = Instantiate(new GameObject(), Target.transform).gameObject;
                newTransition.name = "[FSM_ST]" + Target.gameObject.name + "_" + rlist.count;
                newTransition.AddComponent<FSMStateTransition>();

                int index = list.serializedProperty.arraySize++;
                list.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = newTransition;
                serializedObject.ApplyModifiedProperties();
            };
        }

        private void SetRemoveCallback(ReorderableList list, List<FSMStateTransition> targetList)
        {
            list.onRemoveCallback = (ReorderableList rlist) =>
            {
                int j = rlist.index;

                FSMStateTransition dataCondition = targetList[j];

                targetList.RemoveAt(j);

                if (dataCondition)
                {
                    DestroyImmediate(dataCondition.gameObject);
                    EditorGUIUtility.ExitGUI();
                }
            };
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


            SerializedProperty showStateModule = serializedObject.FindProperty("_ShowStateModules");

            // EditorGUILayout.LabelField("State Modules", st);
            showStateModule.boolValue = GUILayout.Toggle(showStateModule.boolValue, new GUIContent("State Modules"), st);
            serializedObject.ApplyModifiedProperties();

            if (showStateModule.boolValue)
            {
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