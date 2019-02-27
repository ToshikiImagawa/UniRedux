using System;
using System.Collections.Generic;

namespace UniRedux.Provider
{
    public class UniReduxDiContainer<TState> : IDisposable, IUniReduxBinder
    {
        private readonly IStore<TState> _store;
        private readonly IDisposable _disposable;
        private readonly UniReduxSubscription.Pool _pool;
        private readonly UniReduxSignalBus.UniReduxSignalSubscription.Pool _signalPool;

        private readonly UniReduxDiContainer<TState> _parentContainer;

        private readonly Dictionary<RuntimeTypeHandle, IUniReduxContainerDirector> _directors =
            new Dictionary<RuntimeTypeHandle, IUniReduxContainerDirector>();

        private readonly Dictionary<RuntimeTypeHandle, List<UniReduxSubscription>> _subscriber =
            new Dictionary<RuntimeTypeHandle, List<UniReduxSubscription>>();

        private readonly List<UniReduxSignalBus.UniReduxSignalDeclaration> _signalDeclarations =
            new List<UniReduxSignalBus.UniReduxSignalDeclaration>();

        private event Action<RuntimeTypeHandle, object> Children;

        public void Bind<TLocalState>(Func<TState, TLocalState> converter)
        {
            if (_directors.ContainsKey(typeof(TLocalState).TypeHandle)) throw Assert.CreateException();
            _directors[typeof(TLocalState).TypeHandle] = UniReduxContainerDirector<TLocalState>.Create(converter);
            _subscriber[typeof(TLocalState).TypeHandle] = new List<UniReduxSubscription>();
        }

        public void DeclareSignal<TLocalState>()
        {
            var bindInfo = new UniReduxSignalDeclarationBindInfo(typeof(TLocalState));
            SetDeclaration(new UniReduxSignalBus.UniReduxSignalDeclaration(bindInfo.SignalType.TypeHandle));
        }

        public UniReduxSignalBus CreateSignalBus(UniReduxSignalBus parentBus = null)
        {
            return new UniReduxSignalBus(_signalDeclarations.ToArray(), _signalPool, parentBus);
        }

        IUniReduxBinder IUniReduxBinder.Subscribe<TLocalState>(Action<TLocalState> listener)
        {
            if (!_subscriber.ContainsKey(typeof(TLocalState).TypeHandle))
                _subscriber[typeof(TLocalState).TypeHandle] = new List<UniReduxSubscription>();
            var subscription = _pool.Spawn();
            subscription.Subscribe(obj => { listener?.Invoke((TLocalState) obj); });
            _subscriber[typeof(TLocalState).TypeHandle].Add(subscription);
            return this;
        }

        public UniReduxDiContainer()
        {
            _parentContainer = null;
            _store = UniReduxProvider.GetStore<TState>();
            _disposable = _store.Subscribe(OnChangeState);
            _pool = new UniReduxSubscription.Pool();
            _signalPool = new UniReduxSignalBus.UniReduxSignalSubscription.Pool();
        }

        public UniReduxDiContainer(UniReduxDiContainer<TState> parentContainer)
        {
            _parentContainer = parentContainer;
            _pool = _parentContainer._pool;
            _parentContainer.Children += OnChangeState;
            _signalPool = _parentContainer._signalPool;
        }

        private void OnChangeState(VoidMessage voidMessage)
        {
            foreach (var director in _directors)
            {
                if (director.Value.CheckingStateEquals(_store.GetState())) return;
                if (!_subscriber.ContainsKey(director.Key)) return;
                OnChangeState(director.Key, director.Value.LocalState);

                Children?.Invoke(director.Key, director.Value.LocalState);
            }
        }

        private void OnChangeState(RuntimeTypeHandle key, object localState)
        {
            foreach (var subscription in _subscriber[key])
            {
                subscription?.Fire(localState);
            }
        }

        private void SetDeclaration(UniReduxSignalBus.UniReduxSignalDeclaration declaration)
        {
            _signalDeclarations.Add(declaration);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _directors.Clear();
            foreach (var keyValuePair in _subscriber)
            {
                foreach (var subscription in keyValuePair.Value)
                {
                    _pool.Despawn(subscription);
                }
            }

            if (_parentContainer != null) _parentContainer.Children -= OnChangeState;
        }

        private class UniReduxContainerDirector<TLocalState> : IUniReduxContainerDirector
        {
            private TLocalState _localState;
            private readonly IEqualityComparer<TLocalState> _equalityComparer;
            private readonly Func<TState, TLocalState> _converter;

            object IUniReduxContainerDirector.LocalState => _localState;

            private UniReduxContainerDirector(Func<TState, TLocalState> converter)
            {
                _converter = converter;
                _equalityComparer = UniReduxEqualityComparer.GetDefault<TLocalState>();
            }

            public static IUniReduxContainerDirector Create(Func<TState, TLocalState> converter)
            {
                return new UniReduxContainerDirector<TLocalState>(converter);
            }

            bool IUniReduxContainerDirector.CheckingStateEquals(TState state)
            {
                var nextState = _converter.Invoke(state);
                if (_equalityComparer.Equals(_localState, nextState)) return true;
                _localState = nextState;
                return false;
            }
        }

        private interface IUniReduxContainerDirector
        {
            object LocalState { get; }
            bool CheckingStateEquals(TState state);
        }

        public class UniReduxSubscription
        {
            private Action<object> _listener;

            public void Subscribe(Action<object> listener)
            {
                _listener = listener;
            }

            public void Fire(object localState)
            {
                _listener?.Invoke(localState);
            }

            public class Pool : IDisposable
            {
                private readonly Queue<UniReduxSubscription> _cache = new Queue<UniReduxSubscription>();

                public UniReduxSubscription Spawn()
                {
                    return _cache.Count > 0 ? _cache.Dequeue() : new UniReduxSubscription();
                }

                public void Despawn(UniReduxSubscription subscription)
                {
                    _cache.Enqueue(subscription);
                }

                public void Dispose()
                {
                    _cache.Clear();
                }
            }
        }
    }

    public interface IUniReduxBinder
    {
        IUniReduxBinder Subscribe<TLocalState>(Action<TLocalState> listener);
    }
}