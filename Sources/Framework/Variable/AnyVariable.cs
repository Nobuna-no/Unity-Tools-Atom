using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    [System.Serializable]
    public class AnyVariable : TVariable<string, StringAsset>
    {
        #region PARAMETERS
        public IAsset AnyAsset;
        #endregion

        #region PROTECTED METHODS
        protected override string GetValue()
        {
            if (_UseConstant)
            {
                return ConstantValue;
            }
            else
            {
                if (!AnyAsset)
                {
                    Logger.Log(Logger.Type.Warning, "<" + VariableName + ">: Is set to use variable ref but as a null ref !Returning constant value!");
                    return ConstantValue;
                }
                return AnyAsset.ToString();
            }
        }

        protected override void SetValue(string value)
        {
            if (_UseConstant)
            {
                ConstantValue = value;
            }
            else
            {
                AnyAsset.SetValueFromString(value);
            }
        }
        #endregion
    }
}
