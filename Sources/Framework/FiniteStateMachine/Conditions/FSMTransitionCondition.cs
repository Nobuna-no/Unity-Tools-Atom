using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.ComponentModel;


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

#if UNITY_EDITOR
        public abstract void RefreshInternalValue();
#endif
    }

    public abstract class BlackboardConditionTemplate<T, TAsset, TVariable> : FSMTransitionCondition
        where TAsset : TAsset<T> 
        where TVariable : TVariable<T, TAsset>
    {
        public string Alpha;
        public TVariable Beta;

        protected TAsset Variable;

#if UNITY_EDITOR
        [HideInInspector]
        public BlackBoardAsset BlackBoard;
        [HideInInspector]
        public GameObject Target;
        [HideInInspector]
        public List<string> BlackboardEntries;
        [HideInInspector]
        public int SelectedEntry = 0;
#endif


        public override string ToString()
        {
            return "Condition[" + this.name + "]: (BlackboardValue[" + Alpha + "]){" + Variable.Value + "} is " + CompareAsEqual + " to {" + Beta.Value + "}?";
        }

        public override void Initialize(BlackBoardAsset blackboard, GameObject target)
        {
            BlackBoard = blackboard;
            Target = target;
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

#if UNITY_EDITOR
        public override void RefreshInternalValue()
        {
            if(BlackBoard == null || Target == null)
            {
                FiniteStateMachine fsm = GetComponentInParent<FiniteStateMachine>();
                BlackBoard = fsm.Blackboard;
                Target = fsm.BlackboardTarget;
            }
            var values = BlackBoard.GetAllBlackboardValueOfTarget(Target);
            if(values.ContainsKey(typeof(T)))
            {
                BlackboardEntries = values[typeof(T)];
            }
            else
            {
                BlackboardEntries = null;
            }
        }
#endif
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

#if UNITY_EDITOR
        public override void RefreshInternalValue()
        {
        }
#endif
    }
#endif
}