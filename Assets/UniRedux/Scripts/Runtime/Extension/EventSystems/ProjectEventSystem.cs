using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UniRedux.EventSystems
{
    public sealed class ProjectEventSystem : ReduxEventSystem, IExecuteReduxEventSystem
    {
        private readonly Dictionary<int, IExecuteReduxEventSystem> _executeReduxEventSystems =
            new Dictionary<int, IExecuteReduxEventSystem>();

        private readonly object _executeReduxEventSystemsLock = new object();

        private bool _isInit;
        private static ProjectEventSystem _instance;

        private static ProjectEventSystem Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = SceneManager.GetActiveScene().GetRootGameObjects()
                    ?.FirstOrDefault(obj => obj.GetComponent<ProjectEventSystem>() != null)
                    ?.GetComponent<ProjectEventSystem>();

                if (_instance == null)
                {
                    var prefab = Resources.Load<ProjectEventSystem>("ProjectEventSystem");
                    _instance = prefab != null
                        ? Instantiate(prefab)
                        : new GameObject().AddComponent<ProjectEventSystem>();
                }

                DontDestroyOnLoad(_instance);
                _instance.name = "ProjectEventSystem";

                var rxBehaviours = _instance.gameObject.scene.GetRootGameObjects()?.SelectMany(obj
                                       => obj.GetComponentsInChildren<IReduxEventSystemSubscriber>()
                                   ).ToArray() ?? Array.Empty<IReduxEventSystemSubscriber>();
                foreach (var rxBehaviour in rxBehaviours)
                {
                    if (rxBehaviour == null) continue;
                    InternalSubscribeEventSystem(rxBehaviour as Component,
                        disposable => { rxBehaviour.SubscribeEventSystem(disposable); }
                    );
                }

                return _instance;
            }
        }

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

            {
                var rxBehaviour = gameObject.GetComponent<IReduxEventSystemSubscriber>();
                if (rxBehaviour != null)
                {
                    InternalSubscribeEventSystem(rxBehaviour as Component,
                        disposable => { rxBehaviour.SubscribeEventSystem(disposable); }
                    );
                }
            }

            foreach (var rxBehaviour in gameObject.GetComponentsInChildren<IReduxEventSystemSubscriber>())
            {
                if (rxBehaviour == null) continue;
                InternalSubscribeEventSystem(rxBehaviour as Component,
                    disposable => { rxBehaviour.SubscribeEventSystem(disposable); }
                );
            }
        }

        internal static void InternalSubscribeEventSystem(Component component, Action<IDisposable> listener)
        {
            var handler = component as IReduxEventSystemHandler;
            if (handler == null) return;
            if (Instance.IsRegistered(component)) return;
            listener?.Invoke(Instance.RegisterComponent(component));
            handler.OnRegisterComponent();
        }

        /// <summary>
        /// Register ExecuteReduxEventSystem
        /// </summary>
        /// <param name="executeReduxEventSystem"></param>
        /// <typeparam name="TExecuteReduxEventSystem"></typeparam>
        /// <returns></returns>
        internal static IDisposable RegisterExecuteReduxEventSystem<TExecuteReduxEventSystem>(
            TExecuteReduxEventSystem executeReduxEventSystem)
            where TExecuteReduxEventSystem : Component, IExecuteReduxEventSystem
        {
            return Instance._RegisterExecuteReduxEventSystem(executeReduxEventSystem);
        }

        /// <summary>
        /// Gets the ExecuteReduxEventSystem associated with the specified key.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="executeReduxEventSystem"></param>
        /// <typeparam name="TExecuteReduxEventSystem"></typeparam>
        /// <returns></returns>
        internal static bool TryGetExecuteReduxEventSystem<TExecuteReduxEventSystem>(int instanceId,
            out TExecuteReduxEventSystem executeReduxEventSystem)
            where TExecuteReduxEventSystem : Component, IExecuteReduxEventSystem
        {
            executeReduxEventSystem = null;
            if (!Instance._executeReduxEventSystems.ContainsKey(instanceId)) return false;
            executeReduxEventSystem = Instance._executeReduxEventSystems[instanceId] as TExecuteReduxEventSystem;
            return executeReduxEventSystem != null;
        }

        void IExecuteReduxEventSystem.Execute(Type key, Action<GameObject, BaseEventData> executeAction,
            BaseEventData data)
        {
            InnerExecute(key, executeAction, data);
            foreach (var executeReduxEventSystem in _executeReduxEventSystems)
            {
                executeReduxEventSystem.Value.Execute(key, executeAction, data);
            }
        }

        int IExecuteReduxEventSystem.TargetObjectsCount(Type key)
        {
            return InnerTargetObjectsCount(key) +
                   _executeReduxEventSystems.Sum(eventSystem => eventSystem.Value.TargetObjectsCount(key));
        }

        private IDisposable _RegisterExecuteReduxEventSystem<TExecuteReduxEventSystem>(
            TExecuteReduxEventSystem executeReduxEventSystem)
            where TExecuteReduxEventSystem : Component, IExecuteReduxEventSystem
        {
            lock (_executeReduxEventSystemsLock)
            {
                _executeReduxEventSystems[executeReduxEventSystem.gameObject.GetInstanceID()] = executeReduxEventSystem;
            }

            return new Subscription(this, executeReduxEventSystem);
        }

        private class Subscription : IDisposable
        {
            private readonly object _disposeLock = new object();
            private Component _component;
            private ProjectEventSystem _parent;

            public Subscription(ProjectEventSystem parent, Component component)
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
                    lock (_parent._executeReduxEventSystemsLock)
                    {
                        _parent._executeReduxEventSystems.Remove(_component.gameObject.GetInstanceID());
                    }

                    _parent = null;
                    _component = null;
                }
            }
        }
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/UniRedux/ProjectEventSystem", priority = 30)]
        public static void CreatePrefab()
        {
            var gameObject =
                UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("ProjectEventSystem",
                    HideFlags.HideInHierarchy);
            gameObject.AddComponent<ProjectEventSystem>();
            UnityEditor.PrefabUtility.CreatePrefab("Assets/Resources/ProjectEventSystem.prefab", gameObject);
            DestroyImmediate(gameObject);
        }
#endif
    }
}