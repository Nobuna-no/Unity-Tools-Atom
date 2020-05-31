using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    public class BlackboardCondition_String : BlackboardConditionTemplate<string, StringAsset, StringVariable>
    {
        public override bool IsConditionValid()
        {
            switch (CompareAsEqual)
            {
                case EConditionComparer.Equal:
                    return Variable.Value == Beta.Value;
                case EConditionComparer.NotEqual:
                    return Variable.Value != Beta.Value;
                default:
                    return false;
            }
        }
    }
}