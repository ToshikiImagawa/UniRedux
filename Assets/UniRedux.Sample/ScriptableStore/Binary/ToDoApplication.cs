using UnityEngine;

namespace UniRedux.Sample.ScriptableStore.Binary
{
    [CreateAssetMenu(fileName = "ToDoApplication", menuName = "UniRedux/ByteToDoApplication", order = 1)]
    public class ToDoApplication : Application<ToDoState>
    {
        protected override IStore<ToDoState> InitStore
        {
            get
            {
                return Redux.CreateDeepFreezeStoreBinary(ToDoReducer.Execute, ToDoReducer.InitState, UniReduxMiddleware.Logger, UniReduxMiddleware.CheckImmutableUpdate);
            }
        }
    }
}
