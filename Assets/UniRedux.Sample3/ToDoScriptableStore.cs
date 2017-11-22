using UniRedux.Sample2;
using UnityEngine;

namespace UniRedux.Sample3
{
    [CreateAssetMenu(fileName = "ToDoStore", menuName = "UniRedux/Sample3", order = 1)]
    public class ToDoScriptableStore : ByteScriptableStore<ToDoState>
    {
        protected override Reducer<ToDoState> InitReducer => ToDoReducer.Execute;
        protected override Middleware<ToDoState>[] InitMiddlewares { get; } = {UniReduxMiddleware.Logger};
    }
}