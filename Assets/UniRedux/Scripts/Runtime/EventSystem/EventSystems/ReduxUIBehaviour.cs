using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRedux.EventSystems
{
    public abstract class ReduxUIBehaviour<TState> : UIBehaviour, IReduxEventSystemSubscriber
    {
        [SerializeField]
        private bool isProject;

        protected TState State => CurrentStore.GetState();
        protected Dispatcher Dispatch => CurrentStore.Dispatch;

        protected abstract IStore<TState> CurrentStore { get; }
        private IDisposable _rxEventSystemDisposable;

        protected virtual void BeforeInit()
        {
        }

        protected virtual void AfterInit()
        {
        }

        protected virtual void BeforeDestroy()
        {
        }

        protected sealed override void Awake()
        {
            base.Awake();
            BeforeInit();
            if (_rxEventSystemDisposable == null)
            {
                if (isProject)
                    ProjectEventSystem.InternalSubscribeEventSystem(this, SubscribeEventSystem);
                else
                    SceneEventSystem.InternalSubscribeEventSystem(this, SubscribeEventSystem);
            }

            AfterInit();
        }

        /// <summary>
        /// Dispose events subscribed to the store.
        /// </summary>
        public void Dispose()
        {
            _rxEventSystemDisposable?.Dispose();
        }

        public void SubscribeEventSystem(IDisposable disposable)
        {
            _rxEventSystemDisposable = disposable;
        }

        protected sealed override void OnDestroy()
        {
            BeforeDestroy();
            Dispose();
            base.OnDestroy();
        }
    }
}