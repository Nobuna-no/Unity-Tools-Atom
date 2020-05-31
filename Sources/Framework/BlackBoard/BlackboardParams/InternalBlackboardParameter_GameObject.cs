using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    public class InternalBlackboardParameter_GameObject : InternalBlackboardParameter<GameObjectAsset>
    {
        public override IClonable Clone()
        {
            InternalBlackboardParameter_GameObject obj = base.Clone() as InternalBlackboardParameter_GameObject;
            obj.Value = Value;
            return obj;
        }
    }
}