using UnityEngine;

namespace UniRedux.Sample2
{
    [CreateAssetMenu(fileName = "ToDoStore", menuName = "UniRedux/JsonToDoStore", order = 1)]
    public class ToDoScriptableStore : ScriptableStore<ToDoState>
    {
        protected override Reducer<ToDoState> InitReducer => ToDoReducer.Execute;
        protected override Middleware<ToDoState>[] InitMiddlewares { get; } = {UniReduxMiddleware.Logger};
    }
}