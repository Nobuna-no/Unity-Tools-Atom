using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    [CreateAssetMenu(fileName = "[Vector2]-", menuName = "Unity Tools/Atom/Asset/Vector2")]
    public class Vector2Asset : TAsset<Vector2>
    {
        #region PUBLIC METHODS
        public override void SetValueFromString(string value)
        {
            Vector2 val = Vector2.zero;
            float buff = 0f;

            int startInd = value.IndexOf("X:") + 2;
            if (float.TryParse(value.Substring(startInd, value.IndexOf(" Y") - startInd), out buff))
            {
                val.x = buff;
            }
            else
            {
                Logger.Log(Logger.Type.Error, "Failed to parse X value string to Vector2.", this);
                return;
            }

            startInd = value.IndexOf("Y:") + 2;
            if (float.TryParse(value.Substring(startInd, value.IndexOf("}") - startInd), out buff))
            {
                val.y = buff;
            }
            else
            {
                Logger.Log(Logger.Type.Error, "Failed to parse Y value string to Vector2.", this);
                return;
            }

            SetValue(val);
        }
        #endregion
    }
}
