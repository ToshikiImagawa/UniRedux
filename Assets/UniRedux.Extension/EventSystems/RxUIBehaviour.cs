using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRedux.EventSystems
{
    public abstract class RxUIBehaviour<TState> : UIBehaviour
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

        protected override void Awake()
        {
            base.Awake();
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Dispose();
        }
    }
}