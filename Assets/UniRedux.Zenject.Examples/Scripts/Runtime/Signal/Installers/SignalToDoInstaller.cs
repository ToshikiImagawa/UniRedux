using UnityEngine;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal.Installers
{
    public class SignalToDoInstaller : MonoInstaller
    {
        [SerializeField] private ToDoElement toDoElement;
        [SerializeField] private Transform createPoint;

        public override void InstallBindings()
        {
            UniReduxSignalBusInstaller.Install(Container);
            Container.Bind<IStore>().FromResolveGetter<ToDoApp>(app => app.Store).AsSingle();
            Container.DeclareUniReduxSignal<ToDoFilter, ToDoState>(state => state.Filter);
            Container.DeclareUniReduxSignal<ToDo[], ToDoState>(state => state.ToDos);

            Container.BindMemoryPool<ToDoElement, ToDoElement.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(toDoElement)
                .UnderTransformGroup("ToDoElement");
            Container.Bind<ToDoApp>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ToDoView>().AsSingle();
            Container.BindInstance(createPoint).WhenInjectedInto<ToDoElement.Pool>();
        }
    }
}