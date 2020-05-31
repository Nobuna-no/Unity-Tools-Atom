using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace UnityTools.Atom
{
    public class TVariable<T, TAssetType>
        where TAssetType : TAsset<T>
    {
        #region PARAMETERS
        [SerializeField, HideInInspector]
        protected bool _UseConstant = true;
        /// <summary>
        /// Is this TVariable making use of constant? If not, it use a referenced asset.
        /// </summary>
        public bool UsingConstant { get => _UseConstant; }

        /// <summary>
        /// Return the asset name if don't make use of constant. Otherwise return "'constant'".
        /// </summary>
        public string VariableName
        {
            get
            {
                return _UseConstant ? "`constant`" : Asset?.name;
            }
        }

        public T ConstantValue;
        public TAssetType Asset;
        public T Value
        {
            get => GetValue();
            set
            {
                OnValueChanged?.Invoke();
                SetValue(value);
            }
        }

        /// <summary>
        /// Raise each time @Value is modified.
        /// </summary>
        public UnityEvent OnValueChanged;
        #endregion


        #region OPERATOR
        public static implicit operator T(TVariable<T, TAssetType> obj)
        {
            return obj.Value;
        }
        #endregion


        #region PUBLIC METHODS
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Is Valid to be set? Return false if Variable is set as Asset and asset is null.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return _UseConstant ? true : Asset != null;
        }

        /// <summary>
        /// @to do: documentation.
        /// </summary>
        /// <param name="refAsset">@to do: documentation.</param>
        public void SeTAssetType(TAssetType refAsset)
        {
            if (refAsset)
            {
                Asset = refAsset;
                _UseConstant = false;
            }
        }

        /// <summary>
        /// Add an event to the OnValueChanged callback of the asset (only for non constant usage).
        /// </summary>
        /// <param name="action">The method or action that will listen the event.</param>
        public void AddActionOnValueChanged(UnityAction action)
        {
            if (!_UseConstant)
            {
                Asset.OnValueChanged.AddListener(action);
            }
        }
        #endregion


        #region PROTECTED METHODS
        /// <summary>
        /// Virtual method use in the get @Value property.
        /// </summary>
        /// <returns></returns>
        protected virtual T GetValue()
        {
            if (_UseConstant)
            {
                return ConstantValue;
            }
            else
            {
                if (!Asset)
                {
                    Logger.Log(Logger.Type.Warning, "<" + VariableName + ">: Is set to use variable ref but as a null ref !Returning constant value!");
                    return ConstantValue;
                }
                return Asset.Value;
            }
        }

        /// <summary>
        /// Virtual method use in the set @Value property.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void SetValue(T value)
        {
            if (_UseConstant)
            {
                ConstantValue = value;
            }
            else if (Asset)
            {
                Asset.Value = value;
            }
        }
        #endregion
    }
}