using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{
    public class VariableCondition_String : VariableConditionTemplate<string, StringAsset, StringVariable>
    {
        public override bool IsConditionValid()
        {
            switch (CompareAsEqual)
            {
                case EConditionComparer.Equal:
                    return Alpha.Value == Beta;
                case EConditionComparer.NotEqual:
                    return Alpha.Value != Beta;
                default:
                    return false;
            }
        }
    }
}