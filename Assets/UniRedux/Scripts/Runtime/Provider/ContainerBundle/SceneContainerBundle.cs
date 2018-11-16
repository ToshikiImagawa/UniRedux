using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniRedux.Provider
{
    public sealed class SceneContainerBundle : MonoBehaviour, IContainerBundle
    {
        [SerializeField] private MonoContainerInstaller[] monoContainerInstaller;
        [SerializeField] private ScriptableObjectContainerInstaller[] scriptableObjectContainerInstaller;

        private IUniReduxComponent[] _uniReduxComponents;
        private readonly object _containersLock = new object();

        private readonly Dictionary<string, IUniReduxContainer> _containers =
            new Dictionary<string, IUniReduxContainer>();

#if UNITY_EDITOR
        [SerializeField] private List<BindComponent> bindComponents = new List<BindComponent>();
#endif

        private void Awake()
        {
            foreach (var containerInstaller in monoContainerInstaller)
            {
                containerInstaller.InstallBindings(this);
            }

            foreach (var containerInstaller in scriptableObjectContainerInstaller)
            {
                containerInstaller.InstallBindings(this);
            }

            InjectComponent();
        }

        private void InjectComponent()
        {
            var uniReduxComponents = SceneManager.GetActiveScene().GetRootGameObjects()?.SelectMany(
                                             obj => obj.GetComponentsInChildren<IUniReduxComponent>(true))
                                         .ToArray() ?? Array.Empty<IUniReduxComponent>();
            foreach (var uniReduxComponent in uniReduxComponents)
            {
                var attribute = uniReduxComponent.GetType()
                    .GetCustomAttributes(typeof(BindUniReduxContainerAttribute), true)
                    .FirstOrDefault() as BindUniReduxContainerAttribute;
                if (attribute == null) continue;
                foreach (var containerName in attribute.ContainerNames)
                {
                    if (string.IsNullOrEmpty(containerName)) continue;
                    lock (_containersLock)
                    {
                        if (_containers.ContainsKey(containerName))
                        {
                            _containers[containerName].Inject(uniReduxComponent);
#if UNITY_EDITOR
                            bindComponents.Add(
                                new BindComponent
                                {
                                    ContainerName = containerName,
                                    Component = uniReduxComponent as Component
                                }
                            );
#endif
                            continue;
                        }
                    }

                    ProjectContainerBundle.InjectComponent(containerName, uniReduxComponent);
                }
            }
        }

        void IContainerBundle.SetUniReduxContainer(string containerName, IUniReduxContainer container)
        {
            lock (_containersLock)
            {
                _containers[containerName] = container;
            }
        }

        private void OnDestroy()
        {
            lock (_containersLock)
            {
                foreach (var uniReduxContainer in _containers)
                {
                    uniReduxContainer.Value.Dispose();
                }

                _containers.Clear();
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UniRedux/SceneContainerBundle", false, priority = 10)]
        public static void CreateSceneEventSystem()
        {
            var gameObject = new GameObject("SceneContainerBundle");
            gameObject.AddComponent<SceneContainerBundle>();
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
        }

        [Serializable]
        public struct BindComponent
        {
            [HideInInspector] public string ContainerName;
            public Component Component;
        }
#endif
    }
}