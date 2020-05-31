using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using UnityTools.Atom;

public class FSMStateModule_SimpleTranslation : FSMStateModule
{
    #region PROPERTIES
    [Header(".FSM STATE MODULE/Simple Translation")]
    public Vector3Variable Distance;
    public CurveVariable XVelocity;
    public CurveVariable YVelocity;
    public CurveVariable ZVelocity;

    public FloatVariable DurationInSeconds;

    [Header(".FSM STATE MODULE/Simple Translation - Event")]
    [SerializeField]
    private UnityEvent _OnTranslationComplete = null;


    [Header(".FSM STATE MODULE/Simple Translation - Infos")]
    private float _TimeStamp = 0;
    private Vector3 _Origin;
    #endregion

    #region PUBLIC METHODS
    public override void OnStateBegin()
    {
        _TimeStamp = 0;
        _Origin = Owner.transform.position;
    }

    public override void OnStateUpdate(float deltaTime)
    {
        if (_TimeStamp >= DurationInSeconds)
        {
            return;
        }

        _TimeStamp += deltaTime;

        Vector3 currentPos;
        currentPos.x = _Origin.x + (Distance.Value.x * XVelocity.Value.Evaluate(_TimeStamp / DurationInSeconds));
        currentPos.y = _Origin.y + (Distance.Value.y * YVelocity.Value.Evaluate(_TimeStamp / DurationInSeconds));
        currentPos.z = _Origin.z + (Distance.Value.z * ZVelocity.Value.Evaluate(_TimeStamp / DurationInSeconds));

        Owner.transform.position = currentPos;

        if (_TimeStamp >= DurationInSeconds)
        {
            _OnTranslationComplete?.Invoke();
        }
    }
    public override void OnStateExit()
    { }
    #endregion

}
