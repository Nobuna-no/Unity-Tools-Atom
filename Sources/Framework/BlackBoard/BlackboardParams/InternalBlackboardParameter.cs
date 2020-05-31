
using UnityEngine;

namespace UnityTools.Atom
{

    public class InternalBlackboardParameter : ScriptableObject, IClonable
    {
        public StringVariable Name = new StringVariable();
        public string Description = "Enter a description...";
        [HideInInspector]
        public bool MustBeShared;

#if UNITY_EDITOR
        [HideInInspector]
        public bool ShowInBlackBoard = false;
#endif

        public virtual IClonable Clone()
        {
            InternalBlackboardParameter obj = CreateInstance(GetType()) as InternalBlackboardParameter;
            obj.Name = Name;
            obj.Description = Description;
            obj.MustBeShared = MustBeShared;
            return obj;
        }

        public virtual void Init()
        { }

        public virtual void Close()
        { }
    }

    public class InternalBlackboardParameter<T, TAsset, TVariable> : InternalBlackboardParameter
        where TAsset : TAsset<T>
        where TVariable : TVariable<T,TAsset>
    {
        public TAsset Value;
        public TVariable DefaultValue;

        public override string ToString()
        {
            return "{" + Value + "}";
        }

        public override IClonable Clone()
        {
            InternalBlackboardParameter<T, TAsset, TVariable> obj = base.Clone() as InternalBlackboardParameter<T, TAsset, TVariable>;
            if (obj)
            {
                obj.Value = Value;
                obj.DefaultValue = DefaultValue;
                return obj;
            }
            return null;
        }

        public override void Init()
        {
            if (!MustBeShared
                || (MustBeShared && Value == null))
            {
                Value = ScriptableObject.CreateInstance<TAsset>();
            }

            Value.Value = DefaultValue.Value;
        }

        public override void Close()
        {
            if (!MustBeShared && Value != null)
            {
                Value = null;
            }
        }
    }

}