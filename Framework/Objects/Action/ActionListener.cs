using UnityEngine;
using UnityEngine.Events;


namespace UnityTools.Atom
{
    public class ActionListener : MonoBehaviour
    {
        #region PARAMETERS
        [SerializeField]
        private Action _Action = null;
        [SerializeField]
        private UnityEvent _Response = null;
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Change Action to listen. Might want to remove this function as it may be a source of bug.
        /// </summary>
        /// <param name="action">Action to set</param>
        public void SwitchAction(Action action)
        {
            gameObject.SetActive(false);

            _Action = action;

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Function called by Action scriptable object when the action is raised
        /// </summary>
        public void OnActionRaised()
        {
            if (_Response.GetPersistentEventCount() == 0)
                Logger.Log(Logger.Type.Warning, "_Response doesn't have any listener...", this);

            _Response?.Invoke();
        }
        #endregion

        #region UNITY METHODS
        private void OnEnable()
        {
            Logger.Assert(_Action != null, "_Action is null", this);
            _Action.Register(this);
        }

        private void OnDisable()
        {
            Logger.Assert(_Action != null, "_Action is null", this);
            _Action.Unregister(this);
        }
        #endregion
    }
}