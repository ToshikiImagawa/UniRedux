using Zenject;

namespace UniRedux.Zenject.Examples.Simple
{
    public class ToDoStateInstaller : Installer<ToDoStateInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IStore<ToDoState>>().FromInstance(Redux.CreateStore(
                ToDoReducer.Execute, ToDoReducer.InitState,
                UniReduxMiddleware.Logger,
                UniReduxMiddleware.CheckImmutableUpdate
            )).AsSingle();
            Container.Bind<IStore>().To<IStore<ToDoState>>().FromResolve();
        }
    }
}