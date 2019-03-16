using System;
using System.Linq;
using Zenject;

namespace UniRedux
{
    public class UniReduxSignalSubscription : IDisposable, IPoolable<Action<object>, UniReduxSignalDeclaration>
    {
        private readonly Pool _pool;
        private Action<object> _callback;
        private UniReduxSignalDeclaration _declaration;
        private UniReduxBindingId _signalId;

        public UniReduxBindingId SignalId => _signalId;

        public UniReduxSignalSubscription(Pool pool)
        {
            _pool = pool;
        }

        public void Dispose()
        {
            if (!_pool.InactiveItems.Contains(this)) _pool.Despawn(this);
        }

        public void OnDespawned()
        {
            _declaration?.Remove(this);
            SetDefaults();
        }

        public void OnSpawned(Action<object> callback, UniReduxSignalDeclaration declaration)
        {
            _callback = callback ?? throw Assert.CreateException();
            _declaration = declaration;
            _signalId = declaration.BindingId;
            declaration.Add(this);
        }

        public void Invoke(object signal)
        {
            _callback(signal);
        }
        public void OnDeclarationDespawned()
        {
            _declaration = null;
        }

        private void SetDefaults()
        {
            _callback = null;
            _declaration = null;
            _signalId = UniReduxBindingId.Empty;
        }

        public class Pool : PoolableMemoryPool<Action<object>, UniReduxSignalDeclaration, UniReduxSignalSubscription>
        {
        }
    }
}
