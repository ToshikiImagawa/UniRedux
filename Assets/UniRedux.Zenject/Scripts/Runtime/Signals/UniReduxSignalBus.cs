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
        private readonly UniReduxBindingId[] _rootStateDeclarationBindingIds;
        private readonly Dictionary<UniReduxBindingId, UniReduxBindingId[]> _nestedLocalDeclarationMap;

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
            _disposable = store.Subscribe(OnChangeState);
            _getState = store.GetState;
            _rootStateDeclarationBindingIds = signalDeclarations
                .Where(x => x.BindingId.OriginalStateType == store.GetStateType())
                .Select(x => x.BindingId).ToArray();
            _nestedLocalDeclarationMap = signalDeclarations.GroupBy(x => x.ParentBindingId)
                .ToDictionary(x => x.Key, x => x.Select(y => y.BindingId).ToArray());
        }

        public void Subscribe<TLocalState, TOriginalState>(Action callback, object identifier = null)
        {
            Action<object> wrapperCallback = args => callback();
            SubscribeInternal(typeof(TLocalState), typeof(TOriginalState), identifier, callback, wrapperCallback);
        }

        public void Subscribe<TLocalState, TOriginalState>(Action<TLocalState> callback, object identifier = null)
        {
            Action<object> wrapperCallback = args => callback((TLocalState)args);
            SubscribeInternal(typeof(TLocalState), typeof(TOriginalState), identifier, callback, wrapperCallback);
        }

        public void Subscribe(Type localStateType, Type originalStateType, Action<object> callback, object identifier = null)
        {
            SubscribeInternal(localStateType, originalStateType, identifier, callback, callback);
        }

        public void Unsubscribe<TLocalState, TOriginalState>(Action callback, object identifier = null)
        {
            Unsubscribe(typeof(TLocalState), typeof(TOriginalState), callback, identifier);
        }

        public void Unsubscribe(Type localStateType, Type originalStateType, Action callback, object identifier = null)
        {
            UnsubscribeInternal(localStateType, originalStateType, identifier, callback, true);
        }

        public void Unsubscribe(Type localStateType, Type originalStateType, Action<object> callback, object identifier = null)
        {
            UnsubscribeInternal(localStateType, originalStateType, identifier, callback, true);
        }

        public void Unsubscribe<TLocalState, TOriginalState>(Action<TLocalState> callback, object identifier = null)
        {
            UnsubscribeInternal(typeof(TLocalState), typeof(TOriginalState), identifier, callback, true);
        }

        public void LateDispose()
        {
            _disposable?.Dispose();
            foreach (var subscription in _subscriptionMap.Values) subscription.Dispose();
            foreach (var declaration in _localDeclarationMap.Values) declaration.Dispose();
        }

        private void SubscribeInternal(Type localStateType, Type originalStateType, object identifier, object token, Action<object> callback)
        {
            SubscribeInternal(new UniReduxBindingId(localStateType, originalStateType, identifier), token, callback);
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

        private void UnsubscribeInternal(Type localStateType, Type originalStateType, object identifier, object token, bool throwIfMissing)
        {
            UnsubscribeInternal(new UniReduxBindingId(localStateType, originalStateType, identifier), token, throwIfMissing);
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

        private UniReduxSignalDeclaration GetDeclaration(Type localStateType, Type originalStateType, object identifier)
        {
            return GetDeclaration(new UniReduxBindingId(localStateType, originalStateType, identifier));
        }

        private UniReduxSignalDeclaration GetDeclaration(UniReduxBindingId signalId)
        {
            if (_localDeclarationMap.TryGetValue(signalId, out UniReduxSignalDeclaration handler)) return handler;

            if (_parentBus != null) return _parentBus.GetDeclaration(signalId);

            throw Assert.CreateException("Fired undeclared signal '{0}'!", signalId);
        }

        private IEnumerable<UniReduxBindingId> GetNestedBindingIds(UniReduxBindingId signalId)
        {
            if (!_nestedLocalDeclarationMap.ContainsKey(signalId)) yield break;
            foreach (var bindingId in _nestedLocalDeclarationMap[signalId]) yield return bindingId;
        }

        private void OnChangeState(VoidMessage _)
        {
            var state = _getState?.Invoke();
            Fire(_rootStateDeclarationBindingIds, state);
        }

        private void Fire(IEnumerable<UniReduxBindingId> bindingIds, object state)
        {
            if (bindingIds == null) return;
            foreach (var bindingId in bindingIds)
            {
                var declaration = GetDeclaration(bindingId);
                var localState = declaration.Fire(state);
                Fire(GetNestedBindingIds(bindingId), localState);
            }
        }
    }
}