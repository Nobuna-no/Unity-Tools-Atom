using UnityEngine;
using UnityTools.Atom;

namespace UnityTools.Atom
{
    /* OPT VER.*/
    [System.Serializable, CreateAssetMenu(menuName = "Unity Tools/Atom/Utility/Input/Axis", order = 4)]
    public class DATA_Axis : ScriptableObject, ISerializationCallbackReceiver
    {
        public Vector2 InitialValue;
        public FloatVariable XVariable;
        public FloatVariable YVariable;

        public FloatVariable TimestampX;
        public FloatVariable TimestampY;

        /// <summary>
        /// Force value to 0 and reset timestamp. 
        /// </summary>
        public void ConsumeAxisX()
        {
            TimestampX.Value = 0;
        }

        /// <summary>
        /// Force value to 0 and reset timestamp. 
        /// </summary>
        public void ConsumeAxisY()
        {
            TimestampY.Value = 0;
        }

        public Vector2 Value
        {
            get { return new Vector2(XVariable, YVariable); }
            set
            {
                if (XVariable.Value != value.x)
                {
                    XVariable.Value = value.x;
                }
                if (YVariable.Value != value.y)
                {
                    YVariable.Value = value.y;
                }
            }
        }

        public static implicit operator Vector2(DATA_Axis _object)
        {
            return new Vector2(_object.XVariable, _object.YVariable);
        }

        public virtual void OnEnable()
        {
            if (XVariable.IsValid())
            {
                XVariable.ResetActionOnValueChanged();
            }
            if (YVariable.IsValid())
            {
                YVariable.ResetActionOnValueChanged();
            }
        }

        public virtual void OnDisable()
        {
            if (Application.isPlaying)
            {
                if (XVariable.IsValid())
                {
                    XVariable.ResetActionOnValueChanged();
                    XVariable.Value = InitialValue.x;
                }
                if (YVariable.IsValid())
                {
                    YVariable.ResetActionOnValueChanged();
                    YVariable.Value = InitialValue.y;
                }
            }
        }

        public void OnBeforeSerialize()
        {
            //Value = InitialValue;
        }

        public void OnAfterDeserialize()
        {
            if (!XVariable.IsValid())
            {
                XVariable.Value = InitialValue.x;
            }
            if (!YVariable.IsValid())
            {
                YVariable.Value = InitialValue.y;
            }
        }
    }
}