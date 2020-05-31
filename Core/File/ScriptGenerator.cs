using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;

using UnityEditor;

namespace UnityTools.Atom
{
    public class ScriptGeneratorEditor : EditorWindow
    {
        #region PARAMETERS
        private static string ClassName = "New Class Name";
        private static string pathName = "Assets/";

        public static ScriptGeneratorDescriptorAsset Descriptor;
        #endregion

        #region UNITY METHODS
        private void OnGUI()
        {
            if (Descriptor == null)
            {
                GUILayout.Box("Please open an ScriptGeneratorDescriptorAsset!");
                return;
            }

            ClassName = GUILayout.TextField(ClassName);
            pathName = GUILayout.TextField(pathName);

            if (GUILayout.Button("Generate"))
            {
                if (ClassName.Length > 0)
                {
                    foreach (FileDescription data in Descriptor.DescriptionOfFileToGenerate)
                    {
                        string currentName = ClassName.Replace(" ", "");
                        currentName = data.FilePrefix + currentName + data.FileSuffix;

                        pathName = pathName.Replace(" ", "");
                        bool needLastSlash = pathName[pathName.Length - 1] != '/';
                        if (needLastSlash)
                        {
                            pathName += '/';
                        }
                        // Create folder if required.
                        DirectoryInfo di = Directory.CreateDirectory(pathName.Remove(pathName.Length - 1, 1));

                        string copyPath = pathName + currentName + ".cs";


                        Debug.Log("Creating C# file: " + copyPath);
                        if (File.Exists(copyPath) == false)
                        {
                            //string formatString = data.Template.Replace("\t", "   ");
                            string formatString = data.Template.Replace("<CLASSNAME>", ClassName.Replace(" ", ""));
                            string[] contentLines = formatString.Split('\n');

                            // do not overwrite
                            using (StreamWriter outfile = new StreamWriter(copyPath))
                            {
                                for (int i = 0, c = contentLines.Length; i < c; ++i)
                                {
                                    outfile.WriteLine(contentLines[i].Replace("\n", "").Replace("\r", ""));
                                }
                            }//File written
                        }
                    }
                    AssetDatabase.Refresh();
                }
            }
        }
        #endregion

        #region PUBLIC METHODS
        [MenuItem("Window/Script Generator Editor")]
        public static ScriptGeneratorEditor OpenGenerateScript()
        {
            return EditorWindow.GetWindow<ScriptGeneratorEditor>("Script Generator");
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenDatabase(int instanceID, int line)
        {
            ScriptGeneratorDescriptorAsset data = EditorUtility.InstanceIDToObject(instanceID) as ScriptGeneratorDescriptorAsset;

            if (data != null)
            {
                Descriptor = data;
                pathName = Descriptor.Destination;
                ScriptGeneratorEditor editorWindow = OpenGenerateScript();
                return true;
            }
            return false;
        }
        #endregion

    }
}
