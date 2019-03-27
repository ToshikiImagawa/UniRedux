namespace UniRedux.Examples.EventSystem
{
    public class CompleteButton : CompleteButtonBase
    {
        protected override IStore<ToDoState> CurrentStore
            => ToDoApp.Store;
    }
}