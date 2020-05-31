using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    [CreateAssetMenu(fileName = "[Transform]-", menuName = "Unity Tools/Atom/Asset/Transform")]
    public class TransformAsset : TAsset<Transform>
    {
        #region PUBLIC METHODS
        public override void SetValueFromString(string value)
        {
            Logger.Log(Logger.Type.Error, "Transform asset can't be set from a string.", this);

        }
        #endregion
    }
}
