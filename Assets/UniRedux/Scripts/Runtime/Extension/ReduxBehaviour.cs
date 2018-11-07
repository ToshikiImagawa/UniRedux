using System;
using UniRedux.EventSystems;
using UnityEngine;
using UnityEngine.Serialization;

namespace UniRedux
{
    public abstract class ReduxBehaviour<TState> : MonoBehaviour, IReduxEventSystemSubscriber
    {
        [FormerlySerializedAs("isProject")] [SerializeField]
        private bool _isProject;

        public TState State => CurrentStore.GetState();
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

        public void Awake()
        {
            BeforeInit();
            if (_rxEventSystemDisposable == null)
            {
                if (_isProject)
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

        public void OnDestroy()
        {
            BeforeDestroy();
            Dispose();
        }
    }

    public interface IReduxEventSystemSubscriber : IDisposable
    {
        void SubscribeEventSystem(IDisposable disposable);
    }
}