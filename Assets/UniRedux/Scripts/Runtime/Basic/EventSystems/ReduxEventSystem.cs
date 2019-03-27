using System;
using System.Collections.Generic;
using System.Linq;
using UniSystem.Reactive.Disposables;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRedux.EventSystems
{
    [DisallowMultipleComponent]
    public abstract class ReduxEventSystem : MonoBehaviour
    {
        private readonly object _handlerLock = new object();
        private readonly IDictionary<int, GameObject> _targetObjects = new Dictionary<int, GameObject>();
        private readonly IDictionary<Type, HashSet<int>> _eventHandlerIndexes = new Dictionary<Type, HashSet<int>>();
        private readonly IDictionary<int, HashSet<Type>> _eventHandlerCounter = new Dictionary<int, HashSet<Type>>();

        /// <summary>
        /// Register the component that will receive the Event
        /// </summary>
        /// <param name="targetComponent"></param>
        /// <returns></returns>
        protected IDisposable RegisterComponent(Component targetComponent)
        {
            if (targetComponent == null) return Disposable.Empty;
            if (!(targetComponent is IReduxEventSystemHandler)) return Disposable.Empty;
            lock (_handlerLock)
            {
                var instanceId = targetComponent.gameObject.GetInstanceID();
                if (!_targetObjects.ContainsKey(instanceId))
                    _targetObjects[instanceId] = targetComponent.gameObject;
                else if (_targetObjects[instanceId] == null)
                    _targetObjects[instanceId] = targetComponent.gameObject;

                if (!_eventHandlerCounter.ContainsKey(instanceId))
                    _eventHandlerCounter[instanceId] = new HashSet<Type>();

                foreach (var targetInterface in targetComponent.GetType().GetInterfaces())
                {
                    if (!typeof(IReduxEventSystemHandler).IsAssignableFrom(targetInterface)) continue;
                    if (typeof(IReduxEventSystemHandler) == targetInterface) continue;
                    if (!_eventHandlerIndexes.ContainsKey(targetInterface))
                        _eventHandlerIndexes[targetInterface] = new HashSet<int>();

                    _eventHandlerIndexes[targetInterface].Add(instanceId);
                    _eventHandlerCounter[instanceId].Add(targetInterface);
                }
            }

            return new Subscription(this, targetComponent);
        }

        /// <summary>
        /// Determines registered an component is in the EventSystem.
        /// </summary>
        /// <param name="targetComponent"></param>
        /// <returns></returns>
        protected bool IsRegistered(Component targetComponent)
        {
            return _targetObjects.ContainsKey(targetComponent.gameObject.GetInstanceID());
        }

        protected void InnerExecute(Type key, Action<GameObject, BaseEventData> executeAction,
            BaseEventData data)
        {
            lock (_handlerLock)
            {
                if (!_eventHandlerIndexes.ContainsKey(key)) return;
                var executeObjectIds = _eventHandlerIndexes[key];
                foreach (var id in executeObjectIds.ToArray())
                {
                    if (_targetObjects.ContainsKey(id))
                    {
                        executeAction.Invoke(_targetObjects[id], data);
                    }
                    else
                    {
                        _eventHandlerIndexes[key].Remove(id);
                    }
                }
            }
        }

        protected int InnerTargetObjectsCount(Type key)
        {
            lock (_handlerLock)
            {
                return _eventHandlerIndexes.ContainsKey(key) ? _eventHandlerIndexes[key].Count : 0;
            }
        }

        protected internal virtual void OnDestroy()
        {
            lock (_handlerLock)
            {
                _targetObjects.Clear();
                _eventHandlerIndexes.Clear();
                _eventHandlerCounter.Clear();
            }
        }


#if UNITY_EDITOR
        public IEnumerable<GameObject> TargetObjects => _targetObjects.Select(pair => pair.Value).ToArray();
#endif

        private class Subscription : IDisposable
        {
            private readonly object _disposeLock = new object();
            private Component _component;
            private ReduxEventSystem _parent;

            public Subscription(ReduxEventSystem parent, Component component)
            {
                _parent = parent;
                _component = component;
            }

            public void Dispose()
            {
                lock (_disposeLock)
                {
                    if (_parent == null) return;
                    if (_component == null) return;
                    if (_component.gameObject == null) return;
                    lock (_parent._handlerLock)
                    {
                        var instanceId = _component.gameObject.GetInstanceID();
                        foreach (var targetInterface in _component.GetType().GetInterfaces())
                        {
                            if (!_parent._eventHandlerIndexes.ContainsKey(targetInterface)) continue;
                            _parent._eventHandlerIndexes[targetInterface].Remove(instanceId);
                            if (_parent._eventHandlerIndexes[targetInterface].Count == 0)
                                _parent._eventHandlerIndexes.Remove(targetInterface);
                            _parent._eventHandlerCounter[instanceId].Remove(targetInterface);
                        }

                        if (_parent._eventHandlerCounter[instanceId].Count == 0)
                        {
                            _parent._eventHandlerCounter.Remove(instanceId);
                            _parent._targetObjects.Remove(instanceId);
                        }
                    }

                    _parent = null;
                    _component = null;
                }
            }
        }
    }

    /// <summary>
    /// It is possible to run ReduxEventSystem.
    /// </summary>
    public interface IExecuteReduxEventSystem
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="key"></param>
        /// <param name="executeAction"></param>
        /// <param name="data"></param>
        void Execute(Type key, Action<GameObject, BaseEventData> executeAction, BaseEventData data = null);

        /// <summary>
        /// Number of objects to be executed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int TargetObjectsCount(Type key);
    }

    /// <summary>
    /// Base class that all RxEventSystem events inherit from.
    /// </summary>
    public interface IReduxEventSystemHandler : IEventSystemHandler
    {
        /// <summary>
        /// Called when a component is registered.
        /// </summary>
        void OnRegisterComponent();
    }
}