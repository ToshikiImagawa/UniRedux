namespace UniRedux.Sample.Test
{
    public class SingletonBinaryToDoApplication : Application<ToDoState, SingletonBinaryToDoApplication>
    {
        protected override IStore<ToDoState> InitStore => Redux.CreateDeepFreezeStoreBinary(ToDoReducer.Execute, ToDoReducer.InitState);
    }
    public class SingletonJsonToDoApplication : Application<ToDoState, SingletonJsonToDoApplication>
    {
        protected override IStore<ToDoState> InitStore => Redux.CreateDeepFreezeStore(ToDoReducer.Execute, ToDoReducer.InitState);
    }
    public class SingletonToDoApplication : Application<ToDoState, SingletonToDoApplication>
    {
        protected override IStore<ToDoState> InitStore => Redux.CreateStore(ToDoReducer.Execute, ToDoReducer.InitState);
    }
}
