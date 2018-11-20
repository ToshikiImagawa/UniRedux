using UniRedux.Provider;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace UniRedux.Zenject.Examples.Provider.Installers
{
    [CreateAssetMenu(fileName = "ProviderToDoInstaller", menuName = "Installers/ProviderToDoInstaller")]
    public class ProviderToDoInstaller : ScriptableObjectInstaller<ProviderToDoInstaller>
    {
        [SerializeField] private ToDoElement toDoElement;

        public override void InstallBindings()
        {
            Container.BindMemoryPool<ToDoElement, ToDoElement.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(toDoElement)
                .UnderTransformGroup("ToDoElement");
            Container.Bind<IUniReduxContainer>().WithId("ToDoViewContainer")
                .FromInstance(ToDoApp.ToDoViewContainer).AsSingle();
        }
    }
}