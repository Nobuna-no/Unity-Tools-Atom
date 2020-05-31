using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{

    // Class that allow to get a variable reference to a blackboard of an undefined target.
    [System.Serializable]
    public abstract class BlackboardParameter
    {
        public StringVariable EntryName;
        protected bool _IsValid;
        public bool IsValid { get => _IsValid; }

        [SerializeField, HideInInspector]
        protected bool UseEntryName = true;
        // Use to set the default value.
        public virtual void Initialize(BlackBoardAsset board, GameObject Target)
        { }
    }

    public class BlackboardParameter_Template<T, TVariable> : BlackboardParameter
        where TVariable : TAsset<T>
    {
        public TVariable _Variable;
        public TVariable Variable { get => _Variable; }

        /// <summary>
        /// Initialize the blackboard value, getting the target's variable in the blackboard.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="Target">Target related using as key.</param>
        public override void Initialize(BlackBoardAsset board, GameObject Target)
        {
            _IsValid = false;
            if (!UseEntryName
                || EntryName == null
                || (EntryName.Value == null && !EntryName.UsingConstant)
                || EntryName.Value.Length == 0)
            {
                Debug.LogWarning(this + ": Failed to Initialize blackboard parameter from BlackBoard[\"" + board + "\"] and target[\"" + Target.name + "\"].");
                _Variable = ScriptableObject.CreateInstance<TVariable>();
                return;
            }
            _Variable = board.GetVariable<T, TVariable>(Target, EntryName);

            if (_Variable != null)
            {
                _IsValid = true;
            }
        }

        public static implicit operator T(BlackboardParameter_Template<T, TVariable> _object)
        {
            return _object._Variable.Value;
        }
    }

    [System.Serializable]
    public class BBP_Bool : BlackboardParameter_Template<bool, BoolAsset>
    { }
    [System.Serializable]
    public class BBP_Int : BlackboardParameter_Template<int, IntegerAsset>
    { }
    [System.Serializable]
    public class BBP_Float : BlackboardParameter_Template<float, FloatAsset>
    { }
    [System.Serializable]
    public class BBP_String : BlackboardParameter_Template<string, StringAsset>
    { }
    [System.Serializable]
    public class BBP_Vector2 : BlackboardParameter_Template<Vector2, Vector2Asset>
    { }
    [System.Serializable]
    public class BBP_Vector3 : BlackboardParameter_Template<Vector3, Vector3Asset>
    { }
    [System.Serializable]
    public class BBP_GameObject : BlackboardParameter_Template<GameObject, GameObjectAsset>
    { }
}