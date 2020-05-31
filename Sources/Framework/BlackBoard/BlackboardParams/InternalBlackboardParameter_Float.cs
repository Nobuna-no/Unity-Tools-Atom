using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{
    [System.Serializable]
    public class InternalBlackboardParameter_Float : InternalBlackboardParameter<FloatAsset>
    {
        public override IClonable Clone()
        {
            InternalBlackboardParameter_Float obj = base.Clone() as InternalBlackboardParameter_Float;
            obj.Value = Value;
            return obj;
        }
    }
}