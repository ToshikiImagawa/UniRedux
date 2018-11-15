namespace UniRedux.Provider
{
    public abstract class ContainerInstaller : IContainerInstaller
    {
        public abstract void InstallBindings(IContainerBundle containerBundle);
    }
}