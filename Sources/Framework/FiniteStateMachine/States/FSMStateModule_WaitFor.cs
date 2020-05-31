﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityTools.Atom;


public class FSMStateModule_WaitFor : FSMStateModule
{
    #region TYPES
   
    [System.Serializable]
    public class Attributes
    {
        public MinMaxRange DurationInSecond = new MinMaxRange(0, 10);
        public bool UseSecondsRealtime = false;
    }
    #endregion


    #region PROPERTIES
    [Header(".FSM STATE MODULE/Wait For")]
    [SerializeField]
    private Attributes _Data = null;
    public FSMState _AutoTransitionNextState;

    [Header(".FSM STATE MODULE/Wait For - Event")]
    [SerializeField]
    public UnityEvent OnWaitCompleted;

#if UNITY_EDITOR
    [Header(".FSM STATE MODULE/Wait For - Infos")]
    [SerializeField, ReadOnly]
    private float LastWaitingTime;
#endif
    [SerializeField, ReadOnly]
    private bool _CancelCoroutine = false;
    #endregion


    #region PUBLIC METHODS
    public override void Initialize(BlackBoardAsset blackBoard, GameObject target)
    {
        base.Initialize(blackBoard, target);

        if(_Data == null)
        {
            Debug.Break();
            Verbose(VerboseMask.ErrorLog, "Data is null!");
        }
    }

    public override void OnStateBegin()
    {
        _CancelCoroutine = false;
        Verbose(VerboseMask.Log, "Enter WaitFor Module() + _CancelCoroutine = " + _CancelCoroutine);
        StartCoroutine(WaitFor_Coroutine());
    }

    public override void OnStateExit()
    {
        _CancelCoroutine = true;
        Verbose(VerboseMask.Log, "Exit WaitFor Module()  + _CancelCoroutine = " + _CancelCoroutine);
    }
    #endregion

    #region Coroutine
    private IEnumerator WaitFor_Coroutine()
    {
        if (_Data.UseSecondsRealtime)
        {
#if UNITY_EDITOR
            yield return new WaitForSecondsRealtime(LastWaitingTime = _Data.DurationInSecond.Draw());
#else
            yield return new WaitForSecondsRealtime(_Data._DurationInSecond.Draw());
#endif
        }
        else
        {
#if UNITY_EDITOR
            yield return new WaitForSeconds(LastWaitingTime = _Data.DurationInSecond.Draw());
#else
            yield return new WaitForSeconds(_Data._DurationInSecond.Draw());
#endif
        }

        if (!_CancelCoroutine && _Data != null)
        {
            Verbose(VerboseMask.Log, "Complete coroutine! + _CancelCoroutine = " + _CancelCoroutine);
            OnWaitCompleted?.Invoke();

            if (_AutoTransitionNextState != null)
            {
                Verbose(VerboseMask.Log, "Auto transition to \"" + _AutoTransitionNextState.name + "\"");
                GetComponent<FSMState>()?.SetState(_AutoTransitionNextState);
            }
        }
        else
        {
            Verbose(VerboseMask.Log, "Cancel coroutine!");
        }
    }
    #endregion
}