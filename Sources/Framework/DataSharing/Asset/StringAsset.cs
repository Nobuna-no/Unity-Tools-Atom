using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    [CreateAssetMenu(fileName = "[String]-", menuName = "Unity Tools/Atom/Asset/String")]
    public class StringAsset : TAsset<string>
    {
        #region PARAMETERS
        [Header("String Asset - NERVER ASSIGNED THE ASSET ON ITSELF!")]
        [SerializeField]
        protected List<AnyVariable> Sentences = new List<AnyVariable>();
        #endregion

        #region PUBLIC METHODS
        public override string ToString()
        {
            return Value;
        }

        public override void SetValueFromString(string value)
        {
            SetValue(value);
        }
        #endregion


        #region PROTECTED METHODS
        protected override string GetValue()
        {
            if (Sentences != null && Sentences.Count > 0)
            {
                _Value = "";

                foreach (AnyVariable str in Sentences)
                {
                    _Value += str.Value;
                }

            }
            return _Value;
        }

        protected override void SetValue(string value)
        {
            if (Sentences != null && Sentences.Count > 0)
            {
                Sentences.Clear();
            }

            _Value = value;
        }
        #endregion
    }
}