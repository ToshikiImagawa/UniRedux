using UnityEngine;

namespace UniRedux.Sample.ScriptableStore.Simple
{
    [CreateAssetMenu(fileName = "ToDoApplication", menuName = "UniRedux/ToDoApplication", order = 1)]
    public class ToDoApplication : Application<ToDoState>
    {
        protected override IStore<ToDoState> InitStore
        {
            get
            {
                return Redux.CreateStore(ToDoReducer.Execute, ToDoReducer.InitState, UniReduxMiddleware.Logger, UniReduxMiddleware.CheckImmutableUpdate);
            }
        }
    }
}
