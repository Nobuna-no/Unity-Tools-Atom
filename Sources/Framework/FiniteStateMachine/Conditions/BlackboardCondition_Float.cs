using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    public class BlackboardCondition_Float : BlackboardConditionTemplate<float, FloatAsset, FloatVariable>
    {
        public override bool IsConditionValid()
        {
            switch (CompareAsEqual)
            {
                case EConditionComparer.Equal:
                    return Variable.Value == Beta.Value;
                case EConditionComparer.NotEqual:
                    return Variable.Value != Beta.Value;
                case EConditionComparer.MoreThan:
                    return Variable.Value > Beta;
                case EConditionComparer.MoreOrEqualThan:
                    return Variable.Value >= Beta;
                case EConditionComparer.LessThan:
                    return Variable.Value < Beta;
                case EConditionComparer.LessOrEqualThan:
                    return Variable.Value <= Beta;
                default:
                    return false;
            }
        }
    }
}