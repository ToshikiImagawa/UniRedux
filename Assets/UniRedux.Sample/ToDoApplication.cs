namespace UniRedux.Sample
{
    public class ToDoApplication : IApplication<ToDoListState>
    {
        public IStore<ToDoListState> CurrentStore => new Store<ToDoListState>(ToDoListReducer.Execute, ToDoListReducer.InitState);
    }
}