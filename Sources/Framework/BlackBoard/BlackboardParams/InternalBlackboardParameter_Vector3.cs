using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{
    [System.Serializable]
    public class InternalBlackboardParameter_Vector3 : InternalBlackboardParameter<Vector3, Vector3Asset, Vector3Variable>
    {
        //public override IClonable Clone()
        //{
        //    InternalBlackboardParameter_Vector3 obj = base.Clone() as InternalBlackboardParameter_Vector3;
        //    obj.Value = Value;
        //    return obj;
        //}
    }
}