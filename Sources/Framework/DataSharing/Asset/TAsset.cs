using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{
    public class TAsset<T> : IAsset
    {
        #region PARAMETERS
        [Header("TA Asset - Value")]
        [SerializeField]
        protected T _Value;
        public T Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
                OnValueChanged?.Invoke();
            }
        }
        #endregion


        #region PUBLIC METHODS
        public override string ToString()
        {
            return Value.ToString();
        }
        #endregion


        #region PROTECTED METHODS
        protected virtual T GetValue()
        {
            return _Value;
        }

        protected virtual void SetValue(T value)
        {
            _Value = value;
        }
        #endregion
    }
}