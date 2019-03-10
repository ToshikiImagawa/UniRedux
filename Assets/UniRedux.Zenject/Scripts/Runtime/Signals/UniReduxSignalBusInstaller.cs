using Zenject;

namespace UniRedux
{
    public class UniReduxSignalBusInstaller : Installer<UniReduxSignalBusInstaller>
    {
        public override void InstallBindings()
        {
            if (Container.HasBinding<UniReduxSignalBus>()) throw Assert.CreateException("Detected multiple UniReduxSignalBus bindings.  UniReduxSignalBusInstaller should only be installed once");
            Container.BindInterfacesAndSelfTo<UniReduxSignalBus>().AsSingle().CopyIntoAllSubContainers();
            Container.BindMemoryPool<UniReduxSignalSubscription, UniReduxSignalSubscription.Pool>();
            Container.BindLateDisposableExecutionOrder<UniReduxSignalBus>(-999);
        }
    }
}
