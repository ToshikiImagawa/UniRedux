namespace UniRedux.Examples.EventSystem
{
    public class AddToDoButton : AddToDoButtonBase
    {
        protected override IStore<ToDoState> CurrentStore
            => ToDoApp.Store;
    }
}