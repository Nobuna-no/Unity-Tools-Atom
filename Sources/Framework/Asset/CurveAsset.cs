using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    [CreateAssetMenu(fileName = "[Curve]-", menuName = "Unity Tools/Atom/Asset/Curve")]
    public class CurveAsset : TAsset<AnimationCurve>
    {
        #region PUBLIC METHODS
        public override void SetValueFromString(string value)
        {
            Logger.Log(Logger.Type.Error, "GameObject asset can't be set from a string.", this);
        }
        #endregion
    }
}
