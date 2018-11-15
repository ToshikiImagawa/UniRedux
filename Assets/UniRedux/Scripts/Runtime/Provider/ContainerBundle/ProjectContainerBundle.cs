using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniRedux.Provider
{
    public sealed class ProjectContainerBundle : MonoBehaviour, IContainerBundle
    {
        [SerializeField] private MonoContainerInstaller[] monoContainerInstaller;
        [SerializeField] private ScriptableObjectContainerInstaller[] scriptableObjectContainerInstaller;

        private ProjectContainerBundle _projectContainer;
        private IUniReduxComponent[] _uniReduxComponents;
        private readonly object _containersLock = new object();

        private readonly Dictionary<string, IUniReduxContainer> _containers =
            new Dictionary<string, IUniReduxContainer>();

        internal static void InjectComponent(string containerName, IUniReduxComponent component)
        {
            if (!Instance._containers.ContainsKey(containerName)) return;
            lock(Instance._containersLock)
            {
                Instance._containers[containerName].Inject(component);
            }
        }

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
            var uniReduxComponents = gameObject.scene.GetRootGameObjects()?.SelectMany(
                                             obj => obj.GetComponentsInChildren<IUniReduxComponent>(true))
                                         .ToArray() ?? Array.Empty<IUniReduxComponent>();
            foreach (var uniReduxComponent in uniReduxComponents)
            {
                var attribute = uniReduxComponent.GetType()
                    .GetCustomAttributes(typeof(BindUniReduxContainerAttribute), true)
                    .FirstOrDefault() as BindUniReduxContainerAttribute;
                if (attribute == null) return;
                foreach (var containerName in attribute.ContainerNames)
                {
                    if (string.IsNullOrEmpty(containerName)) continue;
                    lock (_containersLock)
                    {
                        if (!_containers.ContainsKey(containerName)) continue;
                        _containers[containerName].Inject(uniReduxComponent);
                    }
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

        private static ProjectContainerBundle _instance;

        private static ProjectContainerBundle Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = SceneManager.GetActiveScene().GetRootGameObjects()
                    ?.FirstOrDefault(obj => obj.GetComponent<ProjectContainerBundle>() != null)
                    ?.GetComponent<ProjectContainerBundle>();

                if (_instance == null)
                {
                    var prefab = Resources.Load<ProjectContainerBundle>("ProjectContainerBundle");
                    _instance = prefab != null
                        ? Instantiate(prefab)
                        : new GameObject().AddComponent<ProjectContainerBundle>();
                }

                DontDestroyOnLoad(_instance);
                _instance.name = "ProjectContainerBundle";
                return _instance;
            }
        }
    }
}