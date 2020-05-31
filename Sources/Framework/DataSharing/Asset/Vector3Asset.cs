using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    [CreateAssetMenu(fileName = "[Vector3]-", menuName = "Unity Tools/Atom/Asset/Vector3")]
    public class Vector3Asset : TAsset<Vector3>
    {
        #region PUBLIC METHODS
        public override void SetValueFromString(string value)
        {
            Vector3 val = Vector3.zero;
            float buff = 0f;

            int startInd = value.IndexOf("X:") + 2;
            if(float.TryParse(value.Substring(startInd, value.IndexOf(" Y") - startInd), out buff))
            {
                val.x = buff;
            }
            else
            {
                Logger.Log(Logger.Type.Error, "Failed to parse X value string to Vector3.", this);
                return;
            }

            startInd = value.IndexOf("Y:") + 2;
            if(float.TryParse(value.Substring(startInd, value.IndexOf("Z") - startInd), out buff))
            {
                val.y = buff;
            }
            else
            {
                Logger.Log(Logger.Type.Error, "Failed to parse Y value string to Vector3.", this);
                return;
            }

            startInd = value.IndexOf("Z:") + 2;
            if (float.TryParse(value.Substring(startInd, value.IndexOf("}") - startInd), out buff))
            {
                val.z = buff;
            }
            else
            {
                Logger.Log(Logger.Type.Error, "Failed to parse Z value from string to Vector3.", this);
                return;
            }

            SetValue(val);
        }
        #endregion
    }
}
