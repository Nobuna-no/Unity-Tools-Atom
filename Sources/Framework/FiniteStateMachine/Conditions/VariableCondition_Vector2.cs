﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    public class VariableCondition_Vector2 : VariableConditionTemplate<Vector2, Vector2Asset, Vector2Variable>
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
                    return Alpha.Value.sqrMagnitude > Beta.Value.sqrMagnitude;
                case EConditionComparer.MoreOrEqualThan:
                    return Alpha.Value.sqrMagnitude >= Beta.Value.sqrMagnitude;
                case EConditionComparer.LessThan:
                    return Alpha.Value.sqrMagnitude < Beta.Value.sqrMagnitude;
                case EConditionComparer.LessOrEqualThan:
                    return Alpha.Value.sqrMagnitude <= Beta.Value.sqrMagnitude;
                default:
                    return false;
            }
        }
    }
}