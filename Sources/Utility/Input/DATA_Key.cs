using UnityEngine;

namespace UnityTools.Atom
{
    [System.Serializable, CreateAssetMenu(menuName = "Unity Tools/Atom/Utility/Input/Key", order = 0)]
    public class DATA_Key : BoolAsset
    {
        [Header(".DATA/Key")]
        public string Name;
        public string Description;
        [Space]
        [Space]
        public KeyCode[] KeyCodes;

        /// <summary>
        /// (Value == true) mean that the key is down.
        /// Time use when Value is true means the time since it's pressed. 
        /// Time use when Value is false means the duration since the key was released.
        /// </summary>
        [System.NonSerialized]
        public float Timestamp;

        /// <summary>
        /// Force value to false and reset timestamp.
        /// </summary>
        public void ConsumeInput()
        {
            Value = false;
            Timestamp = 0;
        }
    }
}
