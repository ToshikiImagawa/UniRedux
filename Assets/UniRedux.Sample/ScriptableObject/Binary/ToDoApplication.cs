using UnityEngine;

namespace UniRedux.Sample.ScriptableObject.Binary
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
