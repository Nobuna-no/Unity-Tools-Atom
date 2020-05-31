using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{

    // Class that allow to get a variable reference to a blackboard of a defined target.
    [System.Serializable]
    public class BlackboardTargetParameter
    {
        public GameObject Target;
        public StringVariable EntryName;

        // Use to set the default value.
        public virtual void Initialize(BlackBoardAsset board)
        { }
    }

    public class BlackboardTargetParameter_Template<T, TVariable> : BlackboardTargetParameter
        where TVariable : TAsset<T>
    {
        //public T DefaultValue;
        [ReadOnly]
        public TVariable Variable;

        public override void Initialize(BlackBoardAsset board)
        {
            //board.SetValue(this, DefaultValue);
            Variable = board.GetVariable<T, TVariable>(this);
        }

        public static implicit operator T(BlackboardTargetParameter_Template<T, TVariable> _object)
        {
            return _object.Variable.Value;
        }
    }

    [System.Serializable]
    public class BBTP_Bool : BlackboardTargetParameter_Template<bool, BoolAsset>
    { }
    [System.Serializable]
    public class BBTP_Int : BlackboardTargetParameter_Template<int, IntegerAsset>
    { }
    [System.Serializable]
    public class BBTP_Float : BlackboardTargetParameter_Template<float, FloatAsset>
    { }
    [System.Serializable]
    public class BBTP_Vector2 : BlackboardTargetParameter_Template<Vector2, Vector2Asset>
    { }
    [System.Serializable]
    public class BBTP_Vector3 : BlackboardTargetParameter_Template<Vector3, Vector3Asset>
    { }
    [System.Serializable]
    public class BBTP_GameObject : BlackboardTargetParameter_Template<GameObject, GameObjectAsset>
    { }

}