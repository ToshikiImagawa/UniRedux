namespace UniRedux.Examples.EventSystem
{
    public class SortButton : SortButtonBase
    {
        protected override IStore<ToDoState> CurrentStore
            => ToDoApp.Store;
    }
}