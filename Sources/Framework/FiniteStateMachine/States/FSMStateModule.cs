using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Atom
{
    [RequireComponent(typeof(FSMState))]
    public abstract class FSMStateModule : MonoBehaviour_Utility
    {
        #region PROPERTIES
        private GameObject _Owner;
        public GameObject Owner { get => _Owner; }

        private BlackBoardAsset _Blackboard;
        public BlackBoardAsset Blackboard { get => _Blackboard; }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Methods to initialize BBP_ or BBTP_. (BlackboardParameter or BlackboardTargetParameter)
        /// </summary>
        /// <param name="blackBoard"></param>
        /// <param name="target"></param>
        public virtual void Initialize(BlackBoardAsset blackBoard, GameObject target)
        {
            _Owner = target;
            _Blackboard = blackBoard;
            //CopyPreset();
        }

        public virtual void OnStateBegin()
        { }
        public virtual void OnStateUpdate(float deltaTime)
        { }
        public virtual void OnStateExit()
        { }

        [ContextMenu("Remove Component", false, 1)]
        public void RemoveComponentOverride()
        {
            Verbose(VerboseMask.WarningLog, "Please use the button to remove the state module.");
        }
        #endregion

    }
}