using UnityEngine;

namespace UniRedux.Examples.Application
{
    [CreateAssetMenu(fileName = "UniReduxApplication", menuName = "UniRedux/Applications/ToDoApplication")]
    public class ToDoApplication : UniReduxApplication<ToDoState>
    {
        protected override IStore<ToDoState> CrateStore => Redux.CreateStore(
            ToDoReducer.Execute, ToDoReducer.InitState,
            UniReduxMiddleware.Logger,
            UniReduxMiddleware.CheckImmutableUpdate
        );

        protected override void BeforeInit()
        {
            UniReduxEqualityComparer.SetEqualityComparer(new ToDoListEqualityComparer());
        }
    }
}