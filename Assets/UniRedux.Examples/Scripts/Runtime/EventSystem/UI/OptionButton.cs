namespace UniRedux.Examples.EventSystem
{
    public class OptionButton : OptionButtonBase
    {
        protected override IStore<ToDoState> CurrentStore
            => ToDoApp.Store;
    }
}