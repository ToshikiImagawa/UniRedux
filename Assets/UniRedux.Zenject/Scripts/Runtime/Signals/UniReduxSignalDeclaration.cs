using ModestTree;
using System;
using System.Collections.Generic;
using Zenject;

namespace UniRedux
{
    public class UniReduxSignalDeclaration : IDisposable
    {
        private readonly List<UniReduxSignalSubscription> _subscriptions = new List<UniReduxSignalSubscription>();
        private readonly UniReduxBindingId _bindingId;
        private readonly UniReduxBindingId _parentBindingId;
        private readonly IUniReduxStateDirector _director;
        private object _locaState;

        public UniReduxBindingId BindingId => _bindingId;
        public UniReduxBindingId ParentBindingId => _parentBindingId;

        public UniReduxSignalDeclaration(UniReduxSignalDeclarationBindInfo bindInfo)
        {
            _bindingId = new UniReduxBindingId(bindInfo.LocalStateType, bindInfo.OriginalStateType, bindInfo.Identifier);
            _director = bindInfo.Director;
            _parentBindingId = bindInfo.ParentBindingId;
        }
        public void Add(UniReduxSignalSubscription subscription)
        {
            if (_subscriptions.Contains(subscription)) throw Assert.CreateException();
            _subscriptions.Add(subscription);
        }
        public void Remove(UniReduxSignalSubscription subscription)
        {
            _subscriptions.RemoveWithConfirm(subscription);
        }

        public object Fire(object originalState)
        {
            if (originalState.GetType().DerivesFromOrEqual(_bindingId.OriginalStateType)) throw Assert.CreateException();
            using (var block = DisposeBlock.Spawn())
            {
                var subscriptions = block.SpawnList<UniReduxSignalSubscription>();
                subscriptions.AddRange(_subscriptions);
                return FireInternal(subscriptions, originalState);
            }
        }

        public void Dispose()
        {
            foreach (UniReduxSignalSubscription subscription in _subscriptions)
            {
                subscription.OnDeclarationDespawned();
            }
        }

        private object FireInternal(List<UniReduxSignalSubscription> subscriptions, object originalState)
        {
            var localState = _director.Converter(originalState);
            if (_director.Checker(_locaState, localState)) return _locaState;
            _locaState = localState;
            for (int i = 0; i < subscriptions.Count; i++)
            {
                var subscription = subscriptions[i];
                if (_subscriptions.Contains(subscription))
                {
                    subscription.Invoke(_locaState);
                }
            }
            return _locaState;
        }
    }
}
