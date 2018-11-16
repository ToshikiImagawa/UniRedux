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
        [SerializeField] private string[] parentSceneNames = Array.Empty<string>();

        private IUniReduxComponent[] _uniReduxComponents;
        private readonly object _containersLock = new object();

        private readonly Dictionary<string, IUniReduxContainer> _containers =
            new Dictionary<string, IUniReduxContainer>();

        private IEnumerable<SceneContainerBundle> ParentSceneContainerBundles =>
            SceneUtil.FindAllLoadedScenes(parentSceneNames).GetSceneContainerBundles();

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
                    InjectComponent(containerName, uniReduxComponent);
                }
            }
        }

        private void InjectComponent(string containerName, IUniReduxComponent uniReduxComponent)
        {
            if (string.IsNullOrEmpty(containerName)) return;
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
                    return;
                }
            }

            var parentSceneContainerBundles = ParentSceneContainerBundles.ToArray();
            if (parentSceneContainerBundles.Length == 0)
                ProjectContainerBundle.InjectComponent(containerName, uniReduxComponent);
            else
            {
                foreach (var sceneContainerBundle in parentSceneContainerBundles)
                {
                    sceneContainerBundle.InjectComponent(containerName, uniReduxComponent);
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
    }
}