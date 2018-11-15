using UnityEngine;

namespace UniRedux.Provider
{
    public abstract class ScriptableObjectContainerInstaller : ScriptableObject, IContainerInstaller
    {
        public abstract void InstallBindings(IContainerBundle containerBundle);
    }
}