using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniSystem.Reactive.Disposables;

namespace UniRedux.Provider
{
    public static class UniReduxContainer<TState>
    {
        private const BindingFlags PrivateAndPublicBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.FlattenHierarchy;

        private const BindingFlags PublicBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.Public |
            BindingFlags.FlattenHierarchy;

        public static IUniReduxContainer Connect()
        {
            return new Container();
        }

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

        private static void InjectDispatcher(IUniReduxComponent component)
        {
            var componentProperty = component?.GetType().GetProperty("Dispatch", PrivateAndPublicBindingFlags);
            if (
                componentProperty == null || componentProperty.PropertyType != typeof(Dispatcher) ||
                Attribute.GetCustomAttribute(componentProperty, typeof(UniReduxInjectAttribute)) == null
            ) return;
            componentProperty.GetSetMethod(true)?.Invoke(component, new object[]
            {
                (Dispatcher) UniReduxProvider.Store.Dispatch
            });
        }

        private static void InjectActionDispatcher<TActionDispatcher>(IUniReduxComponent component,
            IEnumerable<PropertyInfo> componentProperties, TActionDispatcher actionDispatcher)
            where TActionDispatcher : class
        {
            var actionDispatcherProperties = typeof(TActionDispatcher).GetProperties(PublicBindingFlags);
            var methodPairs = componentProperties.Join(actionDispatcherProperties, info => info.Name,
                    info => info.Name,
                    (componentProperty, actionDispatcherProperty)
                        => new Tuple<PropertyInfo, PropertyInfo>(
                            componentProperty, actionDispatcherProperty)
                )
                .Where(pair => pair.Item1.PropertyType == pair.Item2.PropertyType)
                .Select(pair => new Tuple<MethodInfo, MethodInfo>(
                    pair.Item1.GetSetMethod(true), pair.Item2.GetGetMethod()
                ))
                .Where(pair => pair.Item1 != null && pair.Item2 != null)
                .ToArray();
            foreach (var methodPair in methodPairs)
            {
                SetProperty(component, actionDispatcher, methodPair.Item1, methodPair.Item2);
            }
        }

        private static void SetProperty<TValue>(IUniReduxComponent component, TValue value,
            MethodInfo componentMethodInfo, MethodInfo valueMethodInfo)
        {
            if (componentMethodInfo == null || valueMethodInfo == null || component == null) return;
            componentMethodInfo.Invoke(component, new[]
            {
                valueMethodInfo.Invoke(value, null)
            });
        }

        private class Container : IUniReduxContainer
        {
            public virtual void Dispose()
            {
            }

            public virtual IDisposable Inject(IUniReduxComponent component)
            {
                InjectDispatcher(component);
                return Disposable.Empty;
            }
        }

        private class Container<TLocalState> : Container where TLocalState : class
        {
            private readonly MapStateProps<TState, TLocalState> _mapStateProps;
            private readonly IEqualityComparer<TLocalState> _equalityComparer;
            private readonly PropertyInfo[] _localStateProperties;

            private readonly object _syncRoot = new object();
            private ListObserver<TLocalState> _listObserver = ListObserver.Empty<TLocalState>();

            private readonly IDisposable _disposable;

            private TLocalState _localState;

            public override void Dispose()
            {
                lock (_syncRoot)
                {
                    _listObserver.OnCompleted();
                    _listObserver = ListObserver.Empty<TLocalState>();
                }

                _disposable?.Dispose();
            }

            public Container(MapStateProps<TState, TLocalState> mapStateProps)
            {
                _mapStateProps = mapStateProps;
                _equalityComparer = EqualityComparer<TLocalState>.Default;
                _disposable = mapStateProps == null ? null : UniReduxProvider.GetStore<TState>().Subscribe(OnNext);
                _localStateProperties = typeof(TLocalState).GetProperties(PublicBindingFlags).ToArray();
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

            public override IDisposable Inject(IUniReduxComponent component)
            {
                InjectDispatcher(component);

                var observer = new Observer(component, _localStateProperties);
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

            private sealed class Observer : IObserver<TLocalState>
            {
                private readonly IUniReduxComponent _component;
                private readonly Tuple<MethodInfo, MethodInfo>[] _methodInfoPairs;
                private bool _isCompleted;

                public Observer(IUniReduxComponent component, IEnumerable<PropertyInfo> localStateProperties)
                {
                    _component = component;
                    var componentProperties = _component.GetType().GetProperties(PrivateAndPublicBindingFlags).Where(
                        property =>
                            Attribute.GetCustomAttribute(property, typeof(UniReduxInjectAttribute)) != null).ToArray();
                    _methodInfoPairs = componentProperties.Join(
                            localStateProperties,
                            info => info.Name, info => info.Name,
                            (componentProp, localStateProp) => new Tuple<PropertyInfo, PropertyInfo>(
                                componentProp, localStateProp
                            )).Where(pair => pair.Item1.PropertyType == pair.Item2.PropertyType)
                        .Select(pair => new Tuple<MethodInfo, MethodInfo>(
                            pair.Item1.GetSetMethod(true), pair.Item2.GetGetMethod()
                        )).Where(pair => pair.Item1 != null && pair.Item2 != null)
                        .ToArray();
                }

                public void OnCompleted()
                {
                    _isCompleted = true;
                }

                public void OnError(Exception error)
                {
                    _isCompleted = true;
                    Assert.CreateException($"{error}");
                }

                public void OnNext(TLocalState value)
                {
                    if (_isCompleted) return;

                    foreach (var methodInfoPair in _methodInfoPairs)
                    {
                        SetProperty(_component, value, methodInfoPair.Item1, methodInfoPair.Item2);
                    }
                }
            }
        }

        private class Container<TLocalState, TActionDispatcher> : Container where TLocalState : class
            where TActionDispatcher : class
        {
            private readonly MapStateProps<TState, TLocalState> _mapStateProps;
            private readonly IEqualityComparer<TLocalState> _equalityComparer;
            private readonly PropertyInfo[] _localStateProperties;
            private readonly TActionDispatcher _actionDispatcher;

            private readonly object _syncRoot = new object();
            private ListObserver<TLocalState> _listObserver = ListObserver.Empty<TLocalState>();

            private readonly IDisposable _disposable;

            private TLocalState _localState;

            public override void Dispose()
            {
                lock (_syncRoot)
                {
                    _listObserver.OnCompleted();
                    _listObserver = ListObserver.Empty<TLocalState>();
                }

                _disposable?.Dispose();
            }

            public Container(MapStateProps<TState, TLocalState> mapStateProps,
                MapDispatchProps<TActionDispatcher> mapDispatchProps)
            {
                if (mapStateProps == null) return;
                _mapStateProps = mapStateProps;
                _equalityComparer = EqualityComparer<TLocalState>.Default;
                _disposable = UniReduxProvider.GetStore<TState>().Subscribe(OnNext);
                _localStateProperties = typeof(TLocalState).GetProperties(PublicBindingFlags).ToArray();
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

            public override IDisposable Inject(IUniReduxComponent component)
            {
                InjectDispatcher(component);

                var componentProperties = component.GetType().GetProperties(PrivateAndPublicBindingFlags)
                    .Where(property =>
                        Attribute.GetCustomAttribute(
                            property,
                            typeof(UniReduxInjectAttribute)
                        ) != null
                    ).ToArray();

                InjectActionDispatcher(component, componentProperties, _actionDispatcher);

                var observer = new Observer(component, _localStateProperties, componentProperties);
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

            private sealed class Observer : IObserver<TLocalState>
            {
                private readonly IUniReduxComponent _component;
                private readonly Tuple<MethodInfo, MethodInfo>[] _methodInfoPairs;
                private bool _isCompleted;

                public Observer(IUniReduxComponent component, IEnumerable<PropertyInfo> localStateProperties,
                    IEnumerable<PropertyInfo> componentProperties)
                {
                    _component = component;
                    _methodInfoPairs = componentProperties.Join(
                            localStateProperties,
                            info => info.Name, info => info.Name,
                            (componentProp, localStateProp) => new Tuple<PropertyInfo, PropertyInfo>(
                                componentProp, localStateProp
                            )).Where(pair => pair.Item1.PropertyType == pair.Item2.PropertyType)
                        .Select(pair => new Tuple<MethodInfo, MethodInfo>(
                            pair.Item1.GetSetMethod(true), pair.Item2.GetGetMethod()
                        )).Where(pair => pair.Item1 != null && pair.Item2 != null)
                        .ToArray();
                }

                public void OnCompleted()
                {
                    _isCompleted = true;
                }

                public void OnError(Exception error)
                {
                    _isCompleted = true;
                    Assert.CreateException($"{error}");
                }

                public void OnNext(TLocalState value)
                {
                    if (_isCompleted) return;

                    foreach (var methodInfoPair in _methodInfoPairs)
                    {
                        SetProperty(_component, value, methodInfoPair.Item1, methodInfoPair.Item2);
                    }
                }
            }
        }
    }

    public delegate TLocalState MapStateProps<in TState, out TLocalState>(TState state);

    public delegate TActionDispatcher MapDispatchProps<out TActionDispatcher>(Dispatcher dispatcher);

    public interface IUniReduxComponent
    {
    }

    public interface IUniReduxContainer : IDisposable
    {
        IDisposable Inject(IUniReduxComponent component);
    }
}