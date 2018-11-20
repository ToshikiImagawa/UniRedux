using UnityEngine;
using Zenject;

namespace UniRedux.Zenject.Examples.Simple
{
    [CreateAssetMenu(fileName = "SimpleToDoInstaller", menuName = "Installers/SimpleToDoInstaller")]
    public class SimpleToDoInstaller : ScriptableObjectInstaller<SimpleToDoInstaller>
    {
        [SerializeField] private ToDoElement _toDoElement;

        public override void InstallBindings()
        {
            ToDoStateInstaller.Install(Container);
            Container.BindMemoryPool<ToDoElement, ToDoElement.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_toDoElement)
                .UnderTransformGroup("ToDoElement");
        }
    }
}