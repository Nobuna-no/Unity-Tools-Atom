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
        protected UnityEvent OnValueChanged;
        #endregion

        #region PUBLIC METHODS
        public override string ToString()
        {
            return this.ToString();
        }

        public virtual void SetValueFromString(string value)
        { }

        public void AddListenerOnValueChanged(UnityAction unityAction)
        {
            if(OnValueChanged == null)
            {
                OnValueChanged = new UnityEvent();
            }
            OnValueChanged.AddListener(unityAction);
        }
        public void RemoveListenerOnValueChanged(UnityAction unityAction)
        {
            if (OnValueChanged == null)
            {
                OnValueChanged = new UnityEvent();
            }
            OnValueChanged.RemoveListener(unityAction);
        }
        public void ResetOnValueChanged()
        {
            OnValueChanged.RemoveAllListeners();
        }
        #endregion
    }
}