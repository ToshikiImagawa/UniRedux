using UnityEngine;

namespace UniRedux.Sample2
{
    [CreateAssetMenu]
    public class ToDoScriptableStore : UniRedux.ScriptableStore<ToDoState>
    {
        protected override Reducer<ToDoState> InitReducer => ToDoReducer.Execute;
        protected override Middleware<ToDoState>[] InitMiddlewares { get; } = {UniReduxMiddleware.Logger};
    }
}