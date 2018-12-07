using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniRedux.Rx;
using UniSystem.Reactive.Disposables;
using UnityEngine;

namespace UniRedux.Provider
{
    public static class UniReduxContainer<TState>
    {
        public static IUniReduxContainer Connect<TLocalState>(MapStateProps<TState, TLocalState> mapStateToProps)
            where TLocalState : class
        {
            return new Container<TLocalState>(mapStateToProps);
        }

        public static IUniReduxContainer Connect<TLocalState, TActionDispatcher>(
            MapStateProps<TState, TLocalState> mapStateToProps, MapDispatchProps<TActionDispatcher> mapDispatchToProps)
            where TLocalState : class where TActionDispatcher : class
        {
            return new Container<TLocalState, TActionDispatcher>(mapStateToProps, mapDispatchToProps);
        }

        private class Container<TLocalState> : UniReduxContainer.Container, IUniReduxContainerObservable<TLocalState>
            where TLocalState : class
        {
            private readonly MapStateProps<TState, TLocalState> _mapStateProps;
            private readonly IEqualityComparer<TLocalState> _equalityComparer;

            private readonly object _syncRoot = new object();
            private ListObserver<TLocalState> _listObserver = ListObserver.Empty<TLocalState>();

            private readonly IDisposable _disposable;

            private TLocalState _localState;

            public override IDisposable Inject(IUniReduxComponent component)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) return null;
#endif
                component.InjectDispatcher();
                var observer = new UniReduxContainer.Observer<TLocalState>(component);
                return InnerSubscribe(observer);
            }

            public override void Dispose()
            {
                lock (_syncRoot)
                {
                    _listObserver.OnCompleted();
                    _listObserver = ListObserver.Empty<TLocalState>();
                }

                _disposable?.Dispose();
            }

            IDisposable IUniReduxContainerObservable<TLocalState>.Subscribe(IObserver<TLocalState> observer)
            {
                return InnerSubscribe(observer);
            }

            private IDisposable InnerSubscribe(IObserver<TLocalState> observer)
            {
                try
                {
                    lock (_syncRoot)
                    {
                        _listObserver = _listObserver.Add(observer);
                    }

                    observer.OnNext(_localState);

                    return new Subscription(this, observer);
                }
                catch (Exception e)
                {
                    lock (_syncRoot)
                    {
                        _listObserver.OnError(e);
                        _listObserver = ListObserver.Empty<TLocalState>();
                    }

                    return new Subscription(this, EmptyObserver<TLocalState>.Instance);
                }
            }

            public Container(MapStateProps<TState, TLocalState> mapStateProps)
            {
                _mapStateProps = mapStateProps;
                _equalityComparer = EqualityComparer<TLocalState>.Default;
                _disposable = mapStateProps == null ? null : UniReduxProvider.GetStore<TState>().Subscribe(OnNext);
            }

            private void OnNext(VoidMessage v)
            {
                var localState = _mapStateProps.Invoke(UniReduxProvider.GetStore<TState>().GetState());
                if (_equalityComparer.Equals(localState, _localState)) return;
                _localState = localState;
                lock (_syncRoot)
                {
                    _listObserver.OnNext(_localState);
                }
            }

            private sealed class Subscription : IDisposable
            {
                private readonly object _disposeLock = new object();
                private Container<TLocalState> _parent;
                private IObserver<TLocalState> _unsubscribeTarget;

                public Subscription(Container<TLocalState> parent,
                    IObserver<TLocalState> unsubscribeTarget)
                {
                    _parent = parent;
                    _unsubscribeTarget = unsubscribeTarget;
                }

                public void Dispose()
                {
                    lock (_disposeLock)
                    {
                        if (_parent == null) return;
                        lock (_parent._syncRoot)
                        {
                            if (_unsubscribeTarget != null)
                            {
                                if (_parent._listObserver == null) return;
                                _parent._listObserver = _parent._listObserver.Remove(_unsubscribeTarget);
                            }

                            _unsubscribeTarget = null;
                            _parent = null;
                        }
                    }
                }
            }
        }

        private class Container<TLocalState, TActionDispatcher> : UniReduxContainer.Container,
            IUniReduxContainerObservable<TLocalState>
            where TLocalState : class
            where TActionDispatcher : class
        {
            private readonly MapStateProps<TState, TLocalState> _mapStateProps;
            private readonly IEqualityComparer<TLocalState> _equalityComparer;
            private readonly TActionDispatcher _actionDispatcher;

            private readonly object _syncRoot = new object();
            private ListObserver<TLocalState> _listObserver = ListObserver.Empty<TLocalState>();

            private readonly IDisposable _disposable;

            private TLocalState _localState;

            public override IDisposable Inject(IUniReduxComponent component)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) return null;
#endif
                component.InjectDispatcher();

                var componentType = component.GetType();
                var componentProperties = componentType
                    .GetUniReduxInjectProperties();

                component.InjectActionDispatcher(componentProperties, _actionDispatcher);

                var observer = new UniReduxContainer.Observer<TLocalState>(component);
                return InnerSubscribe(observer);
            }

            public override void Dispose()
            {
                lock (_syncRoot)
                {
                    _listObserver.OnCompleted();
                    _listObserver = ListObserver.Empty<TLocalState>();
                }

                _disposable?.Dispose();
            }

            IDisposable IUniReduxContainerObservable<TLocalState>.Subscribe(IObserver<TLocalState> observer)
            {
                return InnerSubscribe(observer);
            }

            private IDisposable InnerSubscribe(IObserver<TLocalState> observer)
            {
                try
                {
                    lock (_syncRoot)
                    {
                        _listObserver = _listObserver.Add(observer);
                    }

                    observer.OnNext(_localState);

                    return new Subscription(this, observer);
                }
                catch (Exception e)
                {
                    lock (_syncRoot)
                    {
                        _listObserver.OnError(e);
                        _listObserver = ListObserver.Empty<TLocalState>();
                    }

                    return new Subscription(this, EmptyObserver<TLocalState>.Instance);
                }
            }

            public Container(MapStateProps<TState, TLocalState> mapStateProps,
                MapDispatchProps<TActionDispatcher> mapDispatchProps)
            {
                if (mapStateProps == null) return;
                _mapStateProps = mapStateProps;
                _equalityComparer = EqualityComparer<TLocalState>.Default;
                _disposable = UniReduxProvider.GetStore<TState>().Subscribe(OnNext);
                _actionDispatcher = mapDispatchProps?.Invoke(UniReduxProvider.GetStore<TState>().Dispatch);
            }

            private void OnNext(VoidMessage v)
            {
                var localState = _mapStateProps.Invoke(UniReduxProvider.GetStore<TState>().GetState());
                if (_equalityComparer.Equals(localState, _localState)) return;
                _localState = localState;
                lock (_syncRoot)
                {
                    _listObserver.OnNext(_localState);
                }
            }

            private sealed class Subscription : IDisposable
            {
                private readonly object _disposeLock = new object();
                private Container<TLocalState, TActionDispatcher> _parent;
                private IObserver<TLocalState> _unsubscribeTarget;

                public Subscription(Container<TLocalState, TActionDispatcher> parent,
                    IObserver<TLocalState> unsubscribeTarget)
                {
                    _parent = parent;
                    _unsubscribeTarget = unsubscribeTarget;
                }

                public void Dispose()
                {
                    lock (_disposeLock)
                    {
                        if (_parent == null) return;
                        lock (_parent._syncRoot)
                        {
                            if (_unsubscribeTarget != null)
                            {
                                if (_parent._listObserver == null) return;
                                _parent._listObserver = _parent._listObserver.Remove(_unsubscribeTarget);
                            }

                            _unsubscribeTarget = null;
                            _parent = null;
                        }
                    }
                }
            }
        }
    }

    public static class UniReduxContainer
    {
        public static IUniReduxContainer Connect()
        {
            return new Container();
        }

        public static IUniReduxContainer Connect<TParentContainerState, TLocalState>(
            this IUniReduxContainer parentContainer,
            MapStateProps<TParentContainerState, TLocalState> mapStateToProps)
            where TParentContainerState : class
            where TLocalState : class
        {
            return new InnerContainer<TParentContainerState, TLocalState>(parentContainer, mapStateToProps);
        }

        public static IUniReduxContainer Connect<TParentContainerState, TLocalState, TActionDispatcher>(
            this IUniReduxContainer parentContainer,
            MapStateProps<TParentContainerState, TLocalState> mapStateToProps,
            MapDispatchProps<TActionDispatcher> mapDispatchToProps)
            where TParentContainerState : class where TLocalState : class where TActionDispatcher : class
        {
            return new InnerContainer<TParentContainerState, TLocalState, TActionDispatcher>(parentContainer,
                mapStateToProps, mapDispatchToProps);
        }

        internal class Container : IUniReduxContainer
        {
            public virtual void Dispose()
            {
            }

            public virtual IDisposable Inject(IUniReduxComponent component)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) return null;
#endif
                component.InjectDispatcher();
                return Disposable.Empty;
            }
        }

        private class InnerContainer<TParentContainerState, TLocalState> : Container
            where TParentContainerState : class where TLocalState : class
        {
            private readonly MapStateProps<TParentContainerState, TLocalState> _mapStateProps;

            private readonly IEqualityComparer<TLocalState> _equalityComparer;

            private readonly object _syncRoot = new object();
            private ListObserver<TLocalState> _listObserver = ListObserver.Empty<TLocalState>();

            private readonly IDisposable _disposable;

            private TLocalState _localState;

            public override IDisposable Inject(IUniReduxComponent component)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) return null;
#endif
                component.InjectDispatcher();

                var observer = new Observer<TLocalState>(component);
                try
                {
                    lock (_syncRoot)
                    {
                        _listObserver = _listObserver.Add(observer);
                    }

                    observer.OnNext(_localState);

                    return new Subscription(this, observer);
                }
                catch (Exception e)
                {
                    lock (_syncRoot)
                    {
                        _listObserver.OnError(e);
                        _listObserver = ListObserver.Empty<TLocalState>();
                    }

                    return new Subscription(this, EmptyObserver<TLocalState>.Instance);
                }
            }

            public override void Dispose()
            {
                lock (_syncRoot)
                {
                    _listObserver.OnCompleted();
                    _listObserver = ListObserver.Empty<TLocalState>();
                }

                _disposable?.Dispose();
            }

            public InnerContainer(IUniReduxContainer parentContainer,
                MapStateProps<TParentContainerState, TLocalState> mapStateProps)
            {
                _mapStateProps = mapStateProps;
                _equalityComparer = EqualityComparer<TLocalState>.Default;

                var observable = parentContainer as IUniReduxContainerObservable<TParentContainerState>;
                if (observable != null && mapStateProps != null)
                {
                    _disposable = observable.Subscribe(
                        Rx.Observer.CreateSubscribeObserver<TParentContainerState>(OnNext, Stubs.Throw, Stubs.Nop)
                    );
                }
            }

            private void OnNext(TParentContainerState container)
            {
                var localState = _mapStateProps.Invoke(container);
                if (_equalityComparer.Equals(localState, _localState)) return;
                _localState = localState;
                lock (_syncRoot)
                {
                    _listObserver.OnNext(_localState);
                }
            }

            private sealed class Subscription : IDisposable
            {
                private readonly object _disposeLock = new object();
                private InnerContainer<TParentContainerState, TLocalState> _parent;
                private IObserver<TLocalState> _unsubscribeTarget;

                public Subscription(InnerContainer<TParentContainerState, TLocalState> parent,
                    IObserver<TLocalState> unsubscribeTarget)
                {
                    _parent = parent;
                    _unsubscribeTarget = unsubscribeTarget;
                }

                public void Dispose()
                {
                    lock (_disposeLock)
                    {
                        if (_parent == null) return;
                        lock (_parent._syncRoot)
                        {
                            if (_unsubscribeTarget != null)
                            {
                                if (_parent._listObserver == null) return;
                                _parent._listObserver = _parent._listObserver.Remove(_unsubscribeTarget);
                            }

                            _unsubscribeTarget = null;
                            _parent = null;
                        }
                    }
                }
            }
        }

        private class InnerContainer<TParentContainerState, TLocalState, TActionDispatcher> :
            Container,
            IUniReduxContainerObservable<TLocalState>
            where TParentContainerState : class
            where TLocalState : class
            where TActionDispatcher : class
        {
            private readonly MapStateProps<TParentContainerState, TLocalState> _mapStateProps;

            private readonly IEqualityComparer<TLocalState> _equalityComparer;

            private readonly TActionDispatcher _actionDispatcher;

            private readonly object _syncRoot = new object();
            private ListObserver<TLocalState> _listObserver = ListObserver.Empty<TLocalState>();

            private readonly IDisposable _disposable;

            private TLocalState _localState;

            public override IDisposable Inject(IUniReduxComponent component)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) return null;
#endif
                component.InjectDispatcher();

                var componentType = component.GetType();
                var componentProperties = componentType
                    .GetUniReduxInjectProperties();

                component.InjectActionDispatcher(componentProperties, _actionDispatcher);

                var observer = new Observer<TLocalState>(component);
                return InnerSubscribe(observer);
            }

            public override void Dispose()
            {
                lock (_syncRoot)
                {
                    _listObserver.OnCompleted();
                    _listObserver = ListObserver.Empty<TLocalState>();
                }

                _disposable?.Dispose();
            }

            IDisposable IUniReduxContainerObservable<TLocalState>.Subscribe(IObserver<TLocalState> observer)
            {
                return InnerSubscribe(observer);
            }

            private IDisposable InnerSubscribe(IObserver<TLocalState> observer)
            {
                try
                {
                    lock (_syncRoot)
                    {
                        _listObserver = _listObserver.Add(observer);
                    }

                    observer.OnNext(_localState);

                    return new Subscription(this, observer);
                }
                catch (Exception e)
                {
                    lock (_syncRoot)
                    {
                        _listObserver.OnError(e);
                        _listObserver = ListObserver.Empty<TLocalState>();
                    }

                    return new Subscription(this, EmptyObserver<TLocalState>.Instance);
                }
            }

            public InnerContainer(IUniReduxContainer parentContainer,
                MapStateProps<TParentContainerState, TLocalState> mapStateProps,
                MapDispatchProps<TActionDispatcher> mapDispatchProps)
            {
                if (mapStateProps == null) return;
                _mapStateProps = mapStateProps;
                _equalityComparer = EqualityComparer<TLocalState>.Default;
                _actionDispatcher =
                    mapDispatchProps?.Invoke(UniReduxProvider.GetStore<TParentContainerState>().Dispatch);

                var observable = parentContainer as IUniReduxContainerObservable<TParentContainerState>;
                if (observable != null)
                {
                    _disposable = observable.Subscribe(
                        Rx.Observer.CreateSubscribeObserver<TParentContainerState>(OnNext, Stubs.Throw, Stubs.Nop)
                    );
                }
            }

            private void OnNext(TParentContainerState container)
            {
                var localState = _mapStateProps.Invoke(container);
                if (_equalityComparer.Equals(localState, _localState)) return;
                _localState = localState;
                lock (_syncRoot)
                {
                    _listObserver.OnNext(_localState);
                }
            }

            private sealed class Subscription : IDisposable
            {
                private readonly object _disposeLock = new object();
                private InnerContainer<TParentContainerState, TLocalState, TActionDispatcher> _parent;
                private IObserver<TLocalState> _unsubscribeTarget;

                public Subscription(InnerContainer<TParentContainerState, TLocalState, TActionDispatcher> parent,
                    IObserver<TLocalState> unsubscribeTarget)
                {
                    _parent = parent;
                    _unsubscribeTarget = unsubscribeTarget;
                }

                public void Dispose()
                {
                    lock (_disposeLock)
                    {
                        if (_parent == null) return;
                        lock (_parent._syncRoot)
                        {
                            if (_unsubscribeTarget != null)
                            {
                                if (_parent._listObserver == null) return;
                                _parent._listObserver = _parent._listObserver.Remove(_unsubscribeTarget);
                            }

                            _unsubscribeTarget = null;
                            _parent = null;
                        }
                    }
                }
            }
        }

        internal sealed class Observer<TLocalState> : IObserver<TLocalState>
        {
            private readonly IUniReduxComponent _component;

            private bool _isCompleted;

            public Observer(IUniReduxComponent component)
            {
                _component = component;
            }

            public void OnCompleted()
            {
                _isCompleted = true;
            }

            public void OnError(Exception error)
            {
                _isCompleted = true;
                throw Assert.CreateException($"{error}");
            }

            public void OnNext(TLocalState value)
            {
                if (_isCompleted) return;

                ComponentLocator.SetValue(_component, value);
            }
        }
    }

    public delegate TLocalState MapStateProps<in TState, out TLocalState>(TState state);

    public delegate TActionDispatcher MapDispatchProps<out TActionDispatcher>(Dispatcher dispatcher);

    public interface IUniReduxContainer : IDisposable
    {
        /// <summary>
        /// Inject component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        IDisposable Inject(IUniReduxComponent component);
    }

    internal interface IUniReduxContainerObservable<out TState>
    {
        IDisposable Subscribe(IObserver<TState> observer);
    }
}