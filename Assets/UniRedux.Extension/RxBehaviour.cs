using System;
using UnityEngine;

namespace UniRedux
{
    public abstract class RxBehaviour<TState> : MonoBehaviour
    {
        public TState State => CurrentStore.GetState();
        protected abstract IStore<TState> CurrentStore { get; }
        private IDisposable _disposable;

        protected virtual void BeforeInit()
        {
        }

        protected virtual void AfterInit()
        {
        }

        private void Awake()
        {
            BeforeInit();
            _disposable?.Dispose();
            _disposable = CurrentStore.Subscribe(
                () => { gameObject.SendMessage("HandleChange", 0, SendMessageOptions.DontRequireReceiver); }
            );
            AfterInit();
        }

        /// <summary>
        /// Dispose events subscribed to the store.
        /// </summary>
        protected void Dispose()
        {
            _disposable?.Dispose();
        }

        protected void OnDestroy()
        {
            Dispose();
        }
    }
}