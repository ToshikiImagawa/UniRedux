using UnityEngine;

namespace UniRedux.Sample.ScriptableObject.Json
{
    [CreateAssetMenu(fileName = "ToDoStore", menuName = "UniRedux/JsonToDoStore", order = 1)]
    public class ToDoScriptableStore : ScriptableStore<ToDoState>
    {
        protected override Reducer<ToDoState> InitReducer => ToDoReducer.Execute;
        protected override Middleware<ToDoState>[] InitMiddlewares { get; } = {UniReduxMiddleware.Logger};
    }
}