using UnityEngine;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal.Installers
{
    public class SignalToDoViewInstaller : MonoInstaller
    {
        [SerializeField] private ToDoElement toDoElement;
        [SerializeField] private Transform createPoint;

        public override void InstallBindings()
        {
            Container.BindMemoryPool<ToDoElement, ToDoElement.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(toDoElement)
                .UnderTransformGroup("ToDoElement");
            Container.BindInterfacesAndSelfTo<ToDoView>().AsSingle();
            Container.BindInstance(createPoint).WhenInjectedInto<ToDoElement.Pool>();
        }
    }
}
