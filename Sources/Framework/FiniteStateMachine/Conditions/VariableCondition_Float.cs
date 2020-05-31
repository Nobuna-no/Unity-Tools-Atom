using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    public class VariableCondition_Float : VariableConditionTemplate<float, FloatAsset, FloatVariable>
    {
        public override bool IsConditionValid()
        {
            switch (CompareAsEqual)
            {
                case EConditionComparer.Equal:
                    return Alpha.Value == Beta.Value;
                case EConditionComparer.NotEqual:
                    return Alpha.Value != Beta.Value;
                case EConditionComparer.MoreThan:
                    return Alpha > Beta;
                case EConditionComparer.MoreOrEqualThan:
                    return Alpha >= Beta;
                case EConditionComparer.LessThan:
                    return Alpha < Beta;
                case EConditionComparer.LessOrEqualThan:
                    return Alpha <= Beta;
                default:
                    return false;
            }
        }
    }
}