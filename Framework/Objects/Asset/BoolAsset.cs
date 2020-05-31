using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    [CreateAssetMenu(fileName = "[Bool]-", menuName = "Unity Tools/Atom/Asset/Boolean")]
    public class BoolAsset : TAsset<bool>
    {
        #region PUBLIC METHODS
        public override void SetValueFromString(string value)
        {
            bool val = false;
            if (bool.TryParse(value, out val))
            {
                SetValue(val);
            }
        }
        #endregion
    }
}
