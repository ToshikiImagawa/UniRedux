using UnityEngine;

namespace UniRedux.Sample.Test
{
    [CreateAssetMenu(fileName = "TestJsonToDoApplication", menuName = "UniRedux/Test/JsonToDoApplication", order = 2)]
    public class ScriptableObjectJsonToDoApplication : Application<ToDoState>
    {
        protected override IStore<ToDoState> InitStore => Redux.CreateDeepFreezeStore(ToDoReducer.Execute, ToDoReducer.InitState);
    }
}