using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRedux.EventSystems
{
    public sealed class SceneEventSystem : ReduxEventSystem, IExecuteReduxEventSystem
    {
        private bool _isInit;
        private IDisposable _projectRegisterDisposable;

        private static readonly Dictionary<string, Queue<Action<SceneEventSystem>>> ExecuteRxEventSystemQueue =
            new Dictionary<string, Queue<Action<SceneEventSystem>>>();

        private static readonly Dictionary<string, int> SceneEventSystemCache =
            new Dictionary<string, int>();

        private static readonly object ExecuteRxEventSystemQueueLock = new object();

        /// <summary>
        /// Subscribe RxEventSystem (RxBehaviour is called implicitly)
        /// </summary>
        /// <param name="component"></param>
        /// <param name="listener"></param>
        public static void SubscribeEventSystem<TComponent>(TComponent component, Action<IDisposable> listener)
            where TComponent : Component, IReduxEventSystemHandler
        {
            if (component is IReduxEventSystemSubscriber) return;
            InternalSubscribeEventSystem(component, listener);
        }

        /// <summary>
        /// Subscribe RxEventSystem
        /// </summary>
        /// <param name="gameObject"></param>
        public static void SubscribeEventSystem(GameObject gameObject)
        {
            if (gameObject == null) return;

            var currentComponent = gameObject.GetComponent(typeof(IReduxEventSystemSubscriber));
            if (currentComponent != null)
            {
                var rxBehaviour = currentComponent as IReduxEventSystemSubscriber;
                if (rxBehaviour != null)
                {
                    InternalSubscribeEventSystem(currentComponent,
                        disposable => { rxBehaviour.SubscribeEventSystem(disposable); }
                    );
                }
            }

            var components = gameObject.GetComponentsInChildren(typeof(IReduxEventSystemSubscriber));
            foreach (var component in components)
            {
                var rxBehaviour = component as IReduxEventSystemSubscriber;
                if (rxBehaviour == null) continue;
                InternalSubscribeEventSystem(component,
                    disposable => { rxBehaviour.SubscribeEventSystem(disposable); }
                );
            }
        }

        internal static void InternalSubscribeEventSystem(Component component, Action<IDisposable> listener)
        {
            var handler = component as IReduxEventSystemHandler;
            if (handler == null) return;

            var name = component.gameObject.scene.name;
            if (name == "DontDestroyOnLoad")
            {
                ProjectEventSystem.InternalSubscribeEventSystem(component, listener);
                return;
            }

            if (SceneEventSystemCache.ContainsKey(name))
            {
                SceneEventSystem instance;
                if (ProjectEventSystem.TryGetExecuteReduxEventSystem(SceneEventSystemCache[name], out instance))
                {
                    if (instance.IsRegistered(component)) return;
                    listener?.Invoke(instance.RegisterComponent(component));
                    handler.OnRegisterComponent();
                    return;
                }
            }

            lock (ExecuteRxEventSystemQueueLock)
            {
                if (!ExecuteRxEventSystemQueue.ContainsKey(name))
                {
                    ExecuteRxEventSystemQueue[name] = new Queue<Action<SceneEventSystem>>();
                }

                ExecuteRxEventSystemQueue[name].Enqueue(reduxEventSystem =>
                {
                    if (reduxEventSystem == null) return;
                    listener?.Invoke(reduxEventSystem.RegisterComponent(component));
                    handler.OnRegisterComponent();
                });
            }
        }

        void IExecuteReduxEventSystem.Execute(Type key, Action<GameObject, BaseEventData> executeAction,
            BaseEventData data)
        {
            InnerExecute(key, executeAction, data);
        }

        int IExecuteReduxEventSystem.TargetObjectsCount(Type key)
        {
            return InnerTargetObjectsCount(key);
        }

        private void Awake()
        {
            var sceneName = gameObject.scene.name;
            SceneEventSystem instance;
            if (ProjectEventSystem.TryGetExecuteReduxEventSystem(gameObject.GetInstanceID(), out instance))
            {
                DestroyImmediate(gameObject);
                return;
            }

            _isInit = true;
            _projectRegisterDisposable = ProjectEventSystem.RegisterExecuteReduxEventSystem(this);
            var executeRxEventSystemQueue = new Queue<Action<SceneEventSystem>>();
            lock (ExecuteRxEventSystemQueueLock)
            {
                SceneEventSystemCache[sceneName] = gameObject.GetInstanceID();
                if (ExecuteRxEventSystemQueue.ContainsKey(sceneName))
                {
                    executeRxEventSystemQueue = ExecuteRxEventSystemQueue[sceneName];
                    ExecuteRxEventSystemQueue.Remove(sceneName);
                }
            }

            while (executeRxEventSystemQueue.Count != 0) executeRxEventSystemQueue.Dequeue()?.Invoke(this);
            var components = gameObject.scene.GetRootGameObjects()?.SelectMany(obj
                                 => obj.GetComponentsInChildren(typeof(IReduxEventSystemSubscriber))
                             ).ToArray() ?? Array.Empty<Component>();
            foreach (var component in components)
            {
                var rxBehaviour = component as IReduxEventSystemSubscriber;
                if (rxBehaviour == null) continue;
                InternalSubscribeEventSystem(component,
                    disposable => { rxBehaviour.SubscribeEventSystem(disposable); }
                );
            }
        }

        protected internal override void OnDestroy()
        {
            if (!_isInit) return;
            var sceneName = gameObject.scene.name;
            lock (ExecuteRxEventSystemQueueLock)
            {
                SceneEventSystemCache.Remove(sceneName);
                ExecuteRxEventSystemQueue.Remove(sceneName);
            }

            _projectRegisterDisposable?.Dispose();
            base.OnDestroy();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UniRedux/SceneEventSystem", false, priority = 10)]
        public static void CreateSceneEventSystem()
        {
            var gameObject = new GameObject("SceneEventSystem");
            gameObject.AddComponent<SceneEventSystem>();
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
        }
#endif
    }
}