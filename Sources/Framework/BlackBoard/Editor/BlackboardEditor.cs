using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using System.IO;

namespace UnityTools.Atom
{
    [CustomEditor(typeof(BlackBoardAsset))]
    public class BlackBoardAssetEditor : Editor
    {
        #region PROPERTIES
        private BlackBoardAsset Target;
        private ReorderableList ExecuteList;
        #endregion


        #region UNITY METHODS
        private void OnDisable()
        {

            Target.OnAssetDeletion -= ClearBeforeDeletion;
        }

        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }

            Target = (BlackBoardAsset)target;
            Target.OnAssetDeletion += ClearBeforeDeletion;

            // Step 1: Get properties.
            SerializedProperty BlackboardParameters = serializedObject.FindProperty("BlackboardParameters");


            // Step 1.1: Security check.
            if (BlackboardParameters == null)
            {
                return;
            }

            // Step 2: Setup Reoderable lists.
            ExecuteList = new ReorderableList(serializedObject, BlackboardParameters, true, true, true, true);

            // Step 2.1: CallBacks setup.
            // Draw Header
            ExecuteList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, new GUIContent("Blackboard parameters"));
            };

            // Draw Element
            ExecuteList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocus) =>
            {
                serializedObject.Update();
                BlackboardParameterDrawerUtility.DrawElementCallback(ExecuteList, rect, index);
            };

            // On Add
            ExecuteList.onAddDropdownCallback = (Rect rect, ReorderableList rlist) =>
            {
                GenericMenu dropdownMenu = new GenericMenu();
                string[] assetGUIDS = AssetDatabase.FindAssets("l:BlackboardParameter");

                for (int i = 0; i < assetGUIDS.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(assetGUIDS[i]);
                    path = System.IO.Path.GetFileNameWithoutExtension(path);

                    dropdownMenu.AddItem(new GUIContent(path.Replace("InternalBlackboardParameter_", "")), false, AddItem, new AssetInfo<ReorderableList> { AssetPath = path, ComplementaryData = ExecuteList });
                }

                dropdownMenu.ShowAsContext();
            };

            ExecuteList.onRemoveCallback = (ReorderableList rlist) =>
            {
                int i = ExecuteList.index;

                if (i >= Target.BlackboardParameters.Count)
                {
                    return;
                }

                RemoveItem(i);
            };

            ExecuteList.elementHeightCallback = (int index) =>
            {
                return BlackboardParameterDrawerUtility.ElementHeightCallback(ExecuteList, index);
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //EditorGUILayout.BeginVertical();//FilledBackgroundStyle.BackgroundStyle((int)EditorGUIUtility.currentViewWidth));
            ExecuteList.DoLayoutList();
            //EditorGUILayout.EndVertical();

            EditorUtility.SetDirty(Target);
        }
        #endregion


        #region PRIVATE METHODS
        private void RemoveItem(int index)
        {
            InternalBlackboardParameter dataCondition = Target.BlackboardParameters[index];

            Target.BlackboardParameters.RemoveAt(index);

            if (dataCondition)
            {
                //dataCondition.hideFlags = HideFlags.None;
                EditorUtility.SetDirty(dataCondition);
                string pathToScript = AssetDatabase.GetAssetPath(dataCondition);
                AssetDatabase.DeleteAsset(pathToScript);
                DestroyImmediate(dataCondition, true);
            }
        }

        private void AddItem(object obj)
        {
            AssetInfo<ReorderableList> assetInfo = (AssetInfo<ReorderableList>)obj;

            string assetName = (assetInfo.AssetPath);

            System.Type assetType = System.Type.GetType("UnityTools.Atom." + assetName + ", Assembly-CSharp");
            if(assetType == null)
            {
                assetType = System.Type.GetType(assetName + ", Assembly-CSharp");
            }

            if (assetType == null)
            {
                Logger.Log(Logger.Type.Error, "Blackboard parameter of unknown type. Please put your type in the UnityTools.Atom namespace.", this);
                return;
            }

            InternalBlackboardParameter param = ScriptableObject.CreateInstance(assetType) as InternalBlackboardParameter;

            string path = PlayerPrefs.GetString("Unity-Tools-Atom-DataCache", "");

            if (path.Length == 0)
            {
                SetDefaultDataCache(out path);
            }

            path += "BlackboardsKeys/" + GUID.Generate() + ".asset";

            {
                // Create the directory if needed.
                (new System.IO.FileInfo(path)).Directory.Create();
            }

            //param.hideFlags = HideFlags.HideInHierarchy;// | HideFlags.NotEditable;
            AssetDatabase.CreateAsset(param, path);
            EditorUtility.SetDirty(param);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (!param)
            {
                Debug.Break();
                Debug.LogError(this + ": ScriptableObject.CreateInstance(assetType) as DATA_BlackboardParameter is null!");
            }

            param.Name.Value = "Param" + Target.BlackboardParameters.Count.ToString();
            Target.BlackboardParameters.Add(param);
            int index = assetInfo.ComplementaryData.serializedProperty.arraySize++;
            assetInfo.ComplementaryData.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = param;

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }

        private void ClearBeforeDeletion()
        {
            for (int i = Target.BlackboardParameters.Count - 1; i >= 0; --i)
            {
                RemoveItem(i);
            }
            Target.BlackboardParameters.Clear();
        }

        private void SetDefaultDataCache(out string path)
        {
            // default path
            path = "UnityTools/Atom/DataCache";

            bool bFound = false;

            while (true)
            {
                if (bFound)
                {
                    break;
                }

                string[] allPaths = AssetDatabase.GetSubFolders("Assets");
                if (allPaths.Length == 0)
                {
                    break;
                }

                foreach (string str in allPaths)
                {
                    if (bFound)
                    {
                        break;
                    }

                    string[] subPath = AssetDatabase.GetSubFolders(str);
                    if (subPath.Length == 0)
                    {
                        break;
                    }

                    foreach (string sstr in subPath)
                    {
                        if (sstr.Contains("Unity-Tools-Atom"))
                        {
                            path = sstr + "/DataCache/";
                            PlayerPrefs.SetString("Unity-Tools-Atom-DataCache", path);
                            bFound = true;
                            break;
                        }
                    }
                }
            }
        }
        #endregion
    }
}

