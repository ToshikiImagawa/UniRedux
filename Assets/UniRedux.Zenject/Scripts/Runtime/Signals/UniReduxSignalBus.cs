using ModestTree;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace UniRedux
{
    public class UniReduxSignalBus : ILateDisposable
    {
        private readonly UniReduxSignalSubscription.Pool _subscriptionPool;
        private readonly Dictionary<UniReduxBindingId, UniReduxSignalDeclaration> _localDeclarationMap;
        private readonly Dictionary<UniReduxSignalSubscriptionId, UniReduxSignalSubscription> _subscriptionMap = new Dictionary<UniReduxSignalSubscriptionId, UniReduxSignalSubscription>();
        private readonly Dictionary<UniReduxBindingId, UniReduxBindingId[]> _nestedBindingIdMap;

        private readonly Dictionary<UniReduxSignalBus, Action<UniReduxBindingId, object>> _childFireMap = new Dictionary<UniReduxSignalBus, Action<UniReduxBindingId, object>>();

        private readonly UniReduxSignalBus _parentBus;
        private readonly IDisposable _disposable;
        private readonly Func<object> _getState;

        public UniReduxSignalBus ParentBus
        {
            get { return _parentBus; }
        }

        public int NumSubscribers
        {
            get { return _subscriptionMap.Count; }
        }

        [Inject]
        public UniReduxSignalBus(
            [Inject(Source = InjectSources.Local)]
            List<UniReduxSignalDeclaration> signalDeclarations,
            [Inject(Source = InjectSources.Parent, Optional = true)]
            UniReduxSignalBus parentBus,
            UniReduxSignalSubscription.Pool subscriptionPool,
            IStore store)
        {
            _subscriptionPool = subscriptionPool;
            _localDeclarationMap = signalDeclarations.ToDictionary(x => x.BindingId, x => x);
            _parentBus = parentBus;
            _getState = store.GetState;
            _nestedBindingIdMap = signalDeclarations.GroupBy(x => x.ParentBindingId)
                .ToDictionary(x => x.Key, x => x.Select(y => y.BindingId).ToArray());
            _disposable = _parentBus == null ? store.Subscribe(OnChangeState) : new ChildFireDisposable(_parentBus, this);
        }

        public void Subscribe<TLocalState>(Action callback, object identifier = null)
        {
            SubscribeInternal(typeof(TLocalState), identifier, callback, (object args) => callback());
        }

        public void Subscribe<TLocalState>(Action<TLocalState> callback, object identifier = null)
        {
            SubscribeInternal(typeof(TLocalState), identifier, callback, (object args) => callback((TLocalState)args));
        }

        public void Subscribe(Type localStateType, Action<object> callback, object identifier = null)
        {
            SubscribeInternal(localStateType, identifier, callback, callback);
        }

        public void Unsubscribe<TLocalState>(Action callback, object identifier = null)
        {
            Unsubscribe(typeof(TLocalState), callback, identifier);
        }

        public void Unsubscribe(Type localStateType, Action callback, object identifier = null)
        {
            UnsubscribeInternal(localStateType, identifier, callback, true);
        }

        public void Unsubscribe(Type localStateType, Action<object> callback, object identifier = null)
        {
            UnsubscribeInternal(localStateType, identifier, callback, true);
        }

        public void Unsubscribe<TLocalState>(Action<TLocalState> callback, object identifier = null)
        {
            UnsubscribeInternal(typeof(TLocalState), identifier, callback, true);
        }

        public void LateDispose()
        {
            _disposable?.Dispose();
            foreach (var subscription in _subscriptionMap.Values) subscription.Dispose();
            foreach (var declaration in _localDeclarationMap.Values) declaration.Dispose();
            _childFireMap.Clear();
        }

        private void SubscribeInternal(Type localStateType, object identifier, object token, Action<object> callback)
        {
            SubscribeInternal(new UniReduxBindingId(localStateType, identifier), token, callback);
        }

        private void SubscribeInternal(UniReduxBindingId signalId, object token, Action<object> callback)
        {
            SubscribeInternal(
                new UniReduxSignalSubscriptionId(signalId, token), callback);
        }

        private void SubscribeInternal(UniReduxSignalSubscriptionId id, Action<object> callback)
        {
            if (_subscriptionMap.ContainsKey(id)) throw Assert.CreateException("Tried subscribing to the same signal with the same callback on UniRedux.UniReduxSignalBus");

            var declaration = GetDeclaration(id.SignalId);
            var subscription = _subscriptionPool.Spawn(callback, declaration);

            _subscriptionMap.Add(id, subscription);
        }

        private void UnsubscribeInternal(Type localStateType, object identifier, object token, bool throwIfMissing)
        {
            UnsubscribeInternal(new UniReduxBindingId(localStateType, identifier), token, throwIfMissing);
        }

        private void UnsubscribeInternal(UniReduxBindingId signalId, object token, bool throwIfMissing)
        {
            UnsubscribeInternal(new UniReduxSignalSubscriptionId(signalId, token), throwIfMissing);
        }

        private void UnsubscribeInternal(UniReduxSignalSubscriptionId id, bool throwIfMissing)
        {
            UniReduxSignalSubscription subscription;

            if (_subscriptionMap.TryGetValue(id, out subscription))
            {
                _subscriptionMap.RemoveWithConfirm(id);
                subscription.Dispose();
            }
            else
            {
                if (throwIfMissing)
                {
                    throw Assert.CreateException(
                        "Called unsubscribe for signal '{0}' but could not find corresponding subscribe.  If this is intentional, call TryUnsubscribe instead.");
                }
            }
        }

        private UniReduxSignalDeclaration GetDeclaration(Type localStateType, object identifier)
        {
            return GetDeclaration(new UniReduxBindingId(localStateType, identifier));
        }

        private UniReduxSignalDeclaration GetDeclaration(UniReduxBindingId signalId)
        {
            if (_localDeclarationMap.TryGetValue(signalId, out UniReduxSignalDeclaration handler)) return handler;

            if (_parentBus != null) return _parentBus.GetDeclaration(signalId);

            throw Assert.CreateException("Fired undeclared signal '{0}'!", signalId);
        }

        private UniReduxBindingId[] GetNestedBindingIds(UniReduxBindingId signalId)
        {
            if (!_nestedBindingIdMap.ContainsKey(signalId)) return Array.Empty<UniReduxBindingId>();
            return _nestedBindingIdMap[signalId];
        }

        private void OnChangeState(VoidMessage _)
        {
            var state = _getState?.Invoke();
            Fire(UniReduxBindingId.Empty, state);
        }

        private void Fire(UniReduxBindingId bindingId, object state)
        {
            InnerFire(GetNestedBindingIds(bindingId), state);
            foreach (var childFire in _childFireMap.Values)
            {
                childFire?.Invoke(bindingId, state);
            }
        }

        private void InnerFire(UniReduxBindingId[] bindingIds, object state)
        {
            if (bindingIds == null || bindingIds.Length == 0) return;
            foreach (var bindingId in bindingIds)
            {
                var declaration = GetDeclaration(bindingId);
                var localState = declaration.Fire(state);
                Fire(bindingId, localState);
            }
        }

        private class ChildFireDisposable : IDisposable
        {
            private UniReduxSignalBus _parentSignalBus;
            private UniReduxSignalBus _signalBus;

            public ChildFireDisposable(UniReduxSignalBus parentSignalBus, UniReduxSignalBus signalBus)
            {
                if (parentSignalBus == null) Assert.CreateException();
                _parentSignalBus = parentSignalBus;
                _signalBus = signalBus;
                _parentSignalBus._childFireMap[_signalBus] = _signalBus.Fire;
            }

            public void Dispose()
            {
                _parentSignalBus?._childFireMap?.Remove(_signalBus);
            }
        }
    }
}