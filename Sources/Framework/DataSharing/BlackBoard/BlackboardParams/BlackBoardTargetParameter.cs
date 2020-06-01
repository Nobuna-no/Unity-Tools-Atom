using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{

    // Class that allow to get a variable reference to a blackboard of a defined target.
    [System.Serializable]
    public abstract class BlackboardTargetParameter
    {
        public GameObject Target;
        public StringVariable EntryName;
        public BlackBoardAsset BlackBoard;
    }

    public class BlackboardTargetParameter_Template<T, TAsset, TVariable> : BlackboardTargetParameter
        where TAsset : TAsset<T>
        where TVariable : TVariable<T, TAsset>
    {
        [SerializeField]
        protected TAsset Variable;
        public T Value
        {
            get 
            {
                if (Variable == null)
                {
                    Variable = BlackBoard.GetVariable<T, TAsset, TVariable>(this);
                }

                return Variable.Value;
            }
            set 
            {
                if (Variable == null)
                {
                    Variable = BlackBoard.GetVariable<T, TAsset, TVariable>(this);
                }

                Variable.Value = value;
            }
        }
    }

    [System.Serializable]
    public class BBTP_Bool : BlackboardTargetParameter_Template<bool, BoolAsset, BoolVariable>
    { }
    [System.Serializable]
    public class BBTP_Int : BlackboardTargetParameter_Template<int, IntegerAsset, IntegerVariable>
    { }
    [System.Serializable]
    public class BBTP_Float : BlackboardTargetParameter_Template<float, FloatAsset, FloatVariable>
    { }
    [System.Serializable]
    public class BBTP_Vector2 : BlackboardTargetParameter_Template<Vector2, Vector2Asset, Vector2Variable>
    { }
    [System.Serializable]
    public class BBTP_Vector3 : BlackboardTargetParameter_Template<Vector3, Vector3Asset, Vector3Variable>
    { }
    [System.Serializable]
    public class BBTP_GameObject : BlackboardTargetParameter_Template<GameObject, GameObjectAsset, GameObjectVariable>
    { }
    [System.Serializable]
    public class BBTP_Transform : BlackboardTargetParameter_Template<Transform, TransformAsset, TransformVariable>
    { }
}