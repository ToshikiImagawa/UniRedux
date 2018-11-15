using UnityEngine;

namespace UniRedux.Provider
{
    public abstract class MonoContainerInstaller : MonoBehaviour, IContainerInstaller
    {
        public abstract void InstallBindings(IContainerBundle containerBundle);
    }
}