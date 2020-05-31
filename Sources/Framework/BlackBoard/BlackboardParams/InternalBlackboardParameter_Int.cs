using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    public class InternalBlackboardParameter_Int : InternalBlackboardParameter<IntegerAsset>
    {
        public override IClonable Clone()
        {
            InternalBlackboardParameter_Int obj = base.Clone() as InternalBlackboardParameter_Int;
            obj.Value = Value;
            return obj;
        }
    }
}