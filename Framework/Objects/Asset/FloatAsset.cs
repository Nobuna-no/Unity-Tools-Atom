using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    [CreateAssetMenu(fileName = "[Float]-", menuName = "Unity Tools/Atom/Asset/Float")]
    public class FloatAsset : TAsset<float>
    {
        #region PUBLIC METHODS
        public override void SetValueFromString(string value)
        {
            float val = 0;
            if (float.TryParse(value, out val))
            {
                SetValue(val);
            }
        }
        #endregion
    }
}