using UnityEngine;

namespace UniRedux.Sample.Test
{
    [CreateAssetMenu(fileName = "TestByteToDoApplication", menuName = "UniRedux/Test/ByteToDoApplication", order = 3)]
    public class ScriptableObjectBinaryToDoApplication : Application<ToDoState>
    {
        protected override IStore<ToDoState> InitStore => Redux.CreateDeepFreezeStoreBinary(ToDoReducer.Execute, ToDoReducer.InitState);
    }
}