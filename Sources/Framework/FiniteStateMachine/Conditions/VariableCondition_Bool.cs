using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{
    public class VariableCondition_Bool : VariableConditionTemplate<bool, BoolAsset, BoolVariable>
    {
        public override bool IsConditionValid()
        {
            switch (CompareAsEqual)
            {
                case EConditionComparer.Equal:
                    return Alpha.Value == Beta.Value;
                case EConditionComparer.NotEqual:
                    return Alpha.Value != Beta.Value;
                default:
                    return false;
            }
        }
    }
}