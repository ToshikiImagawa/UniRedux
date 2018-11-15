namespace UniRedux.Provider
{
    public interface IContainerBundle
    {
        void SetUniReduxContainer(string containerName, IUniReduxContainer container);
    }
}