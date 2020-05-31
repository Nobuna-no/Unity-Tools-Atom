using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{
    [System.Serializable]
    public class InternalBlackboardParameter_Transform : InternalBlackboardParameter<Transform, TransformAsset, TransformVariable>
    {
        //public override IClonable Clone()
        //{
        //    InternalBlackboardParameter_Transform obj = base.Clone() as InternalBlackboardParameter_Transform;
        //    obj.Value = Value;
        //    return obj;
        //}
    }
}