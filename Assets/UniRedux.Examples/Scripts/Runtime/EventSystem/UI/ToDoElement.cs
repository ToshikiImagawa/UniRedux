namespace UniRedux.Examples.EventSystem
{
    public class ToDoElement : ToDoElementBase
    {
        protected override IStore<ToDoState> CurrentStore
            => ToDoApp.Store;
    }
}