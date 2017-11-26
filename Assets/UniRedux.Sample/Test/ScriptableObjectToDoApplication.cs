using UnityEngine;

namespace UniRedux.Sample.Test
{
    [CreateAssetMenu(fileName = "TestToDoApplication", menuName = "UniRedux/Test/ToDoApplication", order = 1)]
    public class ScriptableObjectToDoApplication : Application<ToDoState>
    {
        protected override IStore<ToDoState> InitStore => Redux.CreateStore(ToDoReducer.Execute, ToDoReducer.InitState);
    }
}
