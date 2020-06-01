using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{

    [ExecuteInEditMode]
    public class FiniteStateMachine : FSMState
    {
        #region PROPERTIES
        [Header(".FSM/Settings")]

        //private BlackBoardAsset BlackBoard;
        [SerializeField]
        private BlackBoardAsset _Blackboard;
        public BlackBoardAsset Blackboard { get => _Blackboard; }
        [SerializeField]
        private GameObject _BlackboardTarget;
        public GameObject BlackboardTarget { get => _BlackboardTarget; }

        // Entry or current state.
        [SerializeField]
        private FSMState _InitialState = null;

        [SerializeField]
        private FloatVariable _UpdateTickInSecond = new FloatVariable(0.01f);

        [Header(".FSM/Infos")]
        [SerializeField, ReadOnly]
        private FSMState _CurrentState = null;
        [SerializeField, ReadOnly]
        private bool IsPrimaryFSM = false;

        [ReadOnly, TextArea, SerializeField]
        private string FSMStatus;

#if UNITY_EDITOR
        [SerializeField, ReadOnly]
        private bool _UpdateSignal = false;
#endif

        [SerializeField, ReadOnly]
        private float _RuntimeDeltaTime = 0f;
        private Coroutine _UpdateCrt;
        private float _LastTime = 0f;
        #endregion


        #region UNITY METHODS
        protected override void Start()
        {
            // We need to cancel basic FSMState Start().
        }

        protected override void OnEnable()
        {
            FiniteStateMachine[] parents = GetComponentsInParent<FiniteStateMachine>(false);
            // if there is no FSM parent, it means that this FSM is the primary one.
            if (parents.Length == 1) // If only itself...
            {
                if (_Description != null && !_Description.Contains("<Primary State Machine>"))
                {
                    _Description = "<Primary State Machine>\n" + _Description;
                }
                FSMStatus = this.name + " >>";

                IsPrimaryFSM = true;

                if (Application.isPlaying)
                {
                    if (!_Blackboard)
                    {
                        Debug.Break();
                        Verbose(VerboseMask.ErrorLog, this + ": No blackboard set in the primary FSM!");
                    }
                    if (!_BlackboardTarget)
                    {
                        Verbose(VerboseMask.WarningLog, this + ": No blackboard target set in the primary FSM. Taking own gameobject as target!");
                        _BlackboardTarget = this.gameObject;
                    }
                    // And so, auto launch it.
                    base.Initialize(_Blackboard, _BlackboardTarget);
                    Enter();
                }
                else
                {
                    if (!_Blackboard)
                    {
                        Debug.Break();
                        Verbose(VerboseMask.WarningLog, this + ": No blackboard set in the primary FSM!");
                    }
                    if (!_BlackboardTarget)
                    {
                        Verbose(VerboseMask.WarningLog, this + ": No blackboard target set in the primary FSM. Taking own gameobject as target!");
                        _BlackboardTarget = this.gameObject;
                    }

                    if(_Blackboard && _BlackboardTarget)
                    {
                        _Blackboard.InitializeTarget(_BlackboardTarget);
                    }
                }
            }
            else
            {
                string str = "";
                for (int i = parents.Length - 1; i > 0; --i)
                {
                    str += parents[i].name + " > ";
                }
                FSMStatus = str + this.name;

                if (_Description != null && !_Description.Contains("<SubState>"))
                {
                    _Description = "<SubState>\n" + _Description;
                }
                IsPrimaryFSM = false;

                FiniteStateMachine FSM = parents[parents.Length - 1];
                _SubFSMOwner = FSM;
                //if (!Application.isPlaying)
                //{
                Initialize(FSM.Blackboard, FSM.BlackboardTarget);
                //}
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Exit();
        }
        #endregion


        #region FSMSTATE METHODS
        public override void Initialize(BlackBoardAsset blackBoard, GameObject target)
        {
            if (!blackBoard || !target)
            {
                Debug.Break();
                Verbose(VerboseMask.ErrorLog, this + ": No blackboard or No blackboard target set in the primary FSM!");
            }

            _Blackboard = blackBoard;
            _BlackboardTarget = target;

            base.Initialize(blackBoard, target);
        }


        public override bool Enter()
        {
            if (!base.Enter())
            {
                return false;
            }

            _CurrentState = _InitialState;

            if (_CurrentState.NeedInitialization)
            {
                _CurrentState?.Initialize(_Blackboard, _BlackboardTarget);
            }
            _CurrentState?.Enter();


            _UpdateCrt = StartCoroutine(Update_Coroutine());

            return true;
        }

        public override void Exit()
        {
            _CurrentState?.Exit();
            _CurrentState = null;

            if (_UpdateCrt != null)
            {
                StopCoroutine(_UpdateCrt);
                _UpdateCrt = null;
            }

            base.Exit();
        }

        public override void EnableBooleanBlackboardValue(StringVariable entryName)
        {
            if (entryName != null)
            {
                Verbose(VerboseMask.Log, "Enable \"" + entryName + "\" blackboard value!");
                _Blackboard.SetValue<bool, BoolAsset, BoolVariable>(_BlackboardTarget, entryName, true);
            }
        }

        public override void DisableBooleanBlackboardValue(StringVariable entryName)
        {
            if (entryName != null)
            {
                Verbose(VerboseMask.Log, "Disable \"" + entryName + "\" blackboard value!");
                _Blackboard.SetValue<bool, BoolAsset, BoolVariable>(_BlackboardTarget, entryName, false);
            }
        }
        #endregion


        #region PUBLIC METHODS
        public override void SetState(FSMState state)
        {
            if (!IsPrimaryFSM)
            {
                // if current subFSM is state's parent.
                if (state.SubFSMOwner == this)
                {
                    _CurrentState?.Exit();
                    _CurrentState = state;
                    _CurrentState.Enter();
                    return;
                }

                FiniteStateMachine SubFSM = state as FiniteStateMachine;
                if (SubFSM != null)
                {
                    if (!SubFSM.IsPrimaryFSM)
                    {
                        // If same FSM parent.
                        if (SubFSM.SubFSMOwner == _SubFSMOwner)
                        {
                            _SubFSMOwner.SetState(state);
                        }
                        // if not.
                        else
                        {
                            Verbose(VerboseMask.ErrorLog, "Trying to set a substate machine(" + state.SubFSMOwner.name + ") from an invalid one. You must transit substates layer by layer (" + state.SubFSMOwner.FSMStatus + ").");
                        }
                    }
                    // if current state is a superior FSM
                    else if (SubFSM == _SubFSMOwner)
                    {
                        Exit();
                        state.Enter();
                    }
                    return;
                }
            }

            if (state.SubFSMOwner == this)
            {
                _CurrentState?.Exit();
                _CurrentState = state;
                _CurrentState.Enter();
            }
            else if (state.SubFSMOwner != null)
            {
                state.SubFSMOwner.SetState(state);
            }
            else
            {
                Verbose(VerboseMask.ErrorLog, "State hasn't a valid SubFSM and " + this + " isn't either a valid one. What did you do?");
            }
        }

        public void ExitSubStateMachine()
        {
            if (!IsPrimaryFSM)
            {
                Exit();
                FiniteStateMachine[] parents = GetComponentsInParent<FiniteStateMachine>(false);
                parents[1].Enter();
            }
            else
            {
                Verbose(VerboseMask.WarningLog, "Trying to exit primary state machine. It can't be.");
            }
        }
        #endregion


        #region COROUTINE
        private IEnumerator Update_Coroutine()
        {
            while (true)
            {
                _LastTime = Time.realtimeSinceStartup;
                yield return new WaitForSeconds(_UpdateTickInSecond);

                _RuntimeDeltaTime = Time.realtimeSinceStartup - _LastTime;

#if UNITY_EDITOR
                _UpdateSignal = !_UpdateSignal;
#endif

                base.UpdateState(_RuntimeDeltaTime);
                _CurrentState?.UpdateState(_RuntimeDeltaTime);
            }
        }
        #endregion
    }
}