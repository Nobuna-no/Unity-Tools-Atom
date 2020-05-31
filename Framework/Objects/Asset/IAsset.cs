using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


namespace UnityTools.Atom
{
    [System.Serializable]
    public class IAsset : ScriptableObject
    {
        #region PARAMETER
        public UnityEvent OnValueChanged;
        #endregion

        #region PUBLIC METHODS
        public override string ToString()
        {
            return this.ToString();
        }

        public virtual void SetValueFromString(string value)
        { }
        #endregion
    }
}