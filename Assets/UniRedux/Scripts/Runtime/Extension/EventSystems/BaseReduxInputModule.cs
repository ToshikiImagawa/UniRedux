using System;
using System.Xml.Serialization;
using UnityEngine;

namespace UniRedux.EventSystems
{
    public abstract class BaseReduxInputModule<TState> : MonoBehaviour
    {
        private IDisposable _disposable;

        private IExecuteReduxEventSystem _eventSystem;

        protected IExecuteReduxEventSystem EventSystem =>
            _eventSystem ?? (_eventSystem = GetComponent<IExecuteReduxEventSystem>());

        /// <summary>
        /// Current Store
        /// </summary>
        protected abstract IStore<TState> CurrentStore { get; }

        /// <summary>
        /// Process the current tick for the module.
        /// </summary>
        /// <param name="state"></param>
        protected abstract void Process(VoidMessage state);

        /// <summary>
        /// Before OnDestroy
        /// </summary>
        protected virtual void BeforeDestroy()
        {
        }

        /// <summary>
        /// After OnDestroy
        /// </summary>
        protected virtual void AfterDestroy()
        {
        }

        /// <summary>
        /// Start monitoring status
        /// </summary>
        protected void StartMonitoring()
        {
            _disposable = CurrentStore.Subscribe(Process);
        }

        public void OnDestroy()
        {
            BeforeDestroy();
            _disposable?.Dispose();
            AfterDestroy();
        }
    }
}