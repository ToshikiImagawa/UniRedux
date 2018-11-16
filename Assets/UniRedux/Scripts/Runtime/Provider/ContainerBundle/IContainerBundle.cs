namespace UniRedux.Provider
{
    public interface IContainerBundle
    {
        /// <summary>
        /// Set container to ContainerBundle
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="container"></param>
        void SetUniReduxContainer(string containerName, IUniReduxContainer container);
    }
}