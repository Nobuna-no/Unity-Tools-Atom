using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    [CreateAssetMenu(fileName = "[Int]-", menuName = "Unity Tools/Atom/Asset/Integer")]
    public class IntegerAsset : TAsset<int>
    {
        #region PUBLIC METHODS
        public override void SetValueFromString(string value)
        {
            int val = 0;
            if (int.TryParse(value, out val))
            {
                SetValue(val);
            }
        }
        #endregion
    }
}