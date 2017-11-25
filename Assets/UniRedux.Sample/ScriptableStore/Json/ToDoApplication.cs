using UnityEngine;

namespace UniRedux.Sample.ScriptableStore.Json
{
    [CreateAssetMenu(fileName = "ToDoApplication", menuName = "UniRedux/JsonToDoApplication", order = 1)]
    public class ToDoApplication : Application<ToDoState>
    {
        protected override IStore<ToDoState> InitStore
        {
            get
            {
                return Redux.CreateDeepFreezeStore(ToDoReducer.Execute, ToDoReducer.InitState, UniReduxMiddleware.Logger, UniReduxMiddleware.CheckImmutableUpdate);
            }
        }
    }
}
