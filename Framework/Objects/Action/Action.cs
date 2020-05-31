using System.Collections.Generic;
using UnityEngine;


namespace UnityTools.Atom
{
    [CreateAssetMenu(fileName = "[Action]-", menuName = "Unity Tools/Atom/Asset/Action")]
    public class Action : ScriptableObject
    {
        #region PARAMETERS
        private List<ActionListener> _Listeners = new List<ActionListener>();
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Raise the event to all listeners
        /// </summary>
        public void Raise()
        {
            for (int i = 0; i < _Listeners.Count; i++)
                _Listeners[i].OnActionRaised();
        }

        public void Register(ActionListener listener)
        {
            _Listeners.Add(listener);
        }

        public void Unregister(ActionListener listener)
        {
            _Listeners.Remove(listener);
        }
        #endregion
    }
}
