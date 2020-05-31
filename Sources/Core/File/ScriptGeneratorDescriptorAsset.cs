using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{
    #region STRUCTS
    [System.Serializable]
    public class FileDescription
    {
        public string FilePrefix;
        public string FileSuffix;
        [TextArea(1, 10)]
        public string Template = "using UnityEngine;\nusing System.Collections;\npublic class <CLASSNAME> : MonoBehaviour{}";
    }
    #endregion

    [CreateAssetMenu(fileName = "[SGDescriptor]-", menuName = "Unity Tools/Atom/Core/File/ScriptGeneratorDescriptor")]
    public class ScriptGeneratorDescriptorAsset : ScriptableObject
    {
        #region PARAMETERS
        public List<FileDescription> DescriptionOfFileToGenerate = new List<FileDescription>(1);
        public string Destination = "Assets/";
        #endregion
    }
}
