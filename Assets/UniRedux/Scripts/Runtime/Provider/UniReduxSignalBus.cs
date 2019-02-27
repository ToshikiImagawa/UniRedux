using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRedux.Provider
{
    public class UniReduxSignalBus : IDisposable
    {
        private readonly UniReduxSignalSubscription.Pool _pool;

        private readonly Dictionary<RuntimeTypeHandle, UniReduxSignalDeclaration> _localDeclarationMap;

        private readonly Dictionary<UniReduxSignalSubscriptionId, UniReduxSignalSubscription> _subscriptionMap =
            new Dictionary<UniReduxSignalSubscriptionId, UniReduxSignalSubscription>();

        private UniReduxSignalBus _parentBus;

        public UniReduxSignalBus ParentBus
        {
            get { return _parentBus; }
        }

        public int NumSubscribers
        {
            get { return _subscriptionMap.Count; }
        }

        public UniReduxSignalBus(
            IEnumerable<UniReduxSignalDeclaration> signalDeclarations,
            UniReduxSignalSubscription.Pool pool,
            UniReduxSignalBus parentBus
        )
        {
            _parentBus = parentBus;
            _pool = pool;
            _localDeclarationMap =
                signalDeclarations.ToDictionary(declaration => declaration.SignalId, declaration => declaration);
        }

        public void Subscribe<TLocalState>(Action callback)
        {
            Action<object> wrapperCallback = args => callback();
            SubscribeInternal(typeof(TLocalState).TypeHandle, callback, wrapperCallback);
        }

        public void Subscribe<TLocalState>(Action<TLocalState> callback)
        {
            Action<object> wrapperCallback = args => callback((TLocalState) args);
            SubscribeInternal(typeof(TLocalState).TypeHandle, callback, wrapperCallback);
        }

        public void Unsubscribe<TLocalState>(Action callback)
        {
            UnsubscribeInternal(typeof(TLocalState).TypeHandle, callback);
        }

        public void Unsubscribe<TLocalState>(Action<TLocalState> callback)
        {
            UnsubscribeInternal(typeof(TLocalState).TypeHandle, callback);
        }

        public void Fire<TLocalState>()
        {
            var declaration = GetDeclaration(typeof(TLocalState));
            declaration.Fire(default(TLocalState));
        }

        public void Fire(object localState)
        {
            var declaration = GetDeclaration(localState.GetType());
            declaration.Fire(localState);
        }

        public void Dispose()
        {
        }

        private void SubscribeInternal(RuntimeTypeHandle signalId, object token, Action<object> callback)
        {
            SubscribeInternal(new UniReduxSignalSubscriptionId(signalId, token), callback);
        }

        private void SubscribeInternal(UniReduxSignalSubscriptionId id, Action<object> callback)
        {
            if (_subscriptionMap.ContainsKey(id)) throw Assert.CreateException();
            var declaration = GetDeclaration(id.SignalId);
            var subscription = _pool.Spawn(callback, declaration);
            _subscriptionMap.Add(id, subscription);
        }

        private void UnsubscribeInternal(RuntimeTypeHandle signalId, object token)
        {
            UnsubscribeInternal(new UniReduxSignalSubscriptionId(signalId, token));
        }

        private void UnsubscribeInternal(UniReduxSignalSubscriptionId id)
        {
            UniReduxSignalSubscription subscription;
            if (!_subscriptionMap.TryGetValue(id, out subscription)) return;
            _subscriptionMap.Remove(id);
            subscription.Dispose();
        }

        private UniReduxSignalDeclaration GetDeclaration(Type signalType)
        {
            return GetDeclaration(signalType.TypeHandle);
        }

        private UniReduxSignalDeclaration GetDeclaration(RuntimeTypeHandle signalId)
        {
            UniReduxSignalDeclaration handler;
            return _localDeclarationMap.TryGetValue(signalId, out handler)
                ? handler
                : _parentBus?.GetDeclaration(signalId);
        }


        public class UniReduxSignalDeclaration : IDisposable
        {
            private readonly List<UniReduxSignalSubscription> _subscriptions = new List<UniReduxSignalSubscription>();
            private readonly RuntimeTypeHandle _signalId;

            public RuntimeTypeHandle SignalId => _signalId;

            public UniReduxSignalDeclaration(RuntimeTypeHandle signalId)
            {
                _signalId = signalId;
            }

            public void Fire(object signal)
            {
                if (!signal.GetType().TypeHandle.Equals(_signalId)) throw Assert.CreateException();
                foreach (var subscription in _subscriptions)
                {
                    subscription.Invoke(signal);
                }
            }

            public void Dispose()
            {
                foreach (var subscription in _subscriptions)
                {
                    subscription.OnDeclarationDespawned();
                }
            }

            public void Add(UniReduxSignalSubscription subscription)
            {
                if (_subscriptions.Contains(subscription)) throw Assert.CreateException();
                _subscriptions.Add(subscription);
            }

            public void Remove(UniReduxSignalSubscription subscription)
            {
                _subscriptions.Remove(subscription);
            }
        }

        public class UniReduxSignalSubscription : IDisposable
        {
            private readonly Pool _pool;
            private Action<object> _callback;
            private RuntimeTypeHandle _signalId;
            private UniReduxSignalDeclaration _declaration;

            public UniReduxSignalSubscription(Pool pool)
            {
                _pool = pool;
                SetDefaults();
            }

            public void Invoke(object localState)
            {
                _callback?.Invoke(localState);
            }

            public void Dispose()
            {
                if (!_pool.InactiveItems.Contains(this))
                {
                    _pool.Despawn(this);
                }
            }

            public void OnDeclarationDespawned()
            {
                _declaration = null;
            }

            private void SetDefaults()
            {
                _callback = null;
                _signalId = default;
            }

            private void OnSpawned(Action<object> listener, UniReduxSignalDeclaration declaration)
            {
                _callback = listener;
                _signalId = declaration.SignalId;
                _declaration = declaration;
                declaration.Add(this);
            }

            private void OnDespawned()
            {
                if (_declaration != null)
                {
                    _declaration.Remove(this);
                }

                SetDefaults();
            }

            public class Pool : IDisposable
            {
                private readonly Queue<UniReduxSignalSubscription> _cache = new Queue<UniReduxSignalSubscription>();

                public IEnumerable<UniReduxSignalSubscription> InactiveItems => _cache;

                public UniReduxSignalSubscription Spawn(Action<object> listener, UniReduxSignalDeclaration declaration)
                {
                    var subscription = _cache.Count > 0 ? _cache.Dequeue() : new UniReduxSignalSubscription(this);
                    subscription.OnSpawned(listener, declaration);
                    return subscription;
                }

                public void Despawn(UniReduxSignalSubscription signalSubscription)
                {
                    signalSubscription.OnDespawned();
                    _cache.Enqueue(signalSubscription);
                }

                public void Dispose()
                {
                    _cache.Clear();
                }
            }
        }

        public class UniReduxSignalSubscriptionId : IEquatable<UniReduxSignalSubscriptionId>
        {
            private readonly RuntimeTypeHandle _signalId;
            private readonly object _token;

            public RuntimeTypeHandle SignalId => _signalId;
            public object Token => _token;

            public UniReduxSignalSubscriptionId(RuntimeTypeHandle signalId, object token)
            {
                _signalId = signalId;
                _token = token;
            }

            public bool Equals(UniReduxSignalSubscriptionId other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return _signalId.Equals(other._signalId) && Equals(_token, other._token);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((UniReduxSignalSubscriptionId) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_signalId.GetHashCode() * 397) ^ (_token != null ? _token.GetHashCode() : 0);
                }
            }
        }
    }
}