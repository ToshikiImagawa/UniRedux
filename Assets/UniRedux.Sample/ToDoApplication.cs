namespace UniRedux.Sample
{
    /// <summary>
    /// ToDo Application
    /// </summary>
    public class ToDoApplication : Application<ToDoState, ToDoApplication>
    {
        protected override IStore<ToDoState> InitStore
            => new Store<ToDoState>(ToDoReducer.Execute, ToDoReducer.InitState, UniReduxMiddleware.Logger);
    }
}