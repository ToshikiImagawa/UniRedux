using System.Collections.Generic;
using UnityEngine;

namespace UniRedux.Provider
{
    public sealed class ProjectContainerBundle : MonoBehaviour, IContainerBundle
    {
        [SerializeField] private MonoContainerInstaller[] monoContainerInstaller;
        [SerializeField] private ScriptableObjectContainerInstaller[] scriptableObjectContainerInstaller;
#if UNITY_EDITOR
        [SerializeField] private List<BindComponent> bindComponents = new List<BindComponent>();
#endif

        private const string ProjectContainerBundlePath = "ProjectContainerBundle";

        private ProjectContainerBundle _projectContainer;
        private IUniReduxComponent[] _uniReduxComponents;
        private readonly object _containersLock = new object();

        private readonly Dictionary<string, IUniReduxContainer> _containers =
            new Dictionary<string, IUniReduxContainer>();

        internal static void InjectComponent(string containerName, IUniReduxComponent component)
        {
            if (string.IsNullOrEmpty(containerName)) return;
            if (!Instance._containers.ContainsKey(containerName)) return;
            lock (Instance._containersLock)
            {
                Instance._containers[containerName].Inject(component);
#if UNITY_EDITOR
                Instance.bindComponents.Add(
                    new BindComponent
                    {
                        ContainerName = containerName,
                        Component = component as Component
                    }
                );
#endif
            }
        }

        void IContainerBundle.SetUniReduxContainer(string containerName, IUniReduxContainer container)
        {
            lock (_containersLock)
            {
                _containers[containerName] = container;
            }
        }

        public void Awake()
        {
            if (Application.isPlaying) DontDestroyOnLoad(gameObject);
        }

        private void Initialize()
        {
            if (_containers.Count > 0) throw Assert.CreateException();
            foreach (var containerInstaller in monoContainerInstaller)
            {
                containerInstaller.InstallBindings(this);
            }

            foreach (var containerInstaller in scriptableObjectContainerInstaller)
            {
                containerInstaller.InstallBindings(this);
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
                InstantiateAndInitialize();
                return _instance;
            }
        }

        private static void InstantiateAndInitialize()
        {
            var prefabWasActive = false;
            var prefab = TryGetPrefab();
            if (prefab == null)
            {
                _instance = new GameObject("ProjectContainerBundle")
                    .AddComponent<ProjectContainerBundle>();
            }
            else
            {
                prefabWasActive = prefab.activeSelf;

                GameObject gameObjectInstance;
                if (prefabWasActive)
                {
                    prefab.SetActive(false);
                    gameObjectInstance = Instantiate(prefab);
                    prefab.SetActive(true);
                }
                else
                {
                    gameObjectInstance = Instantiate(prefab);
                }

                _instance = gameObjectInstance.GetComponent<ProjectContainerBundle>();
                if (_instance == null) throw Assert.CreateException();
                _instance.name = "ProjectContainerBundle";
            }

            _instance.Initialize();
            if (prefabWasActive)
            {
                _instance.gameObject.SetActive(true);
            }
        }

        private static GameObject TryGetPrefab()
        {
            var prefabs = Resources.LoadAll(ProjectContainerBundlePath, typeof(GameObject));

            if (prefabs.Length <= 0) return null;
            if (prefabs.Length != 1)
                throw Assert.CreateException(
                    $"Found multiple project context prefabs at resource path '{ProjectContainerBundlePath}'"
                );

            return (GameObject) prefabs[0];
        }
    }
}