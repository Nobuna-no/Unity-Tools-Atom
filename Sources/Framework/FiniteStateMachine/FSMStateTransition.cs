using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityTools.Atom
{

    [ExecuteInEditMode]
    public class FSMStateTransition : MonoBehaviour_Utility
    {
        #region PROPERTIES
        [Header(".FSM STATE/Info")]
#if UNITY_EDITOR
        [SerializeField, TextArea(1, 10)]
        private string _Log = "";
#endif

        [SerializeField, ReadOnly]
        private int ValidationTimes = 0;

        [HideInInspector, SerializeField]
        private FSMState _CurrentState = null;

        [HideInInspector, SerializeField]
        private FSMState _NextState = null;

        [HideInInspector]
        public List<FSMTransitionCondition> Conditions = new List<FSMTransitionCondition>();

#if UNITY_EDITOR
        [HideInInspector, SerializeField]
        private bool _CurrentlySubscribed = false;

        [HideInInspector]
        public bool BlockTransition = false;
#endif

        private bool _NeedInitialization = true;
        #endregion


        #region UNITY METHODS
        private void Awake()
        {
            _NeedInitialization = true;
        }

        private void OnDisable()
        {
            _NeedInitialization = true;
        }
        #endregion


        #region PUBLIC METHODS
        public void Init()
        {
            _CurrentState = GetComponentInParent<FSMState>();
            if (_CurrentState == null)
            {
                Verbose(VerboseMask.WarningLog, "There is no FSMState found in " + this + "'s parent.");
                return;
            }

            FiniteStateMachine FSM = _CurrentState.GetComponentInParent<FiniteStateMachine>();
            if (FSM == null)
            {
                Verbose(VerboseMask.WarningLog, "There is no FiniteStateMachine found in " + this + "'s parent.");
                return;
            }

            if (FSM.Blackboard == null)
            {
                Verbose(VerboseMask.WarningLog, "Invalid Blackboard in FSM '" + FSM.gameObject + "'.");
                return;
            }

            for (int i = 0; i < Conditions.Count; i++)
            {
                Conditions[i].Initialize(FSM.Blackboard, FSM.BlackboardTarget);
            }
        }

        public bool Subscribe()
        {
            _Validated = false;
            if (_NeedInitialization)
            {
#if UNITY_EDITOR
                if (HasFlag(VerboseMask.Log))
                {
                    InternalLog("Initialization.");
                }
#endif
                Init();
                _NeedInitialization = false;
            }

            for (int i = 0; i < Conditions.Count; i++)
            {
#if UNITY_EDITOR
                if (HasFlag(VerboseMask.Log))
                {
                    InternalLog("Subscribing [" + Conditions[i].ToString() + "].");
                }
#endif
                Conditions[i].Subscribe(ComputeCondition);
            }

            _CurrentlySubscribed = true;
            ComputeCondition();
            return _Validated;
        }

        public void UnSubscribe()
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
#if UNITY_EDITOR
                if (HasFlag(VerboseMask.Log))
                {
                    InternalLog("UnSubscribing [" + Conditions[i].name + "].");
                }
#endif
                Conditions[i].UnSubscribe(ComputeCondition);
            }
            _CurrentlySubscribed = false;
        }

        private bool _Validated = false;
        public bool Validated { get => _Validated; }

        public void ComputeCondition()
        {

#if UNITY_EDITOR
            if(BlockTransition)
            {
                return;
            }
#endif

            _Validated = false;
            if (_NextState == null)
            {
                BlockTransition = true;
                Logger.Log( Logger.Type.Error, this + ": Next State is null! It can't be for a StateTransition! Transistion disabled.", this);
                return;
            }
            bool condition = true;

            for (int i = 0; i < Conditions.Count; i++)
            {
                try
                {
                    condition &= Conditions[i].IsConditionValid();
                }
                catch
                {
                    Logger.Log(Logger.Type.Error, "Invalid condition triggered! Transistion disabled.", this);
                }

#if UNITY_EDITOR
                if (HasFlag(VerboseMask.Log))
                {
                    InternalLog("(" + i + ") " + Conditions[i].ToString() + " => " + condition);
                }
#endif
                if (!condition)
                {
                    return;
                }
            }


            if (condition)
            {
#if UNITY_EDITOR
                if (HasFlag(VerboseMask.Log))
                {
                    InternalLog("Conditions are validated! Passing to new states: " + _NextState);
                }
#endif
                //FiniteStateMachine FSM = _CurrentState.GetComponentInParent<FiniteStateMachine>();
                _Validated = true;
                ValidationTimes++;
                _CurrentState.SetState(_NextState);
            }
        }

#endregion


#region PRIVATE METHODS
        private void InternalLog(string message)
        {
            _Log += "\n" + Time.timeSinceLevelLoad.ToString("0:00") + "> " + message;
        }
#endregion
    }
}