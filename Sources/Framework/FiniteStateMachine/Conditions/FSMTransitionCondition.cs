using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

namespace UnityTools.Atom
{
    public abstract class FSMTransitionCondition : MonoBehaviour
    {
        public enum EConditionComparer
        {
            Equal = 0,
            NotEqual,
            MoreThan,
            MoreOrEqualThan,
            LessThan,
            LessOrEqualThan
        }

        public EConditionComparer CompareAsEqual;

        public virtual bool IsConditionValid()
        {
            return false;
        }

        public virtual void Initialize(BlackBoardAsset blackboard, GameObject target)
        { }

        public virtual void Subscribe(UnityAction unityAction)
        { }

        public virtual void UnSubscribe(UnityAction unityAction)
        { }
    }

    public abstract class BlackboardConditionTemplate<T, TAsset, TVariable> : FSMTransitionCondition
        where TAsset : TAsset<T> 
        where TVariable : TVariable<T, TAsset>
    {
        public StringVariable Alpha;
        public TVariable Beta;

        protected TAsset Variable;

        public override string ToString()
        {
            return "Condition[" + this.name + "]: (BlackboardValue[" + Alpha.Value + "]){" + Variable.Value + "} is " + CompareAsEqual + " to {" + Beta.Value + "}?";
        }

        public override void Initialize(BlackBoardAsset blackboard, GameObject target)
        {
            Variable = blackboard.GetVariable<T, TAsset, TVariable>(target, Alpha);
        }

        public override void Subscribe(UnityAction unityAction)
        {
            if (Variable != null)
            {
                Variable.AddListenerOnValueChanged(unityAction);
            }
            if (Beta != null && Beta.IsValid())
            {
                Beta.AddActionOnValueChanged(unityAction);
            }
        }

        public override void UnSubscribe(UnityAction unityAction)
        {
            if (Variable != null)
            {
                Variable.RemoveListenerOnValueChanged(unityAction);
            }
            if (Beta != null && Beta.IsValid())
            {
                Beta.RemoveActionOnValueChanged(unityAction);
            }
        }
    }

    public abstract class VariableConditionTemplate<T, TAsset, ReferenceT> : FSMTransitionCondition
        where TAsset : TAsset<T> where ReferenceT : TVariable<T, TAsset>
    {
        public ReferenceT Alpha;
        public ReferenceT Beta;

        public override string ToString()
        {
            return "Condition[" + this.name + "]: \"" + Alpha.VariableName + "\"{" + Alpha.Value + "} is " + CompareAsEqual + " to \"" + Beta.VariableName + "\"{" + Beta.Value + "}?";
        }

        public override void Subscribe(UnityAction unityAction)
        {
            if (Alpha != null && Alpha.IsValid())
            {
                Alpha.AddActionOnValueChanged(unityAction);
            }
            if (Beta != null && Beta.IsValid())
            {
                Beta.AddActionOnValueChanged(unityAction);
            }
        }

        public override void UnSubscribe(UnityAction unityAction)
        {
            if (Alpha != null && Alpha.IsValid())
            {
                Alpha.RemoveActionOnValueChanged(unityAction);
            }
            if (Beta != null && Beta.IsValid())
            {
                Beta.RemoveActionOnValueChanged(unityAction);
            }
        }
    }
#endif
}