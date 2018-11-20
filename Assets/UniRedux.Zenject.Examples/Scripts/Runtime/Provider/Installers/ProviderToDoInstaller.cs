using UniRedux.Provider;
using UnityEngine;
using Zenject;

namespace UniRedux.Zenject.Examples.Provider.Installers
{
    [CreateAssetMenu(fileName = "ProviderToDoInstaller", menuName = "Installers/ProviderToDoInstaller")]
    public class ProviderToDoInstaller : ScriptableObjectInstaller<ProviderToDoInstaller>
    {
        [SerializeField] private ToDoElement _toDoElement;

        public override void InstallBindings()
        {
            Container.BindMemoryPool<ToDoElement, ToDoElement.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_toDoElement)
                .UnderTransformGroup("ToDoElement");
            Container.Bind<IUniReduxContainer>().WithId("ToDoViewContainer")
                .FromInstance(ToDoApp.ToDoViewContainer);
        }
    }
}