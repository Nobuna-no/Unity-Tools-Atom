using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace UnityTools.Atom
{


    [Serializable]
    class AddButtonScript_EditorWindow_RuntimeBackup : ScriptableObject
    {
        public string scriptPath;
        public bool addAsset;

        public static AddButtonScript_EditorWindow_RuntimeBackup Instance
        {
            get
            {
                var objs = Resources.FindObjectsOfTypeAll<AddButtonScript_EditorWindow_RuntimeBackup>();
                if (objs.Length == 0 || objs[0] == null)
                {
                    return ScriptableObject.CreateInstance<AddButtonScript_EditorWindow_RuntimeBackup>();
                }
                return objs[0];
            }
        }

        void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public void Reset()
        {
            addAsset = false;
            scriptPath = string.Empty;
        }
    }


    // From https://forum.unity.com/threads/custom-add-component-like-button.439730/
    public class AddButtonScript_EditorWindow : EditorWindow
    {
        #region PROPERTIES
        private static AddButtonScript_EditorWindow _Instance;
        private static Styles _Styles;

        private Action<MonoScript> _CreateScriptDelegate;
        private Func<MonoScript, bool> _FilerScriptDelegate;

        private Vector2 _ScrollPosition;
        //private string _ClassName = "NewEquipmentBehaviourScript";
        private bool _ActiveParent = true;


        private string _SearchString = string.Empty;

        private const char UNITY_FOLDER_SEPARATOR = '/';
        private string _TemplatePath;
        #endregion


        #region STATIC METHODS
        public static bool HasAssetToAdd()
        {
            return AddButtonScript_EditorWindow_RuntimeBackup.Instance.addAsset;
        }
        private void Init(Rect rect, Action<MonoScript> onCreateScript, Func<MonoScript, bool> onFilerScript, string templatePath)
        {
            var v2 = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
            rect.x = v2.x;
            rect.y = v2.y;

            //CreateComponentTree();
            ShowAsDropDown(rect, new Vector2(rect.width, 320f));
            Focus();
            wantsMouseMove = true;
            _CreateScriptDelegate = onCreateScript;
            _FilerScriptDelegate = onFilerScript;
        }

        public static void Show(Action<MonoScript> onCreateScript, Func<MonoScript, bool> onFilerScript, string templatePath, string buttonText = "Add ...")
        {

            if (_Instance == null)
            {
                _Instance = ScriptableObject.CreateInstance<AddButtonScript_EditorWindow>();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontSize = 12;
            style.fixedWidth = 230;
            style.fixedHeight = 23;

            var rect = GUILayoutUtility.GetLastRect();
            //var hasAssetToAdd = HasAssetToAdd();
            //EditorGUI.BeginDisabledGroup(hasAssetToAdd);
            if (GUILayout.Button(buttonText, style))
            {
                rect.y += 26f;
                rect.x += rect.width;
                rect.width = style.fixedWidth;
                _Instance.Init(rect, onCreateScript, onFilerScript, templatePath);
                _Instance.Repaint();
            }
            //EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            //if (hasAssetToAdd)
            //{
            //  //  Backup(onCreateScript);
            //}
        }

        public static void Backup(Action<MonoScript> onCreateScript)
        {
            if (_Instance == null)
            {
                _Instance = ScriptableObject.CreateInstance<AddButtonScript_EditorWindow>();
            }
            _Instance._CreateScriptDelegate = onCreateScript;

            if (AddButtonScript_EditorWindow_RuntimeBackup.Instance.addAsset)
            {
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(AddButtonScript_EditorWindow_RuntimeBackup.Instance.scriptPath);
                if (script.GetClass() == null)
                {
                    return;
                }

                string[] assetGUIDS = AssetDatabase.FindAssets("l:ABCD");

                for (int i = 0; i < assetGUIDS.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(assetGUIDS[i]);
                    path = System.IO.Path.GetFileNameWithoutExtension(path);
                }
                AssetInfo info = new AssetInfo { AssetPath = "" };
                _Instance._CreateScriptDelegate(script);
                AddButtonScript_EditorWindow_RuntimeBackup.Instance.Reset();
            }
        }

        /// <summary>
        /// Try to add a component on a gameobject from assetInfo.
        /// </summary>
        /// <param name="info">The asset info...</param>
        /// <returns>Return true if succeed.</returns>
        public static bool AddComponentFromAssetInfo(AssetInfo info, GameObject target)
        {
            if (info == null)
            {
                return false;
            }

            System.Type assetType = info.GetAssetType();
            FSMStateModule instance = target.AddComponent(assetType) as FSMStateModule;
            return true;
        }

        public static bool CanAddScript(MonoScript script)
        {
            var scriptClass = script.GetClass();
            if (scriptClass == null)
            {
                return false;
            }
            return !scriptClass.IsAbstract;
        }

        public static bool CanAddScriptOfType<T>(MonoScript script)
        {
            var scriptClass = script.GetClass();
            if (scriptClass == null)
            {
                return false;
            }
            return !scriptClass.IsAbstract && scriptClass.IsSubclassOf(typeof(T));
        }
        #endregion


        private void OnGUI()
        {
            if (_Styles == null)
            {
                _Styles = new Styles();
            }
            GUI.Label(new Rect(0.0f, 0.0f, this.position.width, this.position.height), GUIContent.none, _Styles.background);

            //GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            GUILayout.Space(7);
            GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(!_ActiveParent);
            _SearchString = GUILayout.TextField(_SearchString, GUI.skin.FindStyle("SearchTextField"));
            var buttonStyle = _SearchString == string.Empty ? GUI.skin.FindStyle("SearchCancelButtonEmpty") : GUI.skin.FindStyle("SearchCancelButton");
            if (GUILayout.Button(string.Empty, buttonStyle))
            {
                // Remove focus if cleared
                _SearchString = string.Empty;
                GUI.FocusControl(null);
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            if (_ActiveParent)
            {
                ListGUI();
            }
        }

        void ListGUI()
        {
            var rect = position;
            rect.x = +1f;
            rect.y = 30f;
            rect.height -= 30f;
            rect.width -= 2f;
            GUILayout.BeginArea(rect);

            rect = GUILayoutUtility.GetRect(10f, 25f);
            GUI.Label(rect, _SearchString == string.Empty ? "Module" : "Search", _Styles.header);
            _ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition);
            var scripts = Resources.FindObjectsOfTypeAll<MonoScript>();
            var searchString = _SearchString.ToLower();

            foreach (var script in scripts)
            {
                if (!script || script.GetClass() == null || !_FilerScriptDelegate(script))
                {
                    continue;
                }
                if (searchString != string.Empty && !script.name.ToLower().Contains(searchString))
                {
                    continue;
                }

                var buttonRect = GUILayoutUtility.GetRect(16f, 20f, GUILayout.ExpandWidth(true));
                if (GUI.Button(buttonRect, script.name, _Styles.componentButton))
                {
                    _CreateScriptDelegate(script);
                    //CreateScriptInstance(script);
                    Close();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private class Styles
        {
            public GUIStyle header = new GUIStyle((GUIStyle)"In BigTitle");
            public GUIStyle componentButton = new GUIStyle((GUIStyle)"PR Label");
            public GUIStyle background = (GUIStyle)"grey_border";
            public GUIStyle previewBackground = (GUIStyle)"PopupCurveSwatchBackground";
            public GUIStyle previewHeader = new GUIStyle(EditorStyles.label);
            public GUIStyle previewText = new GUIStyle(EditorStyles.wordWrappedLabel);
            public GUIStyle rightArrow = (GUIStyle)"AC RightArrow";
            public GUIStyle leftArrow = (GUIStyle)"AC LeftArrow";
            public GUIStyle groupButton;

            public Styles()
            {
                this.header.font = EditorStyles.boldLabel.font;
                this.componentButton.alignment = TextAnchor.MiddleLeft;
                this.componentButton.padding.left -= 15;
                this.componentButton.fixedHeight = 20f;
                this.groupButton = new GUIStyle(this.componentButton);
                this.groupButton.padding.left += 17;
                this.previewText.padding.left += 3;
                this.previewText.padding.right += 3;
                ++this.previewHeader.padding.left;
                this.previewHeader.padding.right += 3;
                this.previewHeader.padding.top += 3;
                this.previewHeader.padding.bottom += 2;
            }
        }
    }
}