using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{
    [System.Serializable]
    public class FloatVariable : TVariable<float, FloatAsset>
    {
        public FloatVariable(float initialValue)
        {
            Value = initialValue;
        }
    }
}