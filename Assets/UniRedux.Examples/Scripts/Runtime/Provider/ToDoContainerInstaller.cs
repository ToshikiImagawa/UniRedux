using UniRedux.Examples;
using UnityEngine;

namespace UniRedux.Provider.Examples
{
    [CreateAssetMenu(fileName = "ToDoContainerInstaller", menuName = "UniRedux/ContainerInstallers/ToDoContainerInstaller")]
    public class ToDoContainerInstaller : ScriptableObjectContainerInstaller
    {
        public override void InstallBindings(IContainerBundle containerBundle)
        {
            UniReduxProvider.SetSetting(new ToDoProviderSetting());
            containerBundle.SetUniReduxContainer(
                "ToDoContainer",
                UniReduxContainer<ToDoState>.Connect(
                    state => new ToDoLocalState
                    {
                        ToDos = state.ToDos,
                        Filter = state.Filter
                    }, dispatcher => new ToDoActionDispatcher(dispatcher)
                )
            );
        }
    }
}