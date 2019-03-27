namespace UniRedux.Examples.EventSystem
{
    public class SelectedToggle : SelectedToggleBase
    {
        protected override IStore<ToDoState> CurrentStore
            => ToDoApp.Store;
    }
}